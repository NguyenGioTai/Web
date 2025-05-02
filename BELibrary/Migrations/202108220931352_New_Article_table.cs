// Khai báo namespace BELibrary.Migrations, nơi chứa các lớp migration cho Entity Framework
namespace BELibrary.Migrations
{
    // Sử dụng System.Data.Entity.Migrations để hỗ trợ migration cơ sở dữ liệu
    using System.Data.Entity.Migrations;

    // Lớp migration New_Article_table, định nghĩa việc thêm bảng Article vào cơ sở dữ liệu
    public partial class New_Article_table : DbMigration
    {
        // Phương thức Up thực hiện các thay đổi để tạo bảng Article
        public override void Up()
        {
            // Tạo bảng Article trong schema dbo
            CreateTable(
                "dbo.Article",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính, kiểu Guid, không cho phép null
                    Title = c.String(), // Tiêu đề bài viết, kiểu chuỗi, cho phép null
                    Image = c.String(), // Liên kết hình ảnh, kiểu chuỗi, cho phép null
                    Description = c.String(), // Mô tả bài viết, kiểu chuỗi, cho phép null
                    Content = c.String(), // Nội dung bài viết, kiểu chuỗi, cho phép null
                    IsDelete = c.Boolean(nullable: false), // Trạng thái xóa mềm, mặc định là false
                    CreatedDate = c.DateTime(nullable: false), // Ngày tạo, không cho phép null
                    CreatedBy = c.String(), // Người tạo, kiểu chuỗi, cho phép null
                    ModifiedDate = c.DateTime(nullable: false), // Ngày sửa, không cho phép null
                    ModifiedBy = c.String(), // Người sửa, kiểu chuỗi, cho phép null
                })
                .PrimaryKey(t => t.Id); // Đặt Id làm khóa chính
        }

        // Phương thức Down thực hiện các thay đổi để hoàn tác (rollback) migration
        public override void Down()
        {
            // Xóa bảng Article
            DropTable("dbo.Article");
        }
    }
}