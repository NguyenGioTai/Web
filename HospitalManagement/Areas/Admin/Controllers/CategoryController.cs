using BELibrary.Core.Entity;  // Các lớp đối tượng của hệ thống
using BELibrary.Core.Utils;   // Các tiện ích của hệ thống
using BELibrary.DbContext;    // Các lớp truy cập cơ sở dữ liệu
using BELibrary.Entity;       // Các thực thể của hệ thống
using HospitalManagement.Areas.Admin.Authorization;  // Phân quyền người dùng trong khu vực Admin
using System;
using System.Linq;
using System.Web.Mvc;  // Sử dụng ASP.NET MVC

namespace HospitalManagement.Areas.Admin.Controllers
{
    [Permission(Role = RoleKey.Admin)]  // Chỉ định rằng controller này yêu cầu quyền Admin
    public class CategoryController : BaseController
    {
        private const string KeyElement = "Danh mục";  // Tên của đối tượng được quản lý (Category)

        // GET: Admin/Category
        public ActionResult Index()
        {
            ViewBag.Feature = "Danh sách";  // Tiêu đề cho chức năng
            ViewBag.Element = KeyElement;  // Tên của đối tượng quản lý

            // Lấy URL hiện tại và hiển thị trong ViewBag (dành cho trường hợp cần thiết kế đường dẫn)
            if (Request.Url != null) ViewBag.BaseURL = Request.Url.LocalPath;

            // Lấy danh sách tất cả các Category từ cơ sở dữ liệu
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var listData = workScope.Categories.GetAll().ToList();  // Truy vấn tất cả các danh mục
                return View(listData);  // Trả về danh sách này trong View
            }
        }

        // Lấy thông tin Category theo ID
        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Tìm Category với ID đã cho
                var category = workScope.Categories.FirstOrDefault(x => x.Id == id);
                // Nếu tìm thấy, trả về dữ liệu, nếu không thì trả về thông báo lỗi
                return category == default ?
                    Json(new
                    {
                        status = false,
                        mess = "Có lỗi xảy ra: "
                    }) :
                    Json(new
                    {
                        status = true,
                        mess = "Lấy thành công " + KeyElement,
                        data = category
                    });
            }
        }

        // Thêm mới hoặc cập nhật Category
        [HttpPost, ValidateInput(false)]  // Không kiểm tra đầu vào (cho phép sử dụng HTML trong nội dung)
        public JsonResult CreateOrEdit(Category input, bool isEdit)
        {
            try
            {
                // Nếu là chỉnh sửa (isEdit = true)
                if (isEdit)
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        // Cập nhật Category trong cơ sở dữ liệu
                        workScope.Categories.Put(input, input.Id);
                        workScope.Complete();
                        return Json(new { status = true, mess = "Sửa thành công " + KeyElement });
                    }
                }
                else
                {
                    // Nếu là thêm mới (isEdit = false)
                    input.Id = Guid.NewGuid();  // Tạo ID mới cho Category
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        // Thêm Category vào cơ sở dữ liệu
                        workScope.Categories.Add(input);
                        workScope.Complete();
                        return Json(new { status = true, mess = "Thêm thành công " + KeyElement });
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // Xóa Category theo ID
        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    // Gọi phương thức xóa Category từ cơ sở dữ liệu
                    workScope.Categories.Del(id, GetCurrentUser().Id);
                    return Json(new { status = true, mess = "Xóa thành công " + KeyElement });
                }
            }
            catch
            {
                // Xử lý lỗi khi xóa thất bại
                return Json(new { status = false, mess = "Thất bại" });
            }
        }
    }
}
