using BELibrary.Core.Utils;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HospitalManagement.Areas.Admin.Authorization
{
    // Custom attribute kế thừa từ AuthorizeAttribute để kiểm tra quyền truy cập theo Role
    public class PermissionAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Role yêu cầu để truy cập:
        /// 1. Admin
        /// 2. Employee
        /// 3. Customer
        /// </summary>
        public int Role { set; get; }

        /// <summary>
        /// Kiểm tra người dùng có quyền truy cập hay không
        /// </summary>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Lấy người dùng hiện tại từ cookie
            var user = CookiesManage.GetUser();

            if (user != null)
            {
                // Cho phép truy cập nếu:
                // - Vai trò người dùng trùng khớp với Role yêu cầu
                // - Hoặc người dùng là Admin (Admin có toàn quyền)
                return this.Role == user.Role || user.Role == RoleKey.Admin;
            }

            // Nếu chưa đăng nhập hoặc không có quyền thì từ chối
            return false;
        }

        /// <summary>
        /// Xử lý khi không được cấp quyền truy cập
        /// </summary>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Gửi thông báo lỗi thông qua TempData
            var TempData = filterContext.Controller.TempData;
            TempData["Messages"] = "Bạn không có quyền truy cập mục này";

            // Chuyển hướng đến trang lỗi 401 trong khu vực admin
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "action", "E401" },
                    { "controller", "Login" },
                    { "Area", "admin" }
                });
        }
    }
}
