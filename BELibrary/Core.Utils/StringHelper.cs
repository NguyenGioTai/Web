using System;
using System.Text;
using System.Text.RegularExpressions;

// Khai báo namespace BELibrary.Utils, nơi chứa các tiện ích hỗ trợ xử lý chuỗi
namespace BELibrary.Utils
{
    // Định nghĩa lớp tĩnh StringHelper chứa các phương thức tiện ích để xử lý và định dạng chuỗi
    public class StringHelper
    {
        // Phương thức ConvertToUnSign chuyển đổi chuỗi có dấu thành chuỗi không dấu (ASCII)
        public static string ConvertToUnSign(string text)
        {
            // Bước 1: Chuẩn hóa chuỗi về dạng Unicode FormD (phân tách dấu thanh và ký tự)
            // Ví dụ: "á" sẽ thành "a" + dấu sắc
            string normalized = text.Normalize(NormalizationForm.FormD);

            // Bước 2: Thay thế các khoảng trắng liên tiếp (nhiều hơn 1) bằng một khoảng trắng duy nhất
            // Ví dụ: "Hello   World" thành "Hello World"
            string singleSpace = new Regex(@"\s\s+").Replace(normalized, " ");

            // Bước 3: Thay thế ký tự đặc biệt 'đ' và 'Đ' bằng 'd' và 'D'
            string replacedD = singleSpace.Replace('\u0111', 'd').Replace('\u0110', 'D');

            // Bước 4: Loại bỏ tất cả các ký tự không phải chữ cái (a-z, A-Z), số (0-9), khoảng trắng hoặc dấu gạch dưới (_)
            // Ví dụ: các dấu thanh, ký tự đặc biệt như @, #, v.v. sẽ bị xóa
            string result = new Regex(@"[^a-zA-Z_0-9 \s]").Replace(replacedD, "");

            // Trả về chuỗi đã được xử lý
            return result;
        }

        // Phương thức ConvertToAlias tạo bí danh (alias) từ chuỗi, thường dùng cho URL hoặc tên định danh
        public static string ConvertToAlias(string text)
        {
            // Gọi ConvertToUnSign để chuyển chuỗi thành không dấu và loại bỏ ký tự không mong muốn
            // Sau đó thay thế tất cả khoảng trắng bằng dấu gạch ngang "-"
            // Ví dụ: "Hello World" thành "Hello-World"
            return ConvertToUnSign(text).Replace(" ", "-");
        }

        // Phương thức ConvertToH chuyển đổi số giây thành định dạng thời gian hh:mm:ss kèm hậu tố "(h)"
        public static string ConvertToH(double seconds)
        {
            // Chuyển số giây (double) thành đối tượng TimeSpan
            TimeSpan time = TimeSpan.FromSeconds(seconds);

            // Định dạng TimeSpan thành chuỗi dạng "hh:mm:ss"
            // Dấu "\" trước ":" để đảm bảo ":" được hiểu là ký tự, không phải phần của định dạng
            // Thêm hậu tố " (h)" để chỉ định đơn vị giờ
            // Ví dụ: 3665 giây sẽ thành "01:01:05 (h)"
            return time.ToString(@"hh\:mm\:ss") + " (h)";
        }
    }
}