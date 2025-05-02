// Khai báo sử dụng namespace BELibrary.Entity, nơi chứa các lớp thực thể (entity) như Account
using System;
using BELibrary.Entity;

namespace BELibrary.Core.Entity.Repositories
{
    // Định nghĩa một giao diện (interface) IAccountRepository, kế thừa từ giao diện generic IRepository<Account>
    // IRepository<Account> có thể chứa các phương thức chung để thao tác với thực thể Account (ví dụ: CRUD operations)
    public interface IAccountRepository : IRepository<Account>
    {
        // Phương thức ValidBEAccount dùng để xác thực tài khoản cho Backend (BE)
        // Nhận vào username và password dạng chuỗi
        // Trả về đối tượng Account nếu thông tin đăng nhập hợp lệ, hoặc null nếu không hợp lệ
        Account ValidBEAccount(string username, string password);

        // Phương thức ValidFeAccount dùng để xác thực tài khoản cho Frontend (FE)
        // Tương tự ValidBEAccount, nhưng có thể áp dụng logic xác thực khác dành riêng cho Frontend
        // Nhận vào username và password, trả về đối tượng Account nếu hợp lệ
        Account ValidFeAccount(string username, string password);

        // Phương thức GetAccountByUsername dùng để lấy thông tin tài khoản dựa trên email (hoặc username)
        // Nhận vào email dạng chuỗi, trả về đối tượng Account tương ứng hoặc null nếu không tìm thấy
        Account GetAccountByUsername(string email);

        // Phương thức GetAccountFeByUsername dùng để lấy thông tin tài khoản cho Frontend dựa trên email
        // Tương tự GetAccountByUsername, nhưng có thể áp dụng logic riêng cho Frontend
        // Nhận vào email, trả về đối tượng Account hoặc null nếu không tìm thấy
        Account GetAccountFeByUsername(string email);
        void Put(Patient account, Guid id);
    }
}