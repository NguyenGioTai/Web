using System;
using System.Security.Cryptography;
using System.Text;

// Khai báo namespace BELibrary.Utils, nơi chứa các tiện ích hỗ trợ mã hóa và giải mã
namespace BELibrary.Utils
{
    // Định nghĩa lớp tĩnh CryptorEngine chứa các phương thức để mã hóa và giải mã chuỗi
    public class CryptorEngine
    {
        /// <summary>
        /// Mã hóa một chuỗi sử dụng phương pháp mã hóa TripleDES. Trả về chuỗi mã hóa dạng Base64.
        /// </summary>
        /// <param name="toEncrypt">Chuỗi cần mã hóa</param>
        /// <param name="useHashing">Sử dụng hashing cho khóa? True để tăng cường bảo mật</param>
        /// <returns>Chuỗi mã hóa dạng Base64</returns>
        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            // Chuyển chuỗi đầu vào thành mảng byte sử dụng mã hóa UTF-8
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            // Khóa bí mật được hard-code (cố định) trong mã nguồn
            string key = "KmL!@z7";
            // Lưu ý: Hard-code khóa trong mã nguồn không an toàn, nên lưu trong cấu hình hoặc kho khóa bảo mật
            byte[] keyArray;

            // Nếu useHashing là true, tạo khóa bằng cách băm chuỗi khóa sử dụng MD5
            if (useHashing)
            {
                // Khởi tạo đối tượng MD5CryptoServiceProvider để tạo mã băm MD5
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                // Băm chuỗi khóa thành mảng byte (16 byte)
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                // Giải phóng tài nguyên của đối tượng MD5
                hashmd5.Clear();
            }
            else
            {
                // Nếu không sử dụng hashing, chuyển trực tiếp chuỗi khóa thành mảng byte
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            // Khởi tạo đối tượng TripleDESCryptoServiceProvider để thực hiện mã hóa TripleDES
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            // Gán khóa cho thuật toán TripleDES
            tdes.Key = keyArray;
            // Sử dụng chế độ mã hóa ECB (Electronic Codebook), không sử dụng vector khởi tạo (IV)
            tdes.Mode = CipherMode.ECB;
            // Sử dụng padding PKCS7 để đảm bảo độ dài dữ liệu phù hợp với kích thước khối
            tdes.Padding = PaddingMode.PKCS7;

            // Tạo bộ mã hóa (encryptor) từ TripleDES
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            // Mã hóa mảng byte đầu vào và trả về mảng byte đã mã hóa
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            // Giải phóng tài nguyên của đối tượng TripleDES
            tdes.Clear();

            // Chuyển mảng byte đã mã hóa thành chuỗi Base64 để dễ lưu trữ hoặc truyền tải
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// Giải mã một chuỗi đã mã hóa (dạng Base64) sử dụng phương pháp TripleDES. Trả về chuỗi gốc.
        /// </summary>
        /// <param name="cipherString">Chuỗi mã hóa dạng Base64</param>
        /// <param name="useHashing">Có sử dụng hashing cho khóa khi mã hóa không? True nếu có</param>
        /// <returns>Chuỗi gốc đã được giải mã</returns>
        public static string Decrypt(string cipherString, bool useHashing)
        {
            // Chuyển chuỗi Base64 thành mảng byte
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            // Khóa bí mật được hard-code, giống như trong phương thức Encrypt
            string key = "KmL!@z7";
            byte[] keyArray;

            // Nếu useHashing là true, tạo khóa bằng cách băm chuỗi khóa sử dụng MD5
            if (useHashing)
            {
                // Khởi tạo đối tượng MD5CryptoServiceProvider
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                // Băm chuỗi khóa thành mảng byte
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                // Giải phóng tài nguyên
                hashmd5.Clear();
            }
            else
            {
                // Nếu không sử dụng hashing, chuyển trực tiếp chuỗi khóa thành mảng byte
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            // Khởi tạo đối tượng TripleDESCryptoServiceProvider để giải mã
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            // Gán khóa cho thuật toán
            tdes.Key = keyArray;
            // Sử dụng chế độ ECB, tương tự khi mã hóa
            tdes.Mode = CipherMode.ECB;
            // Sử dụng padding PKCS7
            tdes.Padding = PaddingMode.PKCS7;

            // Tạo bộ giải mã (decryptor) từ TripleDES
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            // Giải mã mảng byte và trả về mảng byte đã giải mã
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            // Giải phóng tài nguyên
            tdes.Clear();

            // Chuyển mảng byte đã giải mã thành chuỗi UTF-8
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}