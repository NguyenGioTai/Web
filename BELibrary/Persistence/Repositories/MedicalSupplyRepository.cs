using BELibrary.Core.Entity.Repositories; // Import namespace ch?a interface IMedicalSupplyRepository.
using BELibrary.DbContext; // Import namespace ch?a l?p HospitalManagementDbContext (DbContext c?a ?ng d?ng).
using BELibrary.Entity; // Import namespace ch?a l?p MedicalSupply (Entity ð?i di?n cho v?t tý y t?).

namespace BELibrary.Persistence.Repositories // Namespace ð?nh ngh?a nõi ch?a các l?p repository ð? truy c?p d? li?u.
{
    /// <summary>
    /// L?p MedicalSupplyRepository ch?u trách nhi?m th?c hi?n các thao tác truy v?n d? li?u liên quan ð?n ð?i tý?ng MedicalSupply.
    /// K? th?a t? l?p Repository<MedicalSupply> ð? t?n d?ng các phýõng th?c truy v?n d? li?u cõ b?n (CRUD).
    /// Ð?ng th?i tri?n khai interface IMedicalSupplyRepository ð? ð?m b?o l?p này cung c?p các phýõng th?c ð?c thù (n?u có) cho MedicalSupply.
    /// </summary>
    public class MedicalSupplyRepository : Repository<MedicalSupply>, IMedicalSupplyRepository
    {
        /// <summary>
        /// Hàm kh?i t?o c?a l?p MedicalSupplyRepository.
        /// Nh?n m?t instance c?a HospitalManagementDbContext thông qua dependency injection.
        /// G?i constructor c?a l?p cha (Repository<MedicalSupply>) và truy?n context này vào.
        /// </summary>
        /// <param name="context">DbContext c?a ?ng d?ng HospitalManagementDbContext.</param>
        public MedicalSupplyRepository(HospitalManagementDbContext context)
            : base(context)
        {
            // Không có logic kh?i t?o ð?c bi?t nào cho l?p này ngoài vi?c g?i constructor c?a l?p cha.
        }

        /// <summary>
        /// Property ð? truy c?p vào instance c?a HospitalManagementDbContext v?i ki?u c? th?.
        /// Ði?u này giúp tránh vi?c ph?i cast Context m?i khi mu?n s? d?ng các thu?c tính ho?c phýõng th?c ð?c thù c?a HospitalManagementDbContext.
        /// </summary>
        public HospitalManagementDbContext HospitalManagementDbContext
        {
            get { return Context as HospitalManagementDbContext; } // Tr? v? Context sau khi ép ki?u v? HospitalManagementDbContext.
        }
    }
}