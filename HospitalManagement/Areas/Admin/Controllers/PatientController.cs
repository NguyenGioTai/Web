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
    public class PatientController : BaseController
    {
        // Controller quản lý bệnh nhân
        private readonly string KeyElement = "Bệnh nhân";

        public ActionResult Index()
        {
            // Trang chính (dashboard) của quản lý bệnh nhân
            ViewBag.Feature = "Bảng điều khiển";
            ViewBag.Element = KeyElement;
            ViewBag.BaseURL = "/Admin/Patient/All";
            return View();
        }

        [HttpPost]
        public JsonResult GetInfo(Guid id)
        {
            // Lấy thông tin chi tiết của 1 bệnh nhân theo ID
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var patient = workScope.Patients.FirstOrDefault(x => !x.IsDeleted && x.Id == id);

                return Json(new
                {
                    status = true,
                    mess = "Lấy thành công " + KeyElement,
                    data = new
                    {
                        patient.PatientCode,
                        patient.FullName,
                        DateOfBirth = patient.DateOfBirth.ToString("dd/MM/yyyy"),
                        patient.Address,
                        Age = DateTime.Now.Year - patient.DateOfBirth.Year, // Tính tuổi
                    }
                });
            }
        }

        public ActionResult All(string patientCode, string indentificationCardId, string fullName)
        {
            // Hiển thị danh sách bệnh nhân với tính năng lọc tìm kiếm
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;
            ViewBag.BaseURL = "/Admin/Patient";

            // Xử lý các giá trị tìm kiếm rỗng
            if (patientCode == "") patientCode = null;
            if (indentificationCardId == "") indentificationCardId = null;
            if (fullName == "") fullName = null;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var listData = workScope.Patients.Query(x => x.Status).OrderByDescending(x => x.JoinDate).ToList();

                // Lọc theo các tiêu chí
                var q = from mt in listData
                        where (!string.IsNullOrEmpty(patientCode) && mt.PatientCode.ToLower().Contains(patientCode.ToLower()))
                           || (!string.IsNullOrEmpty(indentificationCardId) && mt.IndentificationCardId.ToLower().Contains(indentificationCardId.ToLower()))
                           || (!string.IsNullOrEmpty(fullName) && mt.FullName.ToLower().Contains(fullName.ToLower()))
                        select mt;

                var user = GetCurrentUser();

                // Nếu là bác sĩ, chỉ hiển thị bệnh nhân của bác sĩ đó
                if (user.Role == RoleKey.Doctor)
                {
                    var patientOfDoctors = workScope.PatientDoctors.Query(x => x.DoctorId == user.DoctorId) ?? new List<PatientDoctor>();
                    var patientOfDoctorIds = patientOfDoctors.Select(x => x.PatientId);
                    listData = listData.Where(x => patientOfDoctorIds.Contains(x.Id)).ToList();
                    q = q.Where(x => patientOfDoctorIds.Contains(x.Id)).ToList();
                }

                // Nếu không có tìm kiếm, trả lại danh sách đầy đủ
                if (patientCode == null && indentificationCardId == null && fullName == null)
                {
                    return View(listData);
                }

                // Trả về danh sách sau khi lọc
                return View(q.OrderByDescending(x => x.JoinDate).ToList());
            }
        }

        public ActionResult Create()
        {
            // Màn hình tạo mới bệnh nhân
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = KeyElement;
            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1));

            string code;
            // Tạo mã bệnh nhân tự động
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var patient = workScope.Patients.GetAll().OrderByDescending(x => x.JoinDate).FirstOrDefault();
                if (patient != null)
                {
                    var codeSplit = patient.PatientCode.Split('-');
                    code = Common.Prefix + (int.Parse(codeSplit[1]) + 1);
                }
                else
                {
                    code = Common.Prefix + "1";
                }
            }

            ViewBag.Code = code;
            ViewBag.isEdit = false;
            return View();
        }

        public ActionResult Update(Guid id)
        {
            // Màn hình cập nhật bệnh nhân
            ViewBag.isEdit = true;
            ViewBag.Feature = "Cập nhật";
            ViewBag.Element = KeyElement;

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1));

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var patient = workScope.Patients.FirstOrDefault(x => x.Id == id);
                if (patient != null)
                {
                    return View("Create", patient); // Tái sử dụng View tạo
                }
                else
                {
                    return RedirectToAction("Create", "Patient");
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(Patient input, bool isEdit)
        {
            // Xử lý thêm hoặc cập nhật bệnh nhân
            try
            {
                var user = GetCurrentUser();

                if (isEdit) // Nếu là cập nhật
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.Patients.Get(input.Id);
                        if (elm != null)
                        {
                            input.Status = true;
                            input.PatientCode = elm.PatientCode; // Giữ nguyên mã bệnh nhân
                            elm = input;

                            workScope.Patients.Put(elm, elm.Id);
                            workScope.Complete();

                            return Json(new { status = true, mess = "Cập nhập thành công ", data = new { input.Id } });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                        }
                    }
                }
                else // Nếu là thêm mới
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        string code;
                        var patient = workScope.Patients.GetAll().OrderByDescending(x => x.JoinDate).FirstOrDefault();
                        if (patient != null)
                        {
                            var codeSplit = patient.PatientCode.Split('-');
                            code = Common.Prefix + (int.Parse(codeSplit[1]) + 1);
                        }
                        else
                        {
                            code = Common.Prefix + "1";
                        }

                        input.Id = Guid.NewGuid();
                        input.PatientCode = code;
                        input.JoinDate = DateTime.Now;
                        input.Status = true;

                        workScope.Patients.Add(input);
                        workScope.Complete();

                        // Nếu người dùng là bác sĩ thì gán thêm quan hệ bác sĩ-bệnh nhân
                        if (user.Role == RoleKey.Doctor)
                        {
                            workScope.PatientDoctors.Add(new PatientDoctor
                            {
                                PatientId = input.Id,
                                DoctorId = user.DoctorId.GetValueOrDefault(),
                                Status = 1
                            });
                            workScope.Complete();
                        }
                    }

                    return Json(new
                    {
                        status = true,
                        mess = "Thêm thành công " + KeyElement,
                        data = new { input.Id }
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Del(Guid id)
        {
            // Xử lý xóa bệnh nhân (mềm: đổi Status)
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var user = GetCurrentUser();
                    var elm = workScope.Patients.FirstOrDefault(x => !x.IsDeleted && x.Id == id);

                    // Kiểm tra quyền của bác sĩ
                    if (user.Role == RoleKey.Doctor)
                    {
                        var patientOfDoctors = workScope.PatientDoctors.Query(x => x.DoctorId == user.DoctorId && x.PatientId == id);
                        if (patientOfDoctors == null)
                        {
                            return Json(new { status = false, mess = "Không có quyền " + KeyElement });
                        }
                    }

                    if (elm != null)
                    {
                        elm.Status = false;
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
