using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using BELibrary.Utils;
using System.Web;

namespace HospitalManagement.Areas.Admin.Authorization
{
    // Lớp quản lý thông tin đăng nhập thông qua Cookies
    public class CookiesManage
    {
        /// <summary>
        /// Kiểm tra xem Admin đã đăng nhập hay chưa
        /// </summary>
        /// <returns>True nếu đã đăng nhập, ngược lại False</returns>
        public static bool Logined()
        {
            // Lấy cookie của Admin
            var cookiesClient = HttpContext.Current.Request.Cookies.Get(CookiesKey.Admin);

            if (cookiesClient != null)
            {
                // Giải mã giá trị của cookie
                var decodeCookie = CryptorEngine.Decrypt(cookiesClient.Value, true);
                var vals = decodeCookie.Split('|');

                // Kiểm tra tên miền của cookie có khớp với tên miền hiện tại không
                var host = HttpContext.Current.Request.Url.Authority;
                if (host.ToLower() != vals[1].ToLower())
                {
                    return false;
                }

                // Kiểm tra xem tài khoản có tồn tại trong database không
                using (var unitofwork = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var us = unitofwork.Accounts.GetAccountByUsername(vals[0]);
                    return us != null;
                }
            }

            return false;
        }

        /// <summary>
        /// Trả về thông tin tài khoản Admin hiện tại (nếu đang đăng nhập)
        /// </summary>
        /// <returns>Đối tượng Account hoặc null nếu chưa đăng nhập</returns>
        public static Account GetUser()
        {
            // Lấy cookie của Admin
            var cookiesClient = HttpContext.Current.Request.Cookies.Get(CookiesKey.Admin);

            if (cookiesClient != null)
            {
                // Giải mã cookie
                var decodeCookie = CryptorEngine.Decrypt(cookiesClient.Value, true);
                var vals = decodeCookie.Split('|');

                // Kiểm tra tên miền
                var host = HttpContext.Current.Request.Url.Authority;
                if (host.ToLower() != vals[1].ToLower())
                {
                    return null;
                }

                // Truy vấn tài khoản từ database
                using (var unitofwork = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var us = unitofwork.Accounts.GetAccountByUsername(vals[0]);
                    return us ?? null;
                }
            }

            return null;
        }

        /// <summary>
        /// Xóa tất cả session hiện tại
        /// </summary>
        public static void ClearAll()
        {
            HttpContext.Current.Session.RemoveAll();
        }
    }
}
