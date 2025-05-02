// Khai báo namespace BELibrary.Persistence.Repositories, nõi ch?a các l?p repository cho t?ng d? li?u
namespace BELibrary.Persistence.Repositories
{
    // S? d?ng các namespace c?n thi?t
    using BELibrary.Core.Entity.Repositories; // Ch?a interface IDoctorRepository
    using BELibrary.DbContext; // Ch?a HospitalManagementDbContext
    using BELibrary.Entity; // Ch?a th?c th? Doctor

    // L?p DoctorRepository tri?n khai IDoctorRepository ð? qu?n l? d? li?u bác s?
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        // Constructor nh?n HospitalManagementDbContext và truy?n vào l?p base
        public DoctorRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        // Thu?c tính tr? v? HospitalManagementDbContext t? Context c?a l?p base
        public HospitalManagementDbContext HospitalManagementDbContext => Context;
    }
}