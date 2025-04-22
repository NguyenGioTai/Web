using System.Web.Mvc;
using System.Web.Routing;

namespace HospitalManagement
{
    // Lớp này dùng để cấu hình định tuyến (routing) cho ứng dụng MVC
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Bỏ qua các request đến các tài nguyên tĩnh (ví dụ: WebResource.axd hay ScriptResource.axd)
            // Đây là các tài nguyên được ASP.NET tự động xử lý, không cần qua controller
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Thiết lập route mặc định cho ứng dụng
            routes.MapRoute(
                name: "Default", // Tên của route
                url: "{controller}/{action}/{id}", // Mẫu URL: controller/action/id
                defaults: new
                {
                    controller = "Home",  // Controller mặc định nếu không có trong URL
                    action = "Index",     // Action mặc định
                    id = UrlParameter.Optional // id là tùy chọn
                },
                // Xác định namespace để tìm controller (tránh lỗi nếu có nhiều controller trùng tên)
                new[] { "HospitalManagement.Controllers" }
            );
        }
    }
}
