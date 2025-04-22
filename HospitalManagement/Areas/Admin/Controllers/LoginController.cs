using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Utils;
using HospitalManagement.Areas.Admin.Authorization;
using HospitalManagement.Areas.Admin.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Administrator/Login/

        [HttpGet]
        public ActionResult Index(string returnUrl = "")
        {
            // Nếu người dùng đã đăng nhập, chuyển hướng về trang Dashboard
            if (CookiesManage.Logined())
            {
                return RedirectToAction("Index", "Dashboard");
            }
            // Truyền đường dẫn trở lại (nếu có) để sau khi đăng nhập xong quay lại đúng trang
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpGet]
        public ActionResult E401(string returnUrl = "")
        {
            // Trang lỗi 401 (không có quyền truy cập), cũng kiểm tra đã đăng nhập chưa
            if (CookiesManage.Logined())
            {
                return RedirectToAction("Index", "Dashboard");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateInput(true)]
        public JsonResult CheckLogin(LoginModel model)
        {
            // Kiểm tra thông tin đăng nhập
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Gọi phương thức kiểm tra tài khoản từ lớp nghiệp vụ
                var account = workScope.Accounts.ValidBEAccount(model.Username, model.Password);

                if (account != null)
                {
                    // Nếu tài khoản hợp lệ
                    if (HttpContext.Request.Url != null)
                    {
                        var host = HttpContext.Request.Url.Authority;

                        // Tạo chuỗi cookie chứa thông tin đăng nhập
                        var cookieClient = account.UserName + "|" + host.ToLower() + "|" + account.Id;

                        // Mã hóa cookie
                        var decodeCookieClient = CryptorEngine.Encrypt(cookieClient, true);

                        // Tạo cookie và thiết lập thời gian hết hạn
                        var userCookie = new HttpCookie(CookiesKey.Admin)
                        {
                            Value = decodeCookieClient,
                            Expires = DateTime.Now.AddDays(30) // cookie tồn tại trong 30 ngày
                        };

                        // Thêm cookie vào phản hồi để lưu trên trình duyệt
                        HttpContext.Response.Cookies.Add(userCookie);

                        // Trả về kết quả thành công dưới dạng JSON
                        return Json(new { status = true, mess = "Đăng nhập thành công" });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Đăng nhập Không thành công" });
                    }
                }
                else
                {
                    // Thông tin đăng nhập sai
                    return Json(new { status = false, mess = "Tên và mật khẩu không chính xác" });
                }
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            // Xử lý đăng xuất
            var nameCookie = Request.Cookies[CookiesKey.Admin];

            // Nếu không có cookie, quay lại trang đăng nhập
            if (nameCookie == null) return RedirectToAction("Index");

            // Tạo cookie mới để ghi đè cookie cũ, đặt thời gian hết hạn về quá khứ
            var newCookie = new HttpCookie(CookiesKey.Admin)
            {
                Expires = DateTime.Now.AddDays(-1d)
            };
            Response.Cookies.Add(newCookie);

            // Quay lại trang đăng nhập
            return RedirectToAction("Index");
        }
    }
}
