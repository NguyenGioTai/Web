using System.Web.Mvc;

namespace HospitalManagement
{
    // Lớp này dùng để đăng ký các bộ lọc toàn cục (global filters)
    public class FilterConfig
    {
        // Phương thức này sẽ được gọi ở Global.asax để thêm các filter áp dụng cho toàn bộ ứng dụng
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Thêm bộ lọc xử lý lỗi mặc định
            // HandleErrorAttribute giúp hiển thị trang lỗi thân thiện nếu xảy ra exception
            // và tránh để lộ chi tiết lỗi ra ngoài (nếu không bật chế độ CustomErrors)
            filters.Add(new HandleErrorAttribute());
        }
    }
}
