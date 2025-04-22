using BELibrary.Core.Entity;
using BELibrary.DbContext;
using BELibrary.Entity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class DoctorController : BaseController
    {
        // Khai báo tên phần tử dùng để hiển thị trong View
        private const string KeyElement = "Bác sĩ";

        /// <summary>
        /// Trang hiển thị danh sách bác sĩ
        /// </summary>
        /// <returns>View chứa danh sách bác sĩ</returns>
        public ActionResult Index()
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;

            if (Request.Url != null) ViewBag.BaseURL = Request.Url.LocalPath;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Lấy danh sách bác sĩ và bao gồm thông tin khoa
                var listData = workScope.Doctors.Include(x => x.Faculty).ToList();

                // Lấy danh sách khoa để hiển thị dropdown
                var faculties = workScope.Faculties.GetAll().ToList();
                ViewBag.Faculties = new SelectList(faculties, "Id", "Name");

                return View(listData);
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết bác sĩ theo ID, trả về JSON
        /// </summary>
        /// <param name="id">ID của bác sĩ</param>
        /// <returns>JSON chứa thông tin bác sĩ</returns>
        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var doctor = workScope.Doctors.FirstOrDefault(x => x.Id == id);

                return doctor == default ?
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
                            doctor.Id,
                            doctor.Name,
                            doctor.Address,
                            doctor.Email,
                            doctor.Phone,
                            doctor.FacultyId,
                            doctor.Gender,
                            doctor.Avatar,
                            doctor.Descriptions
                        }
                    });
            }
        }

        /// <summary>
        /// Lấy danh sách bác sĩ theo khoa (nếu có), trả về ID và tên
        /// </summary>
        /// <param name="facultyId">ID của khoa (tùy chọn)</param>
        /// <returns>JSON chứa danh sách bác sĩ</returns>
        [HttpPost]
        public JsonResult GetDoctors(Guid? facultyId)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var lst = facultyId.HasValue
                    ? workScope.Doctors.Query(x => x.FacultyId == facultyId).ToList()
                    : workScope.Doctors.GetAll().ToList();

                return Json(new
                {
                    status = true,
                    mess = "Lấy thành công " + KeyElement,
                    data = lst.Select(x => new
                    {
                        x.Id,
                        x.Name
                    })
                });
            }
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật bác sĩ
        /// </summary>
        /// <param name="input">Đối tượng bác sĩ</param>
        /// <param name="isEdit">Cờ xác định là sửa hay thêm mới</param>
        /// <returns>JSON kết quả thực hiện</returns>
        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(Doctor input, bool isEdit)
        {
            try
            {
                if (isEdit)
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.Doctors.Get(input.Id);

                        if (elm != null) // Nếu tồn tại, cập nhật
                        {
                            elm = input;
                            workScope.Doctors.Put(elm, elm.Id);
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
                        workScope.Doctors.Add(input);
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
        /// Xóa bác sĩ theo ID
        /// </summary>
        /// <param name="id">ID của bác sĩ cần xóa</param>
        /// <returns>JSON kết quả xóa</returns>
        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.Doctors.Get(id);
                    if (elm != null)
                    {
                        workScope.Doctors.Remove(elm);
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
