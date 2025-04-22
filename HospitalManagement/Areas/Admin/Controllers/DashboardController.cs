using BELibrary.Core.Entity;   // Các lớp đối tượng của hệ thống
using BELibrary.Core.Utils;    // Các tiện ích của hệ thống
using BELibrary.DbContext;     // Các lớp truy cập cơ sở dữ liệu
using BELibrary.Entity;        // Các thực thể của hệ thống
using System;
using System.Collections.Generic;  // Dùng cho List
using System.Linq;              // Dùng cho LINQ
using System.Web.Mvc;           // Sử dụng ASP.NET MVC

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            ViewBag.Element = "Hệ thống";  // Tiêu đề của phần tử quản lý (Hệ thống)
            ViewBag.Feature = "Bảng điều khiển";  // Tiêu đề cho chức năng
            if (Request.Url != null) ViewBag.BaseURL = Request.Url.LocalPath;  // Lưu đường dẫn hiện tại vào ViewBag

            // Lấy thông tin người dùng hiện tại
            var user = GetCurrentUser();

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Lấy tất cả các tài liệu đính kèm (documents)
                var documents = workScope.Attachments.GetAll().ToList();

                // Lấy tất cả bệnh nhân
                var patients = workScope.Patients.GetAll().ToList();

                // Nếu người dùng là bác sĩ, chỉ lấy những bệnh nhân thuộc quyền bác sĩ đó
                if (user.Role == RoleKey.Doctor)
                {
                    var patientOfDoctors =
                        workScope.PatientDoctors.Query(x => x.DoctorId == user.DoctorId) ?? new List<PatientDoctor>();

                    var patientOfDoctorIds = patientOfDoctors.Select(x => x.PatientId);
                    patients = patients.Where(x => patientOfDoctorIds.Contains(x.Id)).ToList();
                }

                // Lấy tất cả đơn thuốc (prescriptions)
                var prescriptions = workScope.Prescriptions.GetAll().ToList();

                // Lấy tất cả các món hàng (items)
                var items = workScope.Items.GetAll().ToList();

                // Lấy tất cả lịch khám của bác sĩ (schedules)
                var schedules = workScope.DoctorSchedules.GetAll().ToList();

                // Nếu người dùng là bác sĩ, chỉ lấy lịch khám của bác sĩ đó
                if (user.Role == RoleKey.Doctor)
                {
                    schedules = schedules.Where(x => x.DoctorId == user.DoctorId).ToList();
                }

                // Tính tổng số tài liệu, bệnh nhân, đơn thuốc, món hàng và lịch khám
                ViewBag.DocumentCount = documents.Count;
                ViewBag.PatientCount = patients.Count;
                ViewBag.PrescriptionCount = prescriptions.Count;
                ViewBag.ItemCount = items.Count;
                ViewBag.ScheduleCount = schedules.Count;

                var now = DateTime.Now;  // Lấy ngày giờ hiện tại

                // Tính số tài liệu trong ngày và tháng hiện tại
                ViewBag.DocumentTodayCount = documents.Count(x => x.ModifiedDate.Day == now.Day && x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);
                ViewBag.DocumentMonthCount = documents.Count(x => x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);

                // Tính số lịch khám trong ngày và tháng hiện tại
                ViewBag.ScheduleTodayCount = schedules.Count(x => x.ScheduleBook.Day == now.Day && x.ScheduleBook.Month == now.Month && x.ScheduleBook.Year == now.Year);
                ViewBag.ScheduleMonthCount = schedules.Count(x => x.ScheduleBook.Month == now.Month && x.ScheduleBook.Year == now.Year);

                // Tính số bệnh nhân trong ngày và tháng hiện tại
                ViewBag.PatientTodayCount = patients.Count(x => x.JoinDate.Day == now.Day && x.JoinDate.Month == now.Month && x.JoinDate.Year == now.Year);
                ViewBag.PatientMonthCount = patients.Count(x => x.JoinDate.Month == now.Month && x.JoinDate.Year == now.Year);

                // Tính số đơn thuốc trong ngày và tháng hiện tại
                ViewBag.PrescriptionTodayCount = prescriptions.Count(x => x.ModifiedDate.Day == now.Day && x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);
                ViewBag.PrescriptionMonthCount = documents.Count(x => x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);

                // Tính số món hàng trong ngày và tháng hiện tại
                ViewBag.ItemTodayCount = items.Count(x => x.ModifiedDate.Day == now.Day && x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);
                ViewBag.ItemMonthCount = items.Count(x => x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);

                // Lấy danh sách bệnh nhân mới nhất (6 bệnh nhân mới)
                var patientsNew = workScope.Patients.Query(x => x.Status).OrderByDescending(x => x.JoinDate).Take(6).ToList();

                // Nếu người dùng là bác sĩ, chỉ lấy những bệnh nhân thuộc quyền bác sĩ đó
                if (user.Role == RoleKey.Doctor)
                {
                    var patientOfDoctors =
                        workScope.PatientDoctors.Query(x => x.DoctorId == user.DoctorId) ?? new List<PatientDoctor>();

                    var patientOfDoctorIds = patientOfDoctors.Select(x => x.PatientId);
                    patientsNew = patientsNew.Where(x => patientOfDoctorIds.Contains(x.Id)).ToList();
                }

                // Gửi danh sách bệnh nhân mới nhất vào ViewBag
                ViewBag.PatientsNew = patientsNew;

                // Lấy tất cả các danh mục và gửi vào ViewBag để sử dụng trong dropdown list
                var categories = workScope.Categories.GetAll().ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
            }

            return View();  // Trả về view dashboard với các dữ liệu đã tính toán và gửi vào ViewBag
        }
    }
}
