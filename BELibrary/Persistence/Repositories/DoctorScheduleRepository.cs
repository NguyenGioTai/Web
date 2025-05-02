// Khai báo namespace BELibrary.Persistence.Repositories, nơi chứa các lớp repository cho tầng dữ liệu
namespace BELibrary.Persistence.Repositories
{
    // Sử dụng các namespace cần thiết
    using BELibrary.Core.Entity.Repositories; // Chứa interface IDoctorScheduleRepository
    using BELibrary.DbContext; // Chứa HospitalManagementDbContext
    using BELibrary.Entity; // Chứa thực thể DoctorSchedule

    // Lớp DoctorScheduleRepository triển khai IDoctorScheduleRepository để quản lý dữ liệu lịch làm việc của bác sĩ
    public class DoctorScheduleRepository : Repository<DoctorSchedule>, IDoctorScheduleRepository
    {
        // Constructor nhận HospitalManagementDbContext và truyền vào lớp base
        public DoctorScheduleRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        // Thuộc tính trả về HospitalManagementDbContext từ Context của lớp base
        public HospitalManagementDbContext HospitalManagementDbContext => Context as HospitalManagementDbContext;
    }
}