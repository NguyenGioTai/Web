// Khai báo namespace BELibrary.Persistence.Repositories, nơi chứa các lớp repository cho tầng dữ liệu
namespace BELibrary.Persistence.Repositories
{
    // Sử dụng các namespace cần thiết
    using BELibrary.Core.Entity.Repositories; // Chứa interface IAccountRepository
    using BELibrary.Core.Utils; // Chứa các hằng số như VariableExtensions, RoleKey
    using BELibrary.DbContext; // Chứa HospitalManagementDbContext
    using BELibrary.Entity; // Chứa thực thể Account
    using BELibrary.Utils; // Chứa CryptorEngine để mã hóa mật khẩu
    using System;
    using System.Linq; // Hỗ trợ truy vấn LINQ

    // Lớp AccountRepository triển khai IAccountRepository để quản lý dữ liệu tài khoản
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        // Constructor nhận HospitalManagementDbContext và truyền vào lớp base
        public AccountRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        // Xác thực tài khoản cho backend (Admin hoặc Doctor)
        public Account ValidBEAccount(string username, string password)
        {
            // Lấy DbContext từ thuộc tính HospitalManagementDbContext
            var db = HospitalManagementDbContext;

            // Tạo chuỗi mật khẩu bằng cách nối password với KeyCrypto
            var passwordFactory = password + VariableExtensions.KeyCrypto;
            // Mã hóa mật khẩu sử dụng CryptorEngine
            var passwordCrypto = CryptorEngine.Encrypt(passwordFactory, true);

            // Tìm tài khoản phù hợp với vai trò Admin hoặc Doctor, username không phân biệt hoa thường
            var account = db.Accounts.FirstOrDefault(x =>
                (x.Role == RoleKey.Admin || x.Role == RoleKey.Doctor)
                && x.UserName.ToLower() == username.ToLower()
                && x.Password == passwordCrypto);

            return account; // Trả về tài khoản hoặc null nếu không tìm thấy
        }

        // Lấy tài khoản backend (Admin hoặc Doctor) theo username
        public Account GetAccountByUsername(string username)
        {
            // Lấy DbContext
            var db = HospitalManagementDbContext;

            // Tìm tài khoản phù hợp với vai trò Admin hoặc Doctor, username không phân biệt hoa thường
            var account = db.Accounts.FirstOrDefault(x =>
                (x.Role == RoleKey.Admin || x.Role == RoleKey.Doctor)
                && x.UserName.ToLower() == username.ToLower());

            return account; // Trả về tài khoản hoặc null nếu không tìm thấy
        }

        // Lấy tài khoản frontend (Patient) theo username
        public Account GetAccountFeByUsername(string username)
        {
            // Lấy DbContext
            var db = HospitalManagementDbContext;

            // Tìm tài khoản phù hợp với vai trò Patient, username không phân biệt hoa thường
            var account = db.Accounts.FirstOrDefault(x =>
                x.Role == RoleKey.Patient
                && x.UserName.ToLower() == username.ToLower());

            return account; // Trả về tài khoản hoặc null nếu không tìm thấy
        }

        // Xác thực tài khoản cho frontend (Patient)
        public Account ValidFeAccount(string username, string password)
        {
            // Lấy DbContext
            var db = HospitalManagementDbContext;

            // Tạo chuỗi mật khẩu bằng cách nối password với KeyCryptorClient
            var passwordFactory = password + VariableExtensions.KeyCryptorClient;
            // Mã hóa mật khẩu sử dụng CryptorEngine
            var passwordCrypto = CryptorEngine.Encrypt(passwordFactory, true);

            // Tìm tài khoản phù hợp với vai trò Patient, username không phân biệt hoa thường
            var account = db.Accounts.FirstOrDefault(x =>
                x.Role == RoleKey.Patient
                && x.UserName.ToLower() == username.ToLower()
                && x.Password == passwordCrypto);

            return account; // Trả về tài khoản hoặc null nếu không tìm thấy
        }

        public void Put(Patient account, Guid id)
        {
            throw new NotImplementedException();
        }

        // Thuộc tính trả về HospitalManagementDbContext từ Context của lớp base
        public HospitalManagementDbContext HospitalManagementDbContext => Context;
    }
}