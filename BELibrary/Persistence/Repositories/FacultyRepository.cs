// Khai báo namespace BELibrary.Persistence.Repositories, nõi ch?a các l?p repository cho t?ng d? li?u
namespace BELibrary.Persistence.Repositories
{
    // S? d?ng các namespace c?n thi?t
    using BELibrary.Core.Entity.Repositories; // Ch?a interface IFacultyRepository
    using BELibrary.DbContext; // Ch?a HospitalManagementDbContext
    using BELibrary.Entity; // Ch?a th?c th? Faculty

    // L?p FacultyRepository tri?n khai IFacultyRepository ð? qu?n l? d? li?u khoa/ph?ng ban
    public class FacultyRepository : Repository<Faculty>, IFacultyRepository
    {
        // Constructor nh?n HospitalManagementDbContext và truy?n vào l?p base
        public FacultyRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        // Thu?c tính tr? v? HospitalManagementDbContext t? Context c?a l?p base
        public HospitalManagementDbContext HospitalManagementDbContext => Context;
    }
}