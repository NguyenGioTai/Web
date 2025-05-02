using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// Khai báo namespace BELibrary.Core.Utils, nơi chứa các tiện ích chung và các hằng số, cấu hình
namespace BELibrary.Core.Utils
{
    // Lớp tĩnh Common định nghĩa các hằng số tiền tố (prefix) dùng trong hệ thống
    public static class Common
    {
        // Tiền tố dùng cho các mã định danh (ví dụ: mã bệnh nhân, mã đặt lịch)
        public const string Prefix = "BN-";
        // Tiền tố dùng cho các mã hồ sơ (record), hiện tại giống với Prefix
        public const string PrefixRecord = "BN-";
    }

    // Lớp tĩnh FileKey định nghĩa các cấu hình liên quan đến tệp (file)
    public static class FileKey
    {
        // Dung lượng tối đa của tệp (1000 KB = 1 MB)
        public const int MaxLength = 1024 * 1000;

        // Phương thức trả về danh sách các định dạng tệp hình ảnh được chấp nhận
        public static List<string> FileExtensionApprove()
        {
            // Trả về danh sách các phần mở rộng tệp: png, jpg, jpeg
            return new List<string>(new[] { "png", "jpg", "jpeg" });
        }

        // Phương thức trả về danh sách các loại nội dung (content type) được chấp nhận
        public static List<string> FileContentTypeApprove()
        {
            // Trả về danh sách các MIME type cho PDF và Excel
            return new List<string>(new[]
            {
                "application/pdf", // Tệp PDF
                "application/vnd.ms-excel", // Tệp Excel (.xls)
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" // Tệp Excel (.xlsx)
            });
        }
    }

    // Lớp tĩnh CookiesKey định nghĩa các tên khóa (key) dùng cho cookie
    public static class CookiesKey
    {
        // Khóa cookie lưu mã ngôn ngữ của bệnh nhân
        public const string LangCode = "patient_lang_code";
        // Khóa cookie cho quản trị viên
        public const string Admin = "patient_admin_cookies";
        // Khóa cookie cho bệnh nhân (client)
        public const string Client = "patient_client_cookies";
    }

    // Lớp tĩnh BookingStatusKey định nghĩa các trạng thái đặt lịch và phương thức liên quan
    public static class BookingStatusKey
    {
        // Mã trạng thái: Đã thăm khám
        public const int Active = 100;
        // Mã trạng thái: Đang chờ xử lý
        public const int Pending = 101;
        // Mã trạng thái: Bị từ chối/hủy
        public const int Reject = 102;

        // Phương thức trả về văn bản mô tả trạng thái dựa trên mã
        public static string GetText(int code)
        {
            switch (code)
            {
                case Active:
                    return "Đã thăm khám";
                case Pending:
                    return "Đợi";
                case Reject:
                    return "Hủy";
                default:
                    return "Unknown"; // Trả về nếu mã không xác định
            }
        }
    }

    // Lớp tĩnh LangCode định nghĩa các mã ngôn ngữ và phương thức liên quan
    public static class LangCode
    {
        // Mã ngôn ngữ: Tiếng Anh
        public const string English = "en";
        // Mã ngôn ngữ: Tiếng Việt
        public const string VietNam = "vi";
        // Mã ngôn ngữ: Tiếng Nhật
        public const string Japan = "ja";
        // Mã ngôn ngữ mặc định
        public const string Default = "en";

        // Phương thức trả về tên ngôn ngữ dựa trên mã
        public static string GetText(string code)
        {
            switch (code)
            {
                case "en":
                    return "English";
                case "vi":
                    return "Việt Nam";
                case "ja":
                    return "Japan";
                default:
                    return "Unknown"; // Trả về nếu mã không xác định
            }
        }

        // Phương thức trả về danh sách các mã ngôn ngữ
        public static List<string> GetList()
        {
            return new List<string>(new[] { "en", "vi", "ja" });
        }

        // Phương thức trả về danh sách ngôn ngữ dưới dạng SelectListModel cho giao diện
        public static List<SelectListModel> GetDic()
        {
            return new List<SelectListModel>
            {
                new SelectListModel { Value = "vi", Text = "Tiếng việt" },
                new SelectListModel { Value = "en", Text = "Tiếng anh" },
                new SelectListModel { Value = "ja", Text = "Tiếng nhật" }
            };
        }
    }

    // Lớp tĩnh RoleKey định nghĩa các vai trò và phương thức liên quan
    public static class RoleKey
    {
        // Mã vai trò: Quản trị viên
        public const int Admin = 1;
        // Mã vai trò: Bác sĩ-wise
        public const int Doctor = 2;
        // Mã vai trò: Bệnh nhân
        public const int Patient = 3;

        // Phương thức trả về danh sách các mã vai trò
        public static List<int> GetList()
        {
            return new List<int>(new[] { 1, 2, 3 });
        }

        // Phương thức kiểm tra xem một vai trò có tồn tại trong danh sách hay không
        public static bool Any(int role)
        {
            return GetList().Any(x => x == role);
        }

        // Phương thức trả về danh sách vai trò dưới dạng SelectListModel
        public static List<SelectListModel> GetDic()
        {
            return new List<SelectListModel>
            {
                new SelectListModel { Value = 1, Text = "Admin" },
                new SelectListModel { Value = 2, Text = "Bác sĩ" },
                new SelectListModel { Value = 3, Text = "Bệnh nhân" }
            };
        }

        // Phương thức trả về tên vai trò dựa trên mã
        public static string GetRole(int role)
        {
            switch (role)
            {
                case 1:
                    return "Admin";
                case 2:
                    return "Bác sĩ";
                case 3:
                    return "Bệnh nhân";
                default:
                    return "Unknown"; // Trả về nếu mã không xác định
            }
        }
    }

    // Lớp tĩnh GenderKey định nghĩa các giá trị giới tính và phương thức liên quan
    public static class GenderKey
    {
        // Giới tính: Nam
        public const bool Male = true;
        // Giới tính: Nữ
        public const bool FeMale = false;

        // Phương thức trả về danh sách giới tính dưới dạng SelectListModel
        public static List<SelectListModel> GetDic()
        {
            return new List<SelectListModel>
            {
                new SelectListModel { Value = true, Text = "Nam" },
                new SelectListModel { Value = false, Text = "Nữ" }
            };
        }

        // Phương thức trả về tên giới tính dựa trên mã (0: Nữ, 1: Nam)
        public static string GetEmployeeType(int type)
        {
            switch (type)
            {
                case 0:
                    return "Nữ";
                case 1:
                    return "Nam";
                default:
                    return "Unknown"; // Trả về nếu mã không xác định
            }
        }
    }

    // Lớp tĩnh VariableExtensions định nghĩa các hằng số cấu hình
    public static class VariableExtensions
    {
        // Kích thước trang (số mục mỗi trang) cho phân trang
        public static int PageSize = 2;
        // Khóa mã hóa dùng cho mã hóa dữ liệu
        public static string KeyCrypto = "#!2020";
        // Khóa mã hóa dùng cho phía client
        public static string KeyCryptorClient = "#!2020_Client##";
        // Mật khẩu mặc định
        public static string DefautlPassword = "123qwe"; // Lưu ý: Lỗi đánh máy, nên là "DefaultPassword"
    }

    // Lớp tĩnh StatusMedical định nghĩa các trạng thái thiết bị y tế và phương thức liên quan
    public static class StatusMedical
    {
        // Trạng thái: Đã mượn
        public const int Hired = 1;
        // Trạng thái: Khả dụng
        public const int Availability = 0;
        // Trạng thái: Không khả dụng
        public const int Unavailable = -1;
        // Trạng thái: Bảo trì
        public const int Maintenance = -2;
        // Trạng thái: Hết hạn
        public const int Expired = -3;

        // Phương thức trả về văn bản mô tả trạng thái thiết bị y tế
        public static string GetText(int stt)
        {
            switch (stt)
            {
                case 1:
                    return "Đã mượn";
                case 0:
                    return "Khả dụng";
                case -1:
                    return "Không khả dụng";
                case -2:
                    return "Bảo trì";
                case -3:
                    return "Hết hạn";
                default:
                    return "Unknown";
            }
        }

        // Phương thức trả về danh sách trạng thái thiết bị dưới dạng SelectListModel
        public static List<SelectListModel> GetDic()
        {
            return new List<SelectListModel>
            {
                new SelectListModel { Value = 1, Text = "Đã mượn" },
                new SelectListModel { Value = 0, Text = "Khả dụng" },
                new SelectListModel { Value = -1, Text = "Không khả dụng" },
                new SelectListModel { Value = -2, Text = "Bảo trì" },
                new SelectListModel { Value = -3, Text = "Hết hạn" }
            };
        }
    }

    // Lớp tĩnh StatusRecord định nghĩa các trạng thái hồ sơ điều trị
    public static class StatusRecord
    {
        // Trạng thái: Điều trị nội trú
        public const int InpatientTreatment = 1;
        // Trạng thái: Điều trị ngoại trú
        public const int OutPatientTreatment = 0;

        // Phương thức trả về văn bản mô tả trạng thái hồ sơ
        public static string GetText(int stt)
        {
            switch (stt)
            {
                case 1:
                    return "Điều trị nội trú";
                case 0:
                    return "Điều trị ngoại trú";
                default:
                    return "Unknown";
            }
        }

        // Phương thức trả về danh sách trạng thái hồ sơ dưới dạng SelectListModel
        public static List<SelectListModel> GetDic()
        {
            return new List<SelectListModel>
            {
                new SelectListModel { Value = 1, Text = "Điều trị nội trú" },
                new SelectListModel { Value = 0, Text = "Điều trị ngoại trú" }
            };
        }
    }

    // Enum StatusCode định nghĩa các mã trạng thái HTTP
    public enum StatusCode
    {
        Success = 200, // Thành công
        NotFound = 404, // Không tìm thấy
        NotForbidden = 403, // Bị cấm
        ServerError = 500 // Lỗi máy chủ
    }

    // Lớp tĩnh CountryKey cung cấp danh sách các quốc gia
    public static class CountryKey
    {
        // Phương thức trả về danh sách tất cả các quốc gia dưới dạng SelectListModel
        public static List<SelectListModel> GetAll()
        {
            // Tạo danh sách để lưu các quốc gia
            List<SelectListModel> lst = new List<SelectListModel>();
            // Lấy tất cả các văn hóa (culture) cụ thể
            CultureInfo[] cultureInfos = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            // Duyệt qua từng văn hóa để lấy thông tin quốc gia
            foreach (var item in cultureInfos)
            {
                RegionInfo regionInfo = new RegionInfo(item.LCID);
                // Chỉ thêm quốc gia nếu chưa tồn tại trong danh sách
                if (!(lst.Any(x => x.Text == regionInfo.EnglishName)))
                {
                    lst.Add(new SelectListModel
                    {
                        Text = regionInfo.EnglishName, // Tên quốc gia bằng tiếng Anh
                        Value = regionInfo.EnglishName // Giá trị là tên quốc gia
                    });
                }
            }
            // Sắp xếp danh sách theo tên quốc gia và trả về
            return lst.OrderBy(o => o.Text).ToList();
        }
    }

    // Lớp SelectListModel định nghĩa cấu trúc dữ liệu cho danh sách lựa chọn (dropdown)
    public class SelectListModel
    {
        // Giá trị của mục (có thể là int, bool, string, v.v.)
        public object Value { get; set; }
        // Văn bản hiển thị của mục
        public string Text { get; set; }
    }
}