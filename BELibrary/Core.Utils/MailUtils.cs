using System;
using System.Net;
using System.Net.Mail;
using System.Threading;

// Khai báo namespace BELibrary.Utils, nơi chứa các tiện ích hỗ trợ, bao gồm gửi email
namespace BELibrary.Utils
{
    // Định nghĩa lớp tĩnh MailUtils chứa các phương thức để gửi email và tạo khóa ngẫu nhiên
    public class MailUtils
    {
        // Phương thức Generatekey tạo một khóa ngẫu nhiên theo định dạng "SHOP-XXXX-XXXX-XXXX-XXXX-XXXX"
        // Mỗi XXXX là một chuỗi ngẫu nhiên 4 ký tự
        protected static string Generatekey()
        {
            // Khởi tạo khóa với tiền tố "SHOP"
            string key = "SHOP";
            // Vòng lặp để thêm 5 chuỗi ngẫu nhiên, mỗi chuỗi dài 4 ký tự
            for (int i = 0; i < 5; i++)
            {
                // Gọi phương thức RandomString từ lớp CodeUtils để tạo chuỗi ngẫu nhiên
                string s = CodeUtils.RandomString(4);
                // Tạm dừng 50ms để tránh tạo các chuỗi giống nhau do thời gian gần nhau
                Thread.Sleep(50);
                // Thêm chuỗi ngẫu nhiên vào khóa, cách nhau bằng dấu "-"
                key += "-" + s;
            }
            // Trả về khóa hoàn chỉnh (ví dụ: "SHOP-ABCD-EFGH-IJKL-MNOP-QRST")
            return key;
        }

        // Phương thức SendEmail gửi email đến địa chỉ được chỉ định với tiêu đề và nội dung
        // Trả về đối tượng ResponseEmail để báo cáo trạng thái gửi và mã khóa
        public static ResponseEmail SendEmail(string mailTo, string subject, string content)
        {
            // Tạo mã khóa ngẫu nhiên bằng phương thức Generatekey
            string code = Generatekey();
            try
            {
                // Khởi tạo đối tượng SmtpClient để kết nối với máy chủ SMTP của Gmail
                var client = new SmtpClient
                {
                    Host = "smtp.gmail.com", // Máy chủ SMTP của Gmail
                    Port = 587, // Cổng chuẩn cho Gmail với TLS
                    UseDefaultCredentials = false, // Không sử dụng thông tin xác thực mặc định
                    DeliveryMethod = SmtpDeliveryMethod.Network, // Phương thức gửi qua mạng
                    Credentials = new NetworkCredential(
                        "email@gmail.com", // Địa chỉ email dùng để gửi
                        "password"), // Mật khẩu của email (hard-coded, không an toàn)
                    EnableSsl = true, // Bật SSL/TLS để bảo mật kết nối
                };

                // Tạo địa chỉ email người gửi (hiển thị tên "A - Movie")
                var from = new MailAddress("phucnd.hvit@gmail.com", "A - Movie");
                // Tạo địa chỉ email người nhận
                var to = new MailAddress(mailTo);
                // Tạo đối tượng MailMessage chứa thông tin email
                var mail = new MailMessage(from, to)
                {
                    Subject = subject, // Tiêu đề email
                    Body = content, // Nội dung email
                    IsBodyHtml = true, // Cho phép nội dung email dạng HTML
                };

                // Gửi email qua máy chủ SMTP
                client.Send(mail);

                // Trả về ResponseEmail với trạng thái thành công, mã khóa và không có lỗi
                return new ResponseEmail(true, code, "");
                // Lưu ý: Để gửi email từ Gmail, tài khoản cần bật "Less Secure Apps" 
                // (https://www.google.com/settings/u/1/security/lesssecureapps)
                // Tuy nhiên, tùy chọn này đã bị Google loại bỏ từ năm 2022, cần sử dụng App Password
            }
            catch (Exception ex)
            {
                // Nếu có lỗi khi gửi email, trả về ResponseEmail với trạng thái thất bại, mã khóa và thông báo lỗi
                return new ResponseEmail(false, code, ex.Message);
            }
        }
    }

    // Lớp ResponseEmail định nghĩa cấu trúc dữ liệu để trả về kết quả gửi email
    public class ResponseEmail
    {
        // Thuộc tính Status: true nếu gửi email thành công, false nếu thất bại
        public bool Status { get; set; }
        // Thuộc tính Code: mã khóa ngẫu nhiên được tạo bởi Generatekey
        public string Code { get; set; }
        // Thuộc tính Ex: thông báo lỗi nếu gửi email thất bại
        public string Ex { get; set; }

        // Constructor khởi tạo đối tượng ResponseEmail
        public ResponseEmail(bool status, string code, string ex)
        {
            this.Status = status;
            this.Code = code;
            this.Ex = ex;
        }
    }
}