// Khai báo namespace BELibrary.Persistence.Repositories, nõi ch?a các l?p repository cho t?ng d? li?u
namespace BELibrary.Persistence.Repositories
{
    // S? d?ng các namespace c?n thi?t
    using BELibrary.Core.Entity.Repositories; // Ch?a interface IArticleRepository
    using BELibrary.DbContext; // Ch?a HospitalManagementDbContext
    using BELibrary.Entity; // Ch?a th?c th? Article

    // L?p ArticleRepository tri?n khai IArticleRepository ð? qu?n l? d? li?u bài vi?t
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        // Constructor nh?n HospitalManagementDbContext và truy?n vào l?p base
        public ArticleRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        // Thu?c tính tr? v? HospitalManagementDbContext t? Context c?a l?p base
        public HospitalManagementDbContext HospitalManagementDbContext => Context;
    }
}