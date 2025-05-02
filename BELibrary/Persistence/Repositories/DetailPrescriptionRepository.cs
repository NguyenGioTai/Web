// Khai báo namespace BELibrary.Persistence.Repositories, nõi ch?a các l?p repository cho t?ng d? li?u
namespace BELibrary.Persistence.Repositories
{
    // S? d?ng các namespace c?n thi?t
    using BELibrary.Core.Entity.Repositories; // Ch?a interface IDetailPrescriptionRepository
    using BELibrary.DbContext; // Ch?a HospitalManagementDbContext
    using BELibrary.Entity; // Ch?a th?c th? DetailPrescription

    // L?p DetailPrescriptionRepository tri?n khai IDetailPrescriptionRepository ð? qu?n l? d? li?u chi ti?t ðõn thu?c
    public class DetailPrescriptionRepository : Repository<DetailPrescription>, IDetailPrescriptionRepository
    {
        // Constructor nh?n HospitalManagementDbContext và truy?n vào l?p base
        public DetailPrescriptionRepository(HospitalManagementDbContext context)
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