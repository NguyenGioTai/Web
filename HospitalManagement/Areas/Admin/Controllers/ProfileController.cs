using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using BELibrary.Utils;
using HospitalManagement.Areas.Admin.Authorization;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    // Chỉ những tài khoản có vai trò Doctor mới có thể truy cập controller này
    [Permission(Role = RoleKey.Doctor)]
    public class ProfileController : BaseController
    {
        // Tên phần tử được hiển thị trên giao diện
        private string _keyElement = "Tài khoản";

        // Trang hiển thị hồ sơ người dùng hiện tại
        public ActionResult Index()
        {
            ViewBag.Feature = "Hồ sơ";
            ViewBag.Element = _keyElement;

            // Lấy thông tin người dùng hiện tại
            var user = GetCurrentUser();

            // Nếu chưa đăng nhập, chuyển hướng về trang admin login
            if (user == null)
                return Redirect("/admin");

            // Nếu không phải Doctor thì chỉ hiển thị thông tin cơ bản
            if (user.Role != RoleKey.Doctor)
                return View(user);

            // Nếu là Doctor, load thêm thông tin bác sĩ và khoa (Faculty)
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                ViewBag.Doctor = workScope.Doctors.Include(x => x.Faculty)
                    .FirstOrDefault(x => x.Id == user.DoctorId);
            }

            return View(user);
        }

        // Trang cập nhật hồ sơ
        public ActionResult Edit()
        {
            ViewBag.Feature = "Cập nhật";
            ViewBag.Element = _keyElement;

            var user = GetCurrentUser();

            // Nếu chưa đăng nhập, chuyển hướng về trang admin
            if (user == null)
                return Redirect("/admin");

            // Gán danh sách giới tính cho dropdown
            ViewBag.Genders = new SelectList(GenderKey.GetDic(), "Value", "Text");

            // Nếu không phải bác sĩ, chỉ cập nhật thông tin tài khoản
            if (user.Role != RoleKey.Doctor)
                return View(user);

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                // Lấy thông tin bác sĩ, nếu không có thì tạo mới đối tượng Doctor rỗng
                var doctor = workScope.Doctors.Include(x => x.Faculty)
                    .FirstOrDefault(x => x.Id == user.DoctorId) ?? new Doctor();
                ViewBag.Doctor = doctor;

                // Lấy danh sách khoa để hiển thị dropdown chọn khoa
                var faculties = workScope.Faculties.GetAll().ToList();
                ViewBag.Faculties = new SelectList(faculties, "Id", "Name", selectedValue: doctor.FacultyId);
            }

            return View(user);
        }

        // Cập nhật thông tin tài khoản (bao gồm đổi mật khẩu, khoa nếu là bác sĩ)
        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateInfo(Account input, Guid? facultyId, string rePassword)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var account = GetCurrentUser();

                    if (account != null) // nếu tài khoản tồn tại
                    {
                        // Xử lý nếu người dùng có nhập mật khẩu mới
                        if (!string.IsNullOrEmpty(input.Password) || rePassword != "")
                        {
                            // Kiểm tra xem đã đăng nhập chưa (thường dùng cho các trang bảo vệ)
                            if (!CookiesManage.Logined())
                            {
                                return Json(new { status = false, mess = "Chưa đăng nhập" });
                            }

                            // So sánh mật khẩu và xác nhận lại mật khẩu
                            if (input.Password != rePassword)
                            {
                                return Json(new { status = false, mess = "Mật khẩu không khớp" });
                            }

                            // Mã hóa mật khẩu trước khi lưu
                            var passwordFactory = input.Password + VariableExtensions.KeyCrypto;
                            var passwordCryptor = CryptorEngine.Encrypt(passwordFactory, true);
                            input.Password = passwordCryptor;
                        }
                        else
                        {
                            // Nếu không thay đổi mật khẩu thì giữ nguyên mật khẩu cũ
                            input.Password = account.Password;
                        }

                        // Cập nhật các thuộc tính không được thay đổi bằng thông tin hiện tại
                        input.Id = account.Id;
                        input.UserName = account.UserName;
                        input.Role = account.Role;
                        input.PatientId = account.PatientId;
                        input.DoctorId = account.DoctorId;

                        // Ghi đè thông tin tài khoản cũ bằng thông tin mới
                        account = input;
                        workScope.Accounts.Put(account, account.Id);

                        // Nếu người dùng là bác sĩ và có chọn khoa
                        if (account.Role == RoleKey.Doctor && facultyId.HasValue)
                        {
                            var doctor = workScope.Doctors.FirstOrDefault(x => x.Id == account.DoctorId);

                            // Nếu bác sĩ tồn tại và khoa tồn tại thì cập nhật FacultyId
                            if (doctor != null && workScope.Faculties.GetAll().Any(x => x.Id == facultyId))
                            {
                                doctor.FacultyId = facultyId.GetValueOrDefault();
                                workScope.Doctors.Put(doctor, doctor.Id);
                            }
                        }

                        // Lưu thay đổi vào DB
                        workScope.Complete();

                        return Json(new { status = true, mess = "Cập nhập thành công " });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " + _keyElement });
                    }
                }
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, trả về thông báo lỗi
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}
