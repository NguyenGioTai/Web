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
    // Chỉ cho phép Admin truy cập controller này
    [Permission(Role = RoleKey.Admin)]
    public class FacultyController : BaseController
    {
        // Tên phần tử dùng để hiển thị trên giao diện
        private const string KeyElement = "Khoa";

        /// <summary>
        /// Trang danh sách các khoa
        /// </summary>
        /// <returns>View danh sách khoa</returns>
        public ActionResult Index()
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;

            if (Request.Url != null) ViewBag.BaseURL = Request.Url.LocalPath;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Lấy tất cả khoa từ database
                var listData = workScope.Faculties.GetAll().ToList();
                return View(listData);
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết một khoa dựa vào ID (dùng cho form edit)
        /// </summary>
        /// <param name="id">ID của khoa</param>
        /// <returns>JSON chứa thông tin khoa</returns>
        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var faculty = workScope.Faculties.FirstOrDefault(x => x.Id == id);

                return faculty == default ?
                    Json(new
                    {
                        status = false,
                        mess = "Có lỗi xảy ra: "
                    }) :
                    Json(new
                    {
                        status = true,
                        mess = "Lấy thành công " + KeyElement,
                        data = new
                        {
                            faculty.Id,
                            faculty.Name
                        }
                    });
            }
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật thông tin khoa
        /// </summary>
        /// <param name="input">Đối tượng Faculty truyền từ client</param>
        /// <param name="isEdit">Cờ xác định hành động là edit hay tạo mới</param>
        /// <returns>JSON kết quả</returns>
        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(Faculty input, bool isEdit)
        {
            try
            {
                if (isEdit)
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.Faculties.Get(input.Id);

                        if (elm != null) // Nếu khoa tồn tại thì cập nhật
                        {
                            elm = input; // Gán dữ liệu mới
                            workScope.Faculties.Put(elm, elm.Id);
                            workScope.Complete();

                            return Json(new { status = true, mess = "Cập nhập thành công " });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                        }
                    }
                }
                else // Thêm mới khoa
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        input.Id = Guid.NewGuid(); // Tạo ID mới
                        workScope.Faculties.Add(input); // Thêm vào DB
                        workScope.Complete();
                        return Json(new { status = true, mess = "Thêm thành công " + KeyElement });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        /// <summary>
        /// Xóa khoa theo ID
        /// </summary>
        /// <param name="id">ID của khoa</param>
        /// <returns>JSON phản hồi kết quả xóa</returns>
        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.Faculties.Get(id);
                    if (elm != null)
                    {
                        // Xóa khoa
                        workScope.Faculties.Remove(elm);
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
