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
    public class StatisticalController : BaseController
    {
        // GET: Admin/Statistical
        // Hiển thị trang thống kê và load danh sách danh mục (Categories) lên View
        public ActionResult Index()
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var categories = workScope.Categories.GetAll().ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View();
            }
        }

        // Trả về số lượng bệnh nhân đăng ký theo từng tháng trong một năm
        [HttpPost]
        public JsonResult GetRegByYear(int year)
        {
            var user = GetCurrentUser();

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Lấy toàn bộ danh sách bệnh nhân
                var patients = workScope.Patients.GetAll();

                // Nếu người dùng là bác sĩ, lọc danh sách bệnh nhân thuộc bác sĩ đó
                if (user.Role == RoleKey.Doctor)
                {
                    var patientOfDoctors = workScope.PatientDoctors.Query(x => x.DoctorId == user.DoctorId) ?? new List<PatientDoctor>();
                    var patientOfDoctorIds = patientOfDoctors.Select(x => x.PatientId);
                    patients = patients.Where(x => patientOfDoctorIds.Contains(x.Id)).ToList();
                }

                // Tạo danh sách các tháng trong năm được chỉ định
                var date = new DateTime(year, 1, 1);
                var months = Enumerable.Range(0, 12)
                    .Select(x => new
                    {
                        month = date.AddMonths(x).Month,
                        year = date.AddMonths(x).Year
                    }).ToList();

                // Nhóm bệnh nhân theo tháng và đếm số lượng mỗi tháng
                var dataPerYearAndMonth = months.GroupJoin(
                    patients,
                    m => new { m.month, m.year },
                    patient => new
                    {
                        month = patient.JoinDate.Month,
                        year = patient.JoinDate.Year
                    },
                    (p, g) => new
                    {
                        month = "Tháng " + p.month,
                        p.year,
                        count = g.Count()
                    });

                return Json(new
                {
                    status = true,
                    mess = "Thành công ",
                    data = dataPerYearAndMonth.ToList()
                });
            }
        }

        // Lấy số lượng vật tư theo từng trạng thái dựa trên danh mục (Category)
        [HttpPost]
        public JsonResult GetItemByCategory(Guid? categoryId)
        {
            if (!categoryId.HasValue)
            {
                return Json(new
                {
                    status = false,
                    mess = "Danh mục không tồn tại"
                });
            }

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Tổng số lượng vật tư thuộc danh mục
                var amountItem = workScope.Items
                    .Query(x => x.CategoryId == categoryId)
                    .Sum(item => item.Amount);

                // Lấy toàn bộ vật tư y tế có liên kết với danh mục
                var supplies = workScope.MedicalSupplies
                    .Include(x => x.Item)
                    .Where(x => x.Item.CategoryId == categoryId)
                    .ToList();

                // Đếm số lượng vật tư theo trạng thái
                var hireCount = supplies.Where(x => x.Status == StatusMedical.Hired).Sum(x => x.Amount);
                var availabilityCount = supplies.Where(x => x.Status == StatusMedical.Availability).Sum(x => x.Amount);
                var expiredCount = supplies.Where(x => x.Status == StatusMedical.Expired).Sum(x => x.Amount);
                var unavailableCount = supplies.Where(x => x.Status == StatusMedical.Unavailable).Sum(x => x.Amount);
                var maintenanceCount = supplies.Where(x => x.Status == StatusMedical.Maintenance).Sum(x => x.Amount);

                // Tính số lượng vật tư khả dụng thực tế
                var availabilityItem = amountItem - hireCount - expiredCount - unavailableCount - maintenanceCount;

                return Json(new
                {
                    status = true,
                    mess = "Thành công ",
                    data = new[]
                    {
                        new { label = "Đã sử dụng", value = hireCount },
                        new { label = "Khả dụng", value = availabilityItem },
                        new { label = "Không khả dụng", value = unavailableCount },
                        new { label = "Bảo trì", value = maintenanceCount },
                        new { label = "Hết Hạn", value = expiredCount }
                    }
                });
            }
        }
    }
}
