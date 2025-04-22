using BELibrary.Core.Entity;
using BELibrary.DbContext;
using BELibrary.Entity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class AttachmentController : BaseController
    {
        private const string KeyElement = "Tệp đính kèm";  // Xác định tên phần tử "Tệp đính kèm"

        // GET: Admin/Attachment
        public ActionResult Index(Guid detailRecordId)
        {
            ViewBag.Feature = "Danh sách";  // Tiêu đề "Danh sách"
            ViewBag.Element = KeyElement;  // Tiêu đề phần tử là "Tệp đính kèm"
            ViewBag.DetailRecordId = detailRecordId;  // Truyền ID chi tiết vào ViewBag

            ViewBag.BaseURL = "#";  // Đường dẫn cơ sở cho mục đích tùy chỉnh

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Lấy danh sách các tệp đính kèm liên quan đến detailRecordId
                var listData = workScope.AttachmentAssigns
                    .Include(x => x.Attachment)  // Lấy thông tin tệp đính kèm
                    .Where(x => x.DetailRecordId == detailRecordId)  // Lọc theo ID chi tiết
                    .ToList();

                return View(listData);  // Trả về danh sách tệp đính kèm
            }
        }

        // Phương thức trả về dữ liệu tệp đính kèm dưới dạng JSON
        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Lấy tệp đính kèm theo ID
                var attachment = workScope.Attachments.FirstOrDefault(x => x.Id == id);

                return attachment == default ?  // Nếu không tìm thấy tệp đính kèm
                    Json(new
                    {
                        status = false,
                        mess = "Có lỗi xảy ra: "
                    }) :
                    Json(new  // Nếu tệp đính kèm tồn tại
                    {
                        status = true,
                        mess = "Lấy thành công " + KeyElement,
                        data = new
                        {
                            attachment.Id,
                            attachment.Name,
                            attachment.Url,
                            attachment.Type
                        }
                    });
            }
        }

        // Phương thức tạo mới hoặc cập nhật tệp đính kèm
        [HttpPost, ValidateInput(false)]  // Không kiểm tra đầu vào HTML
        public JsonResult CreateOrEdit(Attachment input, AttachmentAssign assign, bool isEdit)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    if (isEdit)  // Nếu là cập nhật tệp đính kèm
                    {
                        var elm = workScope.Attachments.Get(input.Id);  // Lấy tệp đính kèm theo ID

                        if (elm != null)  // Nếu tệp đính kèm tồn tại
                        {
                            // Cập nhật thông tin tệp đính kèm
                            input.CreatedBy = elm.CreatedBy;
                            input.CreatedDate = elm.CreatedDate;
                            input.ModifiedDate = DateTime.Now;
                            input.ModifiedBy = GetCurrentUser().FullName;
                            elm = input;
                            workScope.Attachments.Put(elm, elm.Id);  // Lưu tệp đính kèm cập nhật
                            workScope.Complete();

                            return Json(new { status = true, mess = "Cập nhập thành công " });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                        }
                    }
                    else  // Nếu là tạo mới tệp đính kèm
                    {
                        input.Id = Guid.NewGuid();  // Tạo ID mới cho tệp đính kèm
                        input.CreatedBy = GetCurrentUser().FullName;
                        input.CreatedDate = DateTime.Now;
                        input.ModifiedDate = DateTime.Now;
                        input.ModifiedBy = GetCurrentUser().FullName;

                        workScope.Attachments.Add(input);  // Thêm tệp đính kèm mới
                        workScope.Complete();

                        // Gán tệp đính kèm vào chi tiết bản ghi
                        workScope.AttachmentAssigns.Add(new AttachmentAssign
                        {
                            Id = Guid.NewGuid(),
                            DetailRecordId = assign.DetailRecordId,
                            AttachmentId = input.Id
                        });

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

        // Phương thức xóa tệp đính kèm
        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.Attachments.Get(id);  // Lấy tệp đính kèm theo ID
                    if (elm != null)  // Nếu tệp đính kèm tồn tại
                    {
                        workScope.Attachments.Remove(elm);  // Xóa tệp đính kèm

                        // Xóa các bản ghi liên quan trong bảng AttachmentAssigns
                        var assigns = workScope.AttachmentAssigns.Query(x => x.AttachmentId == elm.Id);
                        foreach (var assign in assigns)
                        {
                            workScope.AttachmentAssigns.Remove(assign);  // Xóa gán tệp đính kèm cho chi tiết bản ghi
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
