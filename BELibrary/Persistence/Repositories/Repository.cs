using BELibrary.Core.Entity.Repositories; // Import namespace chứa interface IRepository.
using BELibrary.DbContext; // Import namespace chứa lớp HospitalManagementDbContext (DbContext của ứng dụng).
using System; // Import namespace cho các kiểu dữ liệu cơ bản như EventArgs.
using System.Collections.Generic; // Import namespace cho các interface collection như IEnumerable.
using System.Data.Entity; // Import namespace cho các lớp liên quan đến Entity Framework như DbSet.
using System.Linq; // Import namespace cho các phương thức LINQ mở rộng.
using System.Linq.Expressions; // Import namespace cho các lớp biểu thức (Expression).
using Z.EntityFramework.Plus; // Import namespace của thư viện Z.EntityFramework.Plus (có thể cung cấp các extension methods cho EF).

namespace BELibrary.Persistence.Repositories // Namespace định nghĩa nơi chứa các lớp repository để truy cập dữ liệu.
{
    /// <summary>
    /// Lớp Repository<TEntity> là một lớp generic, cung cấp các phương thức truy vấn dữ liệu cơ bản (CRUD - Create, Read, Update, Delete)
    /// cho bất kỳ entity nào trong ứng dụng. Nó triển khai interface IRepository<TEntity>.
    /// where TEntity : class, new() ràng buộc TEntity phải là một lớp tham chiếu và có một constructor không tham số.
    /// </summary>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// Trường protected readonly Context chứa instance của HospitalManagementDbContext.
        /// readonly đảm bảo rằng context chỉ được gán một lần trong constructor.
        /// protected cho phép các lớp kế thừa có thể truy cập vào context này.
        /// </summary>
        protected readonly HospitalManagementDbContext Context;

        /// <summary>
        /// Hàm khởi tạo của lớp Repository.
        /// Nhận một instance của HospitalManagementDbContext thông qua dependency injection.
        /// Gán instance này cho trường Context.
        /// </summary>
        /// <param name="context">DbContext của ứng dụng HospitalManagementDbContext.</param>
        public Repository(HospitalManagementDbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Lấy một entity theo ID.
        /// Sử dụng phương thức Find của DbSet để tìm kiếm entity trong database dựa trên giá trị ID.
        /// </summary>
        /// <param name="id">Giá trị ID của entity cần lấy.</param>
        /// <returns>Entity có ID tương ứng, hoặc null nếu không tìm thấy.</returns>
        public TEntity Get(object id)
        {
            return Context.Set<TEntity>().Find(id);
        }

        /// <summary>
        /// Lấy tất cả các entities thuộc loại TEntity.
        /// Sử dụng phương thức ToList của DbSet để truy vấn tất cả các bản ghi từ bảng tương ứng và trả về một danh sách.
        /// </summary>
        /// <returns>Một IEnumerable chứa tất cả các entities.</returns>
        public IEnumerable<TEntity> GetAll()
        {
            var entities = Context.Set<TEntity>().ToList();
            return entities;
        }

        /// <summary>
        /// Lấy các entities thỏa mãn một điều kiện cụ thể.
        /// Sử dụng phương thức Where của LINQ kết hợp với một biểu thức lambda (predicate) để lọc các entities.
        /// </summary>
        /// <param name="predicate">Biểu thức lambda định nghĩa điều kiện lọc.</param>
        /// <returns>Một IEnumerable chứa các entities thỏa mãn điều kiện.</returns>
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        /// <summary>
        /// Lấy một entity duy nhất thỏa mãn một điều kiện, hoặc giá trị mặc định nếu không tìm thấy hoặc tìm thấy nhiều hơn một.
        /// Sử dụng phương thức SingleOrDefault của LINQ kết hợp với một biểu thức lambda (predicate).
        /// </summary>
        /// <param name="predicate">Biểu thức lambda định nghĩa điều kiện tìm kiếm.</param>
        /// <returns>Entity duy nhất thỏa mãn điều kiện, hoặc null nếu không tìm thấy.</returns>
        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().SingleOrDefault(predicate);
        }

        /// <summary>
        /// Thêm một entity mới vào context.
        /// Sử dụng phương thức Add của DbSet để đánh dấu entity là sẽ được thêm vào database khi SaveChanges được gọi.
        /// </summary>
        /// <param name="entity">Entity cần thêm.</param>
        public void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// Thêm một tập hợp các entities mới vào context.
        /// Sử dụng phương thức AddRange của DbSet để đánh dấu nhiều entities là sẽ được thêm vào database khi SaveChanges được gọi.
        /// </summary>
        /// <param name="entities">Một IEnumerable chứa các entities cần thêm.</param>
        public void AddRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().AddRange(entities);
        }

        /// <summary>
        /// Cập nhật một entity đã tồn tại.
        /// Tìm kiếm entity theo ID, sau đó cập nhật các giá trị thuộc tính của entity tìm thấy bằng các giá trị từ entity được truyền vào.
        /// </summary>
        /// <param name="entity">Entity chứa các giá trị cập nhật.</param>
        /// <param name="id">ID của entity cần cập nhật.</param>
        public void Put(TEntity entity, object id)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }
            var exist = Context.Set<TEntity>().Find(id);
            if (exist != null)
            {
                Context.Entry(exist).CurrentValues.SetValues(entity);
            }
        }

        /// <summary>
        /// Xóa một entity khỏi context.
        /// Sử dụng phương thức Remove của DbSet để đánh dấu entity là sẽ bị xóa khỏi database khi SaveChanges được gọi.
        /// </summary>
        /// <param name="entity">Entity cần xóa.</param>
        public void Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        /// Xóa một tập hợp các entities khỏi context.
        /// Sử dụng phương thức RemoveRange của DbSet để đánh dấu nhiều entities là sẽ bị xóa khỏi database khi SaveChanges được gọi.
        /// </summary>
        /// <param name="entities">Một IEnumerable chứa các entities cần xóa.</param>
        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
        }

        /// <summary>
        /// Lấy các entities và bao gồm các navigation properties được chỉ định.
        /// Sử dụng eager loading để tải các related entities cùng với entity chính.
        /// </summary>
        /// <param name="includes">Một mảng các biểu thức lambda chỉ định các navigation properties cần include.</param>
        /// <returns>Một IEnumerable chứa các entities với các navigation properties đã được tải.</returns>
        public IEnumerable<TEntity> Include(params Expression<Func<TEntity, object>>[] includes)
        {
            IDbSet<TEntity> dbSet = Context.Set<TEntity>();

            IEnumerable<TEntity> query = null;
            foreach (var include in includes)
            {
                query = dbSet.Include(include);
            }

            return query ?? dbSet;
        }

        /// <summary>
        /// Lấy các entities và bao gồm các navigation properties được chỉ định, sử dụng IncludeFilter (từ Z.EntityFramework.Plus).
        /// IncludeFilter có thể cung cấp khả năng lọc dữ liệu của các navigation properties.
        /// </summary>
        /// <param name="includes">Một mảng các biểu thức lambda chỉ định các navigation properties cần include và có thể có bộ lọc.</param>
        /// <returns>Một IEnumerable chứa các entities với các navigation properties đã được tải và có thể đã được lọc.</returns>
        public IEnumerable<TEntity> IncludeFilter(params Expression<Func<TEntity, object>>[] includes)
        {
            IDbSet<TEntity> dbSet = Context.Set<TEntity>();

            IEnumerable<TEntity> query = null;
            foreach (var include in includes)
            {
                query = dbSet.IncludeFilter(include);
            }

            return query ?? dbSet;
        }

        /// <summary>
        /// Truy vấn các entities dựa trên một bộ lọc. Tương tự như Find nhưng có thể trả về IQueryable để thực hiện các thao tác khác.
        /// </summary>
        /// <param name="filter">Biểu thức lambda định nghĩa điều kiện lọc.</param>
        /// <returns>Một IEnumerable chứa các entities thỏa mãn bộ lọc.</returns>
        public IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> filter)
        {
            return Context.Set<TEntity>().Where(filter);
        }

        /// <summary>
        /// Lấy entity đầu tiên thỏa mãn một điều kiện, hoặc giá trị mặc định nếu không tìm thấy.
        /// Sử dụng phương thức FirstOrDefault của LINQ kết hợp với một biểu thức lambda (filter).
        /// </summary>
        /// <param name="filter">Biểu thức lambda định nghĩa điều kiện tìm kiếm.</param>
        /// <returns>Entity đầu tiên thỏa mãn điều kiện, hoặc null nếu không tìm thấy.</returns>
        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> filter)
        {
            return Context.Set<TEntity>().FirstOrDefault(filter);
        }

        /// <summary>
        /// Thực thi một stored procedure trong database và trả về một danh sách các entities.
        /// Sử dụng phương thức SqlQuery của Database để gọi stored procedure và map kết quả vào kiểu TEntity.
        /// </summary>
        /// <param name="query">Tên của stored procedure và các tham số (nếu có) theo cú pháp SQL.</param>
        /// <param name="parameters">Mảng các tham số truyền vào stored procedure.</param>
        /// <returns>Một IEnumerable chứa các entities là kết quả của stored procedure.</returns>
        public IEnumerable<TEntity> ExecWithStoreProcedure(string query, params object[] parameters)
        {
            return Context.Database.SqlQuery<TEntity>(query, parameters);
        }

        /// <summary>
        /// Xóa một entity theo ID và lưu thay đổi vào database.
        /// Tìm kiếm entity theo ID, nếu tìm thấy thì xóa entity đó khỏi context và gọi SaveChanges để cập nhật database.
        /// Tham số userId có vẻ như không được sử dụng trong logic hiện tại.
        /// </summary>
        /// <param name="id">ID của entity cần xóa.</param>
        /// <param name="userId">ID của người dùng thực hiện thao tác xóa (hiện tại không được sử dụng).</param>
        public void Del(object id, Guid userId)
        {
            var entity = Context.Set<TEntity>().Find(id);
            if (entity == null)
            {
                throw new ArgumentNullException();
            }
            Context.Set<TEntity>().Remove(entity);
            Context.SaveChanges();
        }
    }
}