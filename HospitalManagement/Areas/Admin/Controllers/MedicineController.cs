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
    // Controller này xử lý các thao tác CRUD (tạo, đọc, cập nhật, xóa) cho thực thể "Thuốc" trong khu vực quản trị.
    [Permission(Role = RoleKey.Admin)] // Chỉ có admin mới có quyền truy cập vào controller này.
    public class MedicineController : BaseController
    {
        private readonly string KeyElement = "Thuốc"; // Biến này dùng để hiển thị tên thực thể trong các thông báo.

        // GET: Admin/Gallery
        public ActionResult Index()
        {
            ViewBag.Feature = "Danh sách"; // Gán tên chức năng hiển thị trong giao diện.
            ViewBag.Element = KeyElement; // Gán tên thực thể hiển thị trong giao diện.

            if (Request.Url != null) ViewBag.BaseURL = Request.Url.LocalPath; // Lưu URL gốc của yêu cầu hiện tại.

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Lấy danh sách tất cả thuốc từ cơ sở dữ liệu.
                var listData = workScope.Medicines.GetAll().ToList();
                return View(listData);
            }
        }

        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Tìm thuốc theo ID được truyền vào.
                var medicine = workScope.Medicines.FirstOrDefault(x => x.Id == id);

                // Nếu tìm thấy, trả về thông tin thuốc dưới dạng JSON, nếu không thì báo lỗi.
                return medicine == default ?
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
                            medicine.Id,
                            medicine.Name,
                            medicine.Description
                        }
                    });
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(Medicine input, bool isEdit)
        {
            try
            {
                if (isEdit) // Cập nhật thuốc đã có
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.Medicines.Get(input.Id);

                        if (elm != null)
                        {
                            elm = input; // Cập nhật thông tin thuốc.

                            workScope.Medicines.Put(elm, elm.Id); // Lưu thay đổi vào cơ sở dữ liệu.
                            workScope.Complete();

                            return Json(new { status = true, mess = "Cập nhập thành công " });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                        }
                    }
                }
                else // Thêm thuốc mới
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        input.Id = Guid.NewGuid(); // Tạo một ID mới duy nhất cho thuốc.

                        workScope.Medicines.Add(input); // Thêm thuốc mới vào cơ sở dữ liệu.
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

        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.Medicines.Get(id);
                    if (elm != null)
                    {
                        workScope.Medicines.Remove(elm); // Xóa thuốc khỏi cơ sở dữ liệu.
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
