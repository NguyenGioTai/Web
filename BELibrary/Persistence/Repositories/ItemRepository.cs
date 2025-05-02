// Khai báo namespace BELibrary.Persistence.Repositories, nõi ch?a các l?p repository cho t?ng d? li?u
namespace BELibrary.Persistence.Repositories
{
    // S? d?ng các namespace c?n thi?t
    using BELibrary.Core.Entity.Repositories; // Ch?a interface IItemRepository
    using BELibrary.DbContext; // Ch?a HospitalManagementDbContext
    using BELibrary.Entity; // Ch?a th?c th? Item

    // L?p ItemRepository tri?n khai IItemRepository ð? qu?n l? d? li?u m?c (thi?t b?/v?t tý y t?)
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        // Constructor nh?n HospitalManagementDbContext và truy?n vào l?p base
        public ItemRepository(HospitalManagementDbContext context)
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