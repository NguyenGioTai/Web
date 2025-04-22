using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using HospitalManagement.Areas.Admin.Authorization;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    // Chỉ Admin mới có quyền truy cập controller này
    [Permission(Role = RoleKey.Admin)]
    public class ItemController : BaseController
    {
        // Tên phần tử được dùng cho tiêu đề và thông báo
        private readonly string KeyElement = "Vật tư y tế";

        /// <summary>
        /// Trang danh sách vật tư y tế theo từng danh mục
        /// </summary>
        /// <param name="categoryId">ID danh mục vật tư y tế (nếu có)</param>
        /// <returns>View hiển thị danh sách vật tư</returns>
        public ActionResult Index(Guid? categoryId)
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;

            if (Request.Url != null) ViewBag.BaseURL = Request.Url.LocalPath;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Lấy tất cả danh mục
                var categories = workScope.Categories.GetAll().ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");

                // Nếu chưa chọn danh mục thì mặc định lấy cái đầu tiên
                if (!categoryId.HasValue && categories.Count > 0)
                {
                    categoryId = categories[0].Id;
                }

                ViewBag.CategoryId = categoryId;

                // Lấy danh sách vật tư theo danh mục
                var listData = workScope.Items.Query(x => x.CategoryId == categoryId).ToList();

                return View(listData);
            }
        }

        /// <summary>
        /// Hiển thị form thêm mới vật tư y tế
        /// </summary>
        public ActionResult Create()
        {
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = KeyElement;
            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1));

            ViewBag.IsEdit = false;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var categories = workScope.Categories.GetAll().ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
            }

            return View();
        }

        /// <summary>
        /// Hiển thị form cập nhật vật tư y tế
        /// </summary>
        /// <param name="id">ID của vật tư y tế</param>
        public ActionResult Update(Guid id)
        {
            ViewBag.isEdit = true;
            ViewBag.Feature = "Cập nhật";
            ViewBag.Element = KeyElement;

            if (Request.Url != null)
            {
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1));
            }

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var categories = workScope.Categories.GetAll().ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");

                var medicine = workScope.Items.FirstOrDefault(x => x.Id == id);

                if (medicine != null)
                {
                    return View("Create", medicine); // Dùng lại view Create
                }
                else
                {
                    return RedirectToAction("Create", "Item");
                }
            }
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật thông tin vật tư y tế
        /// </summary>
        /// <param name="input">Đối tượng vật tư y tế</param>
        /// <param name="isEdit">Cờ xác định là chỉnh sửa hay thêm mới</param>
        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(Item input, bool isEdit)
        {
            try
            {
                if (isEdit) // Nếu là cập nhật
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.Items.Get(input.Id);
                        if (elm != null)
                        {
                            // Giữ lại thông tin cũ
                            input.CreatedBy = elm.CreatedBy;
                            input.CreatedDate = elm.CreatedDate;

                            // Cập nhật thời gian & người sửa
                            input.ModifiedDate = DateTime.Now;
                            input.ModifiedBy = GetCurrentUser().FullName;

                            // Cập nhật vào DB
                            elm = input;
                            workScope.Items.Put(elm, elm.Id);
                            workScope.Complete();

                            return Json(new { status = true, mess = "Cập nhập thành công " });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                        }
                    }
                }
                else // Thêm mới
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        input.Id = Guid.NewGuid();
                        input.CreatedBy = GetCurrentUser().FullName;
                        input.CreatedDate = DateTime.Now;
                        input.ModifiedDate = DateTime.Now;
                        input.ModifiedBy = GetCurrentUser().FullName;

                        workScope.Items.Add(input);
                        workScope.Complete();
                    }
                    return Json(new { status = true, mess = "Thêm thành công " + KeyElement });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    mess = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Xóa vật tư y tế theo ID
        /// </summary>
        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.Items.Get(id);
                    if (elm != null)
                    {
                        workScope.Items.Remove(elm);
                        workScope.Complete();
                        return Json(new { status = true, mess = "Xóa thành công " + KeyElement });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                    }
                }
            }
            catch
            {
                return Json(new { status = false, mess = "Thất bại" });
            }
        }
    }
}
