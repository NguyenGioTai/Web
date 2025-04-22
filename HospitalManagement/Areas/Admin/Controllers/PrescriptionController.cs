using BELibrary.Core.Entity;
using BELibrary.DbContext;
using BELibrary.Entity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class PrescriptionController : BaseController
    {
        // Định danh tên chức năng chính
        private const string KeyElement = "Đơn thuốc";

        // Hiển thị danh sách đơn thuốc theo DetailRecordId
        public ActionResult Index(Guid detailRecordId)
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;
            ViewBag.DetailRecordId = detailRecordId;
            ViewBag.BaseURL = "#";

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Lấy toàn bộ danh sách thuốc (Medicine) để hiển thị trong dropdown
                var medicines = workScope.Medicines.GetAll().ToList();
                ViewBag.Medicines = new SelectList(medicines, "Id", "Name");

                // Lấy danh sách đơn thuốc liên quan đến detailRecordId
                var listData = workScope.Prescriptions
                    .Include(x => x.DetailPrescription) // load thêm thông tin chi tiết thuốc
                    .Where(x => x.DetailRecordId == detailRecordId)
                    .ToList();

                return View(listData);
            }
        }

        // Lấy thông tin chi tiết một đơn thuốc để hiển thị trong form
        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var detail = workScope.DetailPrescriptions.FirstOrDefault(x => x.Id == id);

                return detail == default ?
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
                            detail.Id,
                            detail.MedicineId,
                            detail.Amount,
                            detail.Unit,
                            detail.Note
                        }
                    });
            }
        }

        // Tạo mới hoặc cập nhật đơn thuốc
        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(DetailPrescription input, Prescription prescription, bool isEdit)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    if (isEdit)
                    {
                        // Trường hợp cập nhật đơn thuốc
                        var elm = workScope.DetailPrescriptions.Get(input.Id);

                        if (elm != null)
                        {
                            // Ghi đè dữ liệu mới vào bản ghi cũ
                            elm = input;
                            workScope.DetailPrescriptions.Put(elm, elm.Id);
                            workScope.Complete();

                            return Json(new { status = true, mess = "Cập nhập thành công " });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                        }
                    }
                    else
                    {
                        // Trường hợp tạo mới đơn thuốc
                        input.Id = Guid.NewGuid();

                        // Thêm chi tiết đơn thuốc
                        workScope.DetailPrescriptions.Add(input);
                        workScope.Complete();

                        // Tạo đơn thuốc chính, liên kết với detailPrescription mới tạo
                        workScope.Prescriptions.Add(new Prescription
                        {
                            Id = Guid.NewGuid(),
                            DetailRecordId = prescription.DetailRecordId,
                            DetailPrescriptionId = input.Id,
                            CreatedBy = GetCurrentUser().FullName,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            ModifiedBy = GetCurrentUser().FullName
                        });

                        workScope.Complete();
                        return Json(new { status = true, mess = "Thêm thành công " + KeyElement });
                    }
                }
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có exception xảy ra
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // Xóa đơn thuốc (bao gồm cả Prescription và DetailPrescription)
        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    // Tìm chi tiết đơn thuốc theo ID
                    var elm = workScope.DetailPrescriptions.Get(id);
                    if (elm != null)
                    {
                        // Xóa chi tiết đơn thuốc
                        workScope.DetailPrescriptions.Remove(elm);

                        // Xóa tất cả các Prescription có liên kết với DetailPrescription này
                        var prescriptions = workScope.Prescriptions.Query(x => x.DetailPrescriptionId == elm.Id);
                        foreach (var prescription in prescriptions)
                        {
                            workScope.Prescriptions.Remove(prescription);
                        }

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
