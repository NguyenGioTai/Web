using System;
using System.Security.Cryptography;
using System.Text;

// Khai báo namespace BELibrary.Utils, nơi chứa các tiện ích (utilities) hỗ trợ mã hóa, sinh chuỗi ngẫu nhiên, v.v.
namespace BELibrary.Utils
{
    // Định nghĩa lớp tĩnh CodeUtils chứa các phương thức tiện ích để tạo chuỗi ngẫu nhiên và mã hóa
    public class CodeUtils
    {
        // Phương thức RandomString tạo một chuỗi ngẫu nhiên với số ký tự được chỉ định (mặc định là 10)
        public static string RandomString(int numberChar = 10)
        {
            // Định nghĩa tập hợp các ký tự có thể sử dụng: số (0-9) và chữ cái in hoa (A-Z)
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            // Chuyển chuỗi allChar thành mảng các ký tự riêng lẻ
            string[] allCharArray = allChar.Split(',');
            // Biến để lưu chuỗi ngẫu nhiên được tạo
            string randomCode = "";
            // Biến temp lưu giá trị chỉ số ký tự được chọn ở lần trước để tránh lặp lại
            int temp = -1;

            // Khởi tạo đối tượng Random để tạo số ngẫu nhiên
            Random rand = new Random();
            // Vòng lặp để tạo chuỗi với số ký tự yêu cầu
            for (int i = 0; i < numberChar; i++)
            {
                // Nếu temp đã được gán (tức là không phải lần đầu), sử dụng seed mới để tăng tính ngẫu nhiên
                // Seed dựa trên chỉ số i, temp và thời gian hiện tại (DateTime.Now.Ticks)
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                // Chọn ngẫu nhiên một chỉ số từ 0 đến 35 (tương ứng với 36 ký tự trong allCharArray)
                int t = rand.Next(36);
                // Nếu ký tự được chọn trùng với ký tự trước đó (temp), gọi đệ quy lại để tạo chuỗi mới
                if (temp != -1 && temp == t)
                {
                    return RandomString(numberChar);
                }
                // Cập nhật temp để lưu chỉ số ký tự hiện tại
                temp = t;
                // Thêm ký tự tương ứng vào chuỗi kết quả
                randomCode += allCharArray[t];
            }
            // Trả về chuỗi ngẫu nhiên đã tạo
            return randomCode;
        }

        // Phương thức GetLetter tạo một ký tự ngẫu nhiên từ tập hợp số (0-9) và chữ cái in hoa (A-Z)
        public static string GetLetter()
        {
            // Tập hợp ký tự giống như trong RandomString
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            // Chuyển thành mảng các ký tự
            string[] allCharArray = allChar.Split(',');
            // Biến temp để tránh lặp lại ký tự (giống RandomString)
            int temp = -1;
            // Khởi tạo đối tượng Random
            Random rand = new Random();
            // Nếu temp đã được gán, tạo Random với seed mới dựa trên temp và thời gian hiện tại
            if (temp != -1)
            {
                Random alimentation = new Random(temp * ((int)DateTime.Now.Ticks));
            }
            // Chọn ngẫu nhiên một chỉ số từ 0 đến 35
            int t = rand.Next(36);
            // Nếu ký tự trùng với ký tự trước, gọi đệ quy lại để chọn ký tự mới
            if (temp != -1 && temp == t)
            {
                return GetLetter();
            }
            // Cập nhật temp
            temp = t;
            // Trả về ký tự được chọn
            return allCharArray[t];
        }

        // Phương thức GxetLetter tạo một ký tự ngẫu nhiên dựa trên thời gian hiện tại
        public static char GxetLetter()
        {
            // Tập hợp ký tự có thể chọn: số (0-9) và chữ cái in hoa (A-Z)
            string chars = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            // Khởi tạo Random
            Random rand = new Random();
            // Lấy số mili giây hiện tại từ DateTime.Now
            int x = DateTime.Now.Millisecond;
            // Đảm bảo x nằm trong khoảng 0-35 (vì có 36 ký tự)
            while (x > 36)
            {
                x = x % 36;
            }
            // Lưu ý: Dòng code bị comment (rand.Next(x, chars.Length - 1)) cho thấy có thể trước đây dùng Random
            // Hiện tại, trả về ký tự tại vị trí x trong chuỗi chars
            return chars[x];
        }

        // Phương thức GetMd5Hash tạo mã băm MD5 từ một chuỗi đầu vào
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Chuyển chuỗi đầu vào thành mảng byte bằng mã hóa UTF-8
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            // Tạo StringBuilder để xây dựng chuỗi hexa từ mảng byte
            StringBuilder sBuilder = new StringBuilder();

            // Duyệt qua từng byte trong mảng dữ liệu đã băm
            // Chuyển mỗi byte thành chuỗi hexa (2 ký tự) và thêm vào StringBuilder
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Trả về chuỗi hexa đại diện cho mã băm MD5
            return sBuilder.ToString();
        }

        // Phương thức VerifyMd5Hash kiểm tra xem chuỗi đầu vào có khớp với mã băm MD5 hay không
        public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Tạo mã băm MD5 từ chuỗi đầu vào
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Tạo StringComparer để so sánh hai chuỗi băm, không phân biệt chữ hoa/thường
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            // So sánh mã băm của chuỗi đầu vào với mã băm được cung cấp
            // Nếu giống nhau, trả về true; nếu không, trả về false
            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}