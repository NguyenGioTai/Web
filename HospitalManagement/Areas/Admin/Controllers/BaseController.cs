using HospitalManagement.Areas.Admin.Authorization;  // Sử dụng lớp xác thực từ khu vực Admin
using System.Web.Mvc;  // Sử dụng các lớp của ASP.NET MVC

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: /Administrator/Base/
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra nếu người dùng chưa đăng nhập
            if (!CookiesManage.Logined())
            {
                // Lưu lại URL hiện tại để trả về sau khi đăng nhập thành công
                var returnUrl = filterContext.HttpContext.Request.RawUrl;
                // Chuyển hướng người dùng đến trang đăng nhập và thêm tham số ReturnUrl để quay lại trang hiện tại sau khi đăng nhập
                filterContext.Result =
                    new RedirectResult(string.Concat("~/Admin/Login/Index", "?ReturnUrl=", returnUrl));
            }
            base.OnActionExecuting(filterContext);  // Gọi phương thức gốc của OnActionExecuting để tiếp tục xử lý hành động
        }

        // Phương thức trả về thông tin người dùng hiện tại từ cookie
        public static BELibrary.Entity.Account GetCurrentUser()
        {
            // Trả về tài khoản người dùng từ thông tin lưu trong cookie
            return CookiesManage.GetUser();
        }
    }
}
