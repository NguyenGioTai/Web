using BELibrary.Core.Entity.Repositories; // Import namespace chứa interface IPatientDoctorRepository.
using BELibrary.DbContext; // Import namespace chứa lớp HospitalManagementDbContext (DbContext của ứng dụng).
using BELibrary.Entity; // Import namespace chứa lớp PatientDoctor (Entity đại diện cho mối quan hệ giữa bệnh nhân và bác sĩ).

namespace BELibrary.Persistence.Repositories // Namespace định nghĩa nơi chứa các lớp repository để truy cập dữ liệu.
{
    /// <summary>
    /// Lớp PatientDoctorRepository chịu trách nhiệm thực hiện các thao tác truy vấn dữ liệu liên quan đến đối tượng PatientDoctor.
    /// Kế thừa từ lớp Repository<PatientDoctor> để tận dụng các phương thức truy vấn dữ liệu cơ bản (CRUD).
    /// Đồng thời triển khai interface IPatientDoctorRepository để đảm bảo lớp này cung cấp các phương thức đặc thù (nếu có) cho PatientDoctor.
    /// </summary>
    public class PatientDoctorRepository : Repository<PatientDoctor>, IPatientDoctorRepository
    {
        /// <summary>
        /// Hàm khởi tạo của lớp PatientDoctorRepository.
        /// Nhận một instance của HospitalManagementDbContext thông qua dependency injection.
        /// Gọi constructor của lớp cha (Repository<PatientDoctor>) và truyền context này vào.
        /// </summary>
        /// <param name="context">DbContext của ứng dụng HospitalManagementDbContext.</param>
        public PatientDoctorRepository(HospitalManagementDbContext context)
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