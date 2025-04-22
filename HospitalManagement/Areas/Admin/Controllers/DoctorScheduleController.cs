using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class DoctorScheduleController : BaseController
    {
        // Tên phần tử sẽ hiển thị ở view
        private const string KeyElement = "Đặt lịch";

        /// <summary>
        /// Trang danh sách các lịch hẹn bác sĩ
        /// </summary>
        /// <returns>Danh sách lịch hẹn</returns>
        public ActionResult Index()
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;

            // Lấy thông tin người dùng hiện tại
            var user = GetCurrentUser();

            if (Request.Url != null) ViewBag.BaseURL = Request.Url.LocalPath;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Tạo dropdown danh sách trạng thái đặt lịch
                ViewBag.Status = new SelectList(new List<object>
                {
                    new
                    {
                        Id = BookingStatusKey.Active,
                        Name = BookingStatusKey.GetText(BookingStatusKey.Active),
                    },
                    new
                    {
                        Id = BookingStatusKey.Pending,
                        Name = BookingStatusKey.GetText(BookingStatusKey.Pending),
                    },
                    new
                    {
                        Id = BookingStatusKey.Reject,
                        Name = BookingStatusKey.GetText(BookingStatusKey.Reject),
                    },
                }, "Id", "Name");

                // Lấy danh sách bệnh nhân chưa bị xóa
                var listPatient = workScope.Patients.Query(x => !x.IsDeleted);

                // Lấy tất cả lịch đặt
                var doctorSchedules = workScope.DoctorSchedules.GetAll();

                // Lấy danh sách bác sĩ
                var doctors = workScope.Doctors.GetAll();

                // Nếu người dùng là bác sĩ, chỉ hiển thị lịch liên quan đến bác sĩ đó
                if (user.Role == RoleKey.Doctor)
                {
                    doctorSchedules = doctorSchedules.Where(x => x.DoctorId == user.DoctorId);
                    doctors = doctors.Where(x => x.Id == user.DoctorId);
                }

                // Kết hợp thông tin bác sĩ và bệnh nhân để hiển thị danh sách
                var listData = (from ds in doctorSchedules
                                join p in listPatient on ds.PatientId equals p.Id
                                join d in doctors on ds.DoctorId equals d.Id
                                select ds).ToList();

                return View(listData);
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết lịch hẹn theo ID
        /// </summary>
        /// <param name="id">ID lịch hẹn</param>
        /// <returns>Trả về JSON chứa thông tin lịch hẹn</returns>
        [HttpPost]
        public JsonResult GetJson(int id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var doctor = workScope.DoctorSchedules.FirstOrDefault(x => x.Id == id);

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
                            doctor.Status
                        }
                    });
            }
        }

        /// <summary>
        /// Cập nhật thông tin lịch hẹn
        /// </summary>
        /// <param name="input">Thông tin lịch hẹn</param>
        /// <param name="isEdit">Cờ kiểm tra có phải đang sửa hay không</param>
        /// <returns>JSON phản hồi kết quả</returns>
        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(DoctorSchedule input, bool isEdit)
        {
            try
            {
                if (isEdit)
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.DoctorSchedules.Get(input.Id);

                        if (elm != null) // Nếu tồn tại, cập nhật
                        {
                            // Chỉ được phép thay đổi status, không thay đổi các thuộc tính khác
                            input.DoctorId = elm.DoctorId;
                            input.PatientId = elm.PatientId;
                            input.ScheduleBook = elm.ScheduleBook;
                            elm = input;

                            workScope.DoctorSchedules.Put(elm, elm.Id);
                            workScope.Complete();

                            return Json(new { status = true, mess = "Cập nhập thành công " });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                        }
                    }
                }
                else
                {
                    // Không cho phép tạo mới lịch từ Admin
                    return Json(new { status = true, mess = "Method not allow" + KeyElement });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        /// <summary>
        /// Xóa lịch hẹn theo ID
        /// </summary>
        /// <param name="id">ID của lịch cần xóa</param>
        /// <returns>JSON kết quả thực hiện</returns>
        [HttpPost]
        public JsonResult Del(int id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.DoctorSchedules.Get(id);
                    if (elm != null)
                    {
                        workScope.DoctorSchedules.Remove(elm);
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
