using BELibrary.Core.Entity.Repositories; // Import namespace chứa interface IUserVerificationRepository.
using BELibrary.DbContext; // Import namespace chứa lớp HospitalManagementDbContext (DbContext của ứng dụng).
using BELibrary.Entity; // Import namespace chứa lớp UserVerification (Entity đại diện cho thông tin xác thực người dùng, ví dụ: mã OTP, token...).

namespace BELibrary.Persistence.Repositories // Namespace định nghĩa nơi chứa các lớp repository để truy cập dữ liệu.
{
    /// <summary>
    /// Lớp UserVerificationRepository chịu trách nhiệm thực hiện các thao tác truy vấn dữ liệu liên quan đến đối tượng UserVerification.
    /// Kế thừa từ lớp Repository<UserVerification> để tận dụng các phương thức truy vấn dữ liệu cơ bản (CRUD).
    /// Đồng thời triển khai interface IUserVerificationRepository để đảm bảo lớp này cung cấp các phương thức đặc thù (nếu có) cho UserVerification.
    /// </summary>
    public class UserVerificationRepository : Repository<UserVerification>, IUserVerificationRepository
    {
        /// <summary>
        /// Hàm khởi tạo của lớp UserVerificationRepository.
        /// Nhận một instance của HospitalManagementDbContext thông qua dependency injection.
        /// Gọi constructor của lớp cha (Repository<UserVerification>) và truyền context này vào.
        /// </summary>
        /// <param name="context">DbContext của ứng dụng HospitalManagementDbContext.</param>
        public UserVerificationRepository(HospitalManagementDbContext context)
            : base(context)
        {
            // Không có logic khởi tạo đặc biệt nào cho lớp này ngoài việc gọi constructor của lớp cha.
        }

        /// <summary>
        /// Property (expression-bodied member) để truy cập vào instance của HospitalManagementDbContext với kiểu cụ thể.
        /// Tương tự như cách khai báo property ở các lớp repository khác,
        /// nhưng sử dụng cú pháp ngắn gọn hơn cho property chỉ có getter.
        /// Điều này giúp tránh việc phải cast Context mỗi khi muốn sử dụng các thuộc tính hoặc phương thức đặc thù của HospitalManagementDbContext.
        /// </summary>
        public HospitalManagementDbContext HospitalManagementDbContext => Context as HospitalManagementDbContext;
    }
}