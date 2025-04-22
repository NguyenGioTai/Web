using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class MedicalSupplyController : BaseController
    {
        // Chuỗi mô tả chức năng dùng trong các thông báo
        private const string KeyElement = "Cung cấp vật tư";

        /// <summary>
        /// Trang danh sách vật tư được cung cấp cho bệnh nhân cụ thể
        /// </summary>
        public ActionResult Index(Guid patientId)
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;
            ViewBag.PatientId = patientId;

            ViewBag.BaseURL = "#";

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Lấy danh sách vật tư theo bệnh nhân
                var listData = workScope.MedicalSupplies
                    .Include(x => x.Item).Where(x => x.PatientId == patientId).ToList();

                // Truyền danh sách tất cả vật tư để chọn khi thêm/sửa
                var items = workScope.Items.GetAll().ToList();
                ViewBag.Items = new SelectList(items, "Id", "Name");

                // Truyền danh sách trạng thái vật tư
                var lstStatus = StatusMedical.GetDic();
                ViewBag.ListStatus = new SelectList(lstStatus, "Value", "Text");

                return View(listData);
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của vật tư để hiển thị ở popup/sửa
        /// </summary>
        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var medicalSupply = workScope.MedicalSupplies.FirstOrDefault(x => x.Id == id);

                return medicalSupply == default ?
                    Json(new { status = false, mess = "Có lỗi xảy ra: " }) :
                    Json(new
                    {
                        status = true,
                        mess = "Lấy thành công " + KeyElement,
                        data = new
                        {
                            medicalSupply.Id,
                            medicalSupply.Amount,
                            medicalSupply.ItemId,
                            DateHide = medicalSupply.DateOfHire.ToString("g"), // định dạng ngày
                            medicalSupply.Status
                        }
                    });
            }
        }

        /// <summary>
        /// Thêm hoặc cập nhật vật tư cho bệnh nhân
        /// </summary>
        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(MedicalSupply input, AttachmentAssign assign, bool isEdit)
        {
            try
            {
                // Kiểm tra số lượng vật tư phải lớn hơn 0
                if (input.Amount <= 0)
                {
                    return Json(new { status = false, mess = "Lỗi số lượng" });
                }

                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var item = workScope.Items.FirstOrDefault(x => x.Id == input.ItemId);

                    // Tính số lượng tồn kho còn lại dựa trên trạng thái các vật tư đã cấp
                    var medicalSupplies = workScope.MedicalSupplies.Query(x => x.ItemId == input.ItemId).ToList();
                    var hireCount = medicalSupplies.Where(x => x.Status == StatusMedical.Hired).Sum(x => x.Amount);
                    var availabilityCount = medicalSupplies.Where(x => x.Status == StatusMedical.Availability).Sum(x => x.Amount);
                    var expiredCount = medicalSupplies.Where(x => x.Status == StatusMedical.Expired).Sum(x => x.Amount);
                    var unavailableCount = medicalSupplies.Where(x => x.Status == StatusMedical.Unavailable).Sum(x => x.Amount);
                    var maintenanceCount = medicalSupplies.Where(x => x.Status == StatusMedical.Maintenance).Sum(x => x.Amount);

                    // Số lượng có thể cấp = Tổng số - đã dùng - hỏng - đang bảo trì ...
                    var availabilityItem = item.Amount - hireCount - expiredCount - unavailableCount - maintenanceCount;

                    if (availabilityItem < 0)
                    {
                        return Json(new { status = false, mess = "Lỗi, dữ liệu âm " + KeyElement });
                    }

                    if (input.Amount > availabilityItem)
                    {
                        return Json(new { status = false, mess = $"Lỗi, kho đã hết " + KeyElement });
                    }

                    if (isEdit)
                    {
                        var elm = workScope.MedicalSupplies.Get(input.Id);

                        if (elm != null)
                        {
                            elm = input;
                            workScope.MedicalSupplies.Put(elm, elm.Id);
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
                        input.Id = Guid.NewGuid();

                        workScope.MedicalSupplies.Add(input);
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
        /// Xóa vật tư theo ID
        /// </summary>
        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.MedicalSupplies.Get(id);
                    if (elm != null)
                    {
                        workScope.MedicalSupplies.Remove(elm);
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
