// Khai báo namespace BELibrary.Persistence.Repositories, nõi ch?a các l?p repository cho t?ng d? li?u
namespace BELibrary.Persistence.Repositories
{
    // S? d?ng các namespace c?n thi?t
    using BELibrary.Core.Entity.Repositories; // Ch?a interface IDetailRecordRepository
    using BELibrary.DbContext; // Ch?a HospitalManagementDbContext
    using BELibrary.Entity; // Ch?a th?c th? DetailRecord

    // L?p DetailRecordRepository tri?n khai IDetailRecordRepository ð? qu?n l? d? li?u chi ti?t h? sõ b?nh án
    public class DetailRecordRepository : Repository<DetailRecord>, IDetailRecordRepository
    {
        // Constructor nh?n HospitalManagementDbContext và truy?n vào l?p base
        public DetailRecordRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        // Thu?c tính tr? v? HospitalManagementDbContext t? Context c?a l?p base
        public HospitalManagementDbContext HospitalManagementDbContext
        {
            get { return Context as HospitalManagementDbContext; }
        }
    }
}