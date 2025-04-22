using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class RecordController : BaseController
    {
        private readonly string KeyElement = "Bệnh án";

        // Trang hiển thị chi tiết bệnh án và các phiếu khám chi tiết
        public ActionResult Index(Guid id)
        {
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = KeyElement;

            var user = GetCurrentUser();

            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    // Lấy thông tin bản ghi bệnh án của bệnh nhân
                    var patientRecord = workScope.PatientRecords.FirstOrDefault(x => x.RecordId == id && !x.IsDelete);

                    // Kiểm tra quyền truy cập: chỉ bác sĩ phụ trách bệnh án mới được xem
                    if (user.Role == RoleKey.Doctor && patientRecord.DoctorId != user.DoctorId)
                    {
                        return RedirectToAction("E401", "Login");
                    }

                    if (patientRecord == null)
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }

                    // Lấy thông tin chính của bệnh án
                    var record = workScope.Records.FirstOrDefault(x => x.Id == patientRecord.RecordId);
                    var patient = workScope.Patients.FirstOrDefault(x => x.Id == patientRecord.PatientId);

                    // Nếu bệnh án chưa có, tạo mới
                    if (record == null)
                    {
                        record = new Record
                        {
                            Id = Guid.NewGuid(),
                            CreatedBy = user.FullName,
                            ModifiedBy = user.FullName,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            Note = "",
                            Result = "",
                            DoctorId = user.Role == RoleKey.Doctor ? user.DoctorId : null
                        };
                        workScope.Records.Add(record);
                        workScope.Complete();
                    }

                    ViewBag.Record = record;
                    ViewBag.Doctors = new SelectList(workScope.Doctors.GetAll().ToList(), "Id", "Name");
                    ViewBag.Faculties = new SelectList(workScope.Faculties.GetAll().ToList(), "Id", "Name");
                    ViewBag.ListStatus = new SelectList(StatusRecord.GetDic(), "Value", "Text");

                    // Lấy danh sách các chi tiết bệnh án phụ
                    var detailRecords = workScope.DetailRecords.Query(x => x.RecordId == record.Id && !x.IsMainRecord)
                        .OrderByDescending(x => x.Process).ToList();

                    // Lấy chi tiết bệnh án chính
                    var mainDetailRecords = workScope.DetailRecords.FirstOrDefault(x => x.RecordId == record.Id && x.IsMainRecord);

                    ViewBag.MainDetailRecords = mainDetailRecords;
                    ViewBag.Patient = patient;
                    ViewBag.DetailRecords = detailRecords;
                    ViewBag.BaseURL = "/admin/patientRecord?patientId=" + patient.Id;

                    return View(patientRecord);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // API lấy thông tin chi tiết bệnh án theo id
        [HttpPost]
        public JsonResult GetJson(Guid id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var detailRecord = workScope.DetailRecords.FirstOrDefault(x => x.Id == id);
                var user = GetCurrentUser();

                if (user.Role == RoleKey.Doctor && detailRecord.DoctorId != user.DoctorId)
                {
                    return Json(new { status = false, mess = "Không có quyền" });
                }

                return detailRecord == null
                    ? Json(new { status = false, mess = "Có lỗi xảy ra" })
                    : Json(new
                    {
                        status = true,
                        mess = "Lấy thành công " + KeyElement,
                        data = new
                        {
                            detailRecord.Id,
                            detailRecord.DiseaseName,
                            detailRecord.FacultyId,
                            detailRecord.DoctorId,
                            detailRecord.Note,
                            detailRecord.Status,
                            detailRecord.Result,
                            detailRecord.Process
                        }
                    });
            }
        }

        // API tạo hoặc cập nhật chi tiết bệnh án
        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(DetailRecord input, Guid detailDoctorId, bool isEdit)
        {
            try
            {
                var user = GetCurrentUser();

                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    if (isEdit) // Cập nhật
                    {
                        var elm = workScope.DetailRecords.Get(input.Id);
                        if (elm != null)
                        {
                            // Gán lại dữ liệu
                            elm = input;
                            elm.DoctorId = user.Role == RoleKey.Doctor ? user.DoctorId : detailDoctorId;

                            workScope.DetailRecords.Put(elm, elm.Id);
                            workScope.Complete();

                            return Json(new { status = true, mess = "Cập nhật thành công", data = new { detailRecordId = input.Id } });
                        }

                        return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                    }
                    else // Thêm mới
                    {
                        input.Id = Guid.NewGuid();
                        input.DoctorId = user.Role == RoleKey.Doctor ? user.DoctorId : detailDoctorId;

                        workScope.DetailRecords.Add(input);
                        workScope.Complete();

                        return Json(new { status = true, mess = "Thêm thành công " + KeyElement, data = new { detailRecordId = input.Id } });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // API cập nhật thông tin bệnh án chính
        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateRecord(Record input)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.Records.FirstOrDefault(x => x.Id == input.Id && !x.IsDelete);

                    if (elm != null)
                    {
                        input.CreatedBy = elm.CreatedBy;
                        input.CreatedDate = elm.CreatedDate;
                        input.ModifiedBy = GetCurrentUser().FullName;
                        input.ModifiedDate = DateTime.Now;

                        elm = input;

                        workScope.Records.Put(elm, elm.Id);
                        workScope.Complete();

                        return Json(new { status = true, mess = "Cập nhật thành công" });
                    }

                    return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // API xóa mềm bệnh án
        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.Records.FirstOrDefault(x => x.Id == id && !x.IsDelete);
                    var user = GetCurrentUser();

                    if (user.Role == RoleKey.Doctor && elm.DoctorId != user.DoctorId)
                    {
                        return Json(new { status = false, mess = "Không có quyền" });
                    }

                    if (elm != null)
                    {
                        elm.IsDelete = true;
                        workScope.Records.Put(elm, elm.Id);
                        workScope.Complete();

                        return Json(new { status = true, mess = "Xóa thành công " + KeyElement });
                    }

                    return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                }
            }
            catch
            {
                return Json(new { status = false, mess = "Thất bại" });
            }
        }
    }
}
