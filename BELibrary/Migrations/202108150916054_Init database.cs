// Khai báo namespace BELibrary.Migrations, nơi chứa các lớp migration cho Entity Framework
namespace BELibrary.Migrations
{
    // Sử dụng System.Data.Entity.Migrations để hỗ trợ migration cơ sở dữ liệu
    using System.Data.Entity.Migrations;

    // Lớp migration Initdatabase, định nghĩa cấu trúc cơ sở dữ liệu ban đầu
    public partial class Initdatabase : DbMigration
    {
        // Phương thức Up thực hiện các thay đổi để tạo cấu trúc cơ sở dữ liệu
        public override void Up()
        {
            // Tạo bảng Account để lưu thông tin tài khoản người dùng
            CreateTable(
                "dbo.Account",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính, kiểu Guid, không cho phép null
                    FullName = c.String(maxLength: 50), // Họ tên, tối đa 50 ký tự
                    Phone = c.String(maxLength: 15), // Số điện thoại, tối đa 15 ký tự
                    UserName = c.String(maxLength: 50), // Tên đăng nhập, tối đa 50 ký tự
                    LinkAvatar = c.String(maxLength: 250), // Liên kết ảnh đại diện, tối đa 250 ký tự
                    Gender = c.Boolean(nullable: false), // Giới tính (true: Nam, false: Nữ)
                    Password = c.String(maxLength: 250), // Mật khẩu, tối đa 250 ký tự
                    Role = c.Int(nullable: false), // Vai trò (Admin, Bác sĩ, Bệnh nhân)
                    IsDeleted = c.Boolean(nullable: false), // Trạng thái xóa mềm
                    PatientId = c.Guid(), // Khóa ngoại tham chiếu đến bảng Patient
                    DoctorId = c.Guid(), // Khóa ngoại tham chiếu đến bảng Doctor
                })
                .PrimaryKey(t => t.Id) // Đặt Id làm khóa chính
                .ForeignKey("dbo.Doctor", t => t.DoctorId) // Khóa ngoại đến bảng Doctor
                .ForeignKey("dbo.Patient", t => t.PatientId) // Khóa ngoại đến bảng Patient
                .Index(t => t.PatientId) // Tạo chỉ mục cho PatientId
                .Index(t => t.DoctorId); // Tạo chỉ mục cho DoctorId

            // Tạo bảng Doctor để lưu thông tin bác sĩ
            CreateTable(
                "dbo.Doctor",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính, kiểu Guid
                    Name = c.String(), // Tên bác sĩ
                    Avatar = c.String(), // Liên kết ảnh đại diện
                    Descriptions = c.String(), // Mô tả bác sĩ
                    Address = c.String(), // Địa chỉ
                    Gender = c.Boolean(nullable: false), // Giới tính
                    Phone = c.String(), // Số điện thoại
                    Email = c.String(), // Email
                    IsDelete = c.Boolean(nullable: false), // Trạng thái xóa mềm
                    FacultyId = c.Guid(nullable: false), // Khóa ngoại tham chiếu đến bảng Faculty
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Faculty", t => t.FacultyId, cascadeDelete: true) // Khóa ngoại, xóa liên đới
                .Index(t => t.FacultyId); // Tạo chỉ mục cho FacultyId

            // Tạo bảng Faculty để lưu thông tin khoa/phòng ban
            CreateTable(
                "dbo.Faculty",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính, kiểu Guid
                    Name = c.String(nullable: false), // Tên khoa, không cho phép null
                })
                .PrimaryKey(t => t.Id);

            // Tạo bảng Patient để lưu thông tin bệnh nhân
            CreateTable(
                "dbo.Patient",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính, kiểu Guid
                    FullName = c.String(nullable: false, maxLength: 100), // Họ tên, tối đa 100 ký tự
                    DateOfBirth = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"), // Ngày sinh
                    Address = c.String(nullable: false, maxLength: 250), // Địa chỉ, tối đa 250 ký tự
                    Gender = c.Boolean(nullable: false), // Giới tính
                    IndentificationCardId = c.String(maxLength: 20, unicode: false), // CMND/CCCD, tối đa 20 ký tự
                    Phone = c.String(nullable: false, maxLength: 15, unicode: false), // Số điện thoại
                    Status = c.Boolean(nullable: false), // Trạng thái (có thể là hoạt động/không hoạt động)
                    ImageProfile = c.String(), // Liên kết ảnh hồ sơ
                    PatientCode = c.String(nullable: false), // Mã bệnh nhân
                    JoinDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"), // Ngày tham gia
                    IndentificationCardDate = c.DateTime(nullable: false), // Ngày cấp CMND/CCCD
                    Job = c.String(), // Nghề nghiệp
                    WorkPlace = c.String(), // Nơi làm việc
                    HistoryOfIllnessFamily = c.String(), // Tiền sử bệnh gia đình
                    HistoryOfIllnessYourself = c.String(), // Tiền sử bệnh cá nhân
                    IsDeleted = c.Boolean(nullable: false), // Trạng thái xóa mềm
                    Email = c.String(maxLength: 250), // Email
                })
                .PrimaryKey(t => t.Id);

            // Tạo bảng MedicalSupplies để lưu thông tin vật tư y tế
            CreateTable(
                "dbo.MedicalSupplies",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính
                    DateOfHire = c.DateTime(nullable: false), // Ngày mượn
                    Status = c.Int(nullable: false), // Trạng thái (Đã mượn, Khả dụng, v.v.)
                    Amount = c.Int(nullable: false), // Số lượng
                    ItemId = c.Guid(nullable: false), // Khóa ngoại đến bảng Item
                    PatientId = c.Guid(nullable: false), // Khóa ngoại đến bảng Patient
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Item", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("dbo.Patient", t => t.PatientId)
                .Index(t => t.ItemId)
                .Index(t => t.PatientId);

            // Tạo bảng Item để lưu thông tin mục (thiết bị/vật tư)
            CreateTable(
                "dbo.Item",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính
                    Name = c.String(nullable: false, maxLength: 250), // Tên mục
                    Amount = c.Int(nullable: false), // Số lượng
                    Description = c.String(), // Mô tả
                    CategoryId = c.Guid(nullable: false), // Khóa ngoại đến bảng Category
                    CreatedDate = c.DateTime(nullable: false), // Ngày tạo
                    CreatedBy = c.String(), // Người tạo
                    ModifiedDate = c.DateTime(nullable: false), // Ngày sửa
                    ModifiedBy = c.String(), // Người sửa
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.CategoryId)
                .Index(t => t.CategoryId);

            // Tạo bảng Category để lưu thông tin danh mục
            CreateTable(
                "dbo.Category",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính
                    Name = c.String(nullable: false, maxLength: 200), // Tên danh mục
                    Unit = c.String(nullable: false, maxLength: 50), // Đơn vị
                    Description = c.String(), // Mô tả
                })
                .PrimaryKey(t => t.Id);

            // Tạo bảng AttachmentAssigns để lưu quan hệ giữa tệp đính kèm và chi tiết hồ sơ
            CreateTable(
                "dbo.AttachmentAssigns",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính
                    AttachmentId = c.Guid(nullable: false), // Khóa ngoại đến bảng Attachments
                    DetailRecordId = c.Guid(nullable: false), // Khóa ngoại đến bảng DetailRecord
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.AttachmentId, cascadeDelete: true)
                .ForeignKey("dbo.DetailRecord", t => t.DetailRecordId, cascadeDelete: true)
                .Index(t => t.AttachmentId)
                .Index(t => t.DetailRecordId);

            // Tạo bảng Attachments để lưu thông tin tệp đính kèm
            CreateTable(
                "dbo.Attachments",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính
                    Name = c.String(), // Tên tệp
                    Type = c.String(), // Loại tệp
                    Url = c.String(), // Liên kết tệp
                    CreatedDate = c.DateTime(nullable: false), // Ngày tạo
                    CreatedBy = c.String(), // Người tạo
                    ModifiedDate = c.DateTime(nullable: false), // Ngày sửa
                    ModifiedBy = c.String(), // Người sửa
                })
                .PrimaryKey(t => t.Id);

            // Tạo bảng DetailRecord để lưu chi tiết hồ sơ bệnh án
            CreateTable(
                "dbo.DetailRecord",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính
                    DiseaseName = c.String(nullable: false, maxLength: 200), // Tên bệnh
                    Note = c.String(), // Ghi chú
                    Result = c.String(), // Kết quả
                    Status = c.Boolean(nullable: false), // Trạng thái
                    DoctorId = c.Guid(), // Khóa ngoại đến bảng Doctor
                    FacultyId = c.Guid(), // Khóa ngoại đến bảng Faculty
                    Process = c.Int(nullable: false), // Tiến trình điều trị
                    RecordId = c.Guid(nullable: false), // Khóa ngoại đến bảng Record
                    IsMainRecord = c.Boolean(nullable: false), // Là hồ sơ chính?
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Faculty", t => t.FacultyId)
                .ForeignKey("dbo.Record", t => t.RecordId)
                .Index(t => t.FacultyId)
                .Index(t => t.RecordId);

            // Tạo bảng Record để lưu hồ sơ bệnh án
            CreateTable(
                "dbo.Record",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính
                    CreatedDate = c.DateTime(nullable: false), // Ngày tạo
                    CreatedBy = c.String(), // Người tạo
                    ModifiedDate = c.DateTime(nullable: false), // Ngày sửa
                    ModifiedBy = c.String(), // Người sửa
                    DoctorId = c.Guid(), // Khóa ngoại đến bảng Doctor
                    Note = c.String(), // Ghi chú
                    Result = c.String(), // Kết quả
                    StatusRecord = c.Int(nullable: false), // Trạng thái hồ sơ (Nội trú/Ngoại trú)
                    IsDelete = c.Boolean(nullable: false), // Trạng thái xóa mềm
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctor", t => t.DoctorId)
                .Index(t => t.DoctorId);

            // Tạo bảng DetailPrescription để lưu chi tiết đơn thuốc
            CreateTable(
                "dbo.DetailPrescription",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính
                    Amount = c.Int(nullable: false), // Số lượng
                    Unit = c.String(nullable: false, maxLength: 50), // Đơn vị
                    Note = c.String(nullable: false), // Ghi chú
                    MedicineId = c.Guid(nullable: false), // Khóa ngoại đến bảng Medicine
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Medicine", t => t.MedicineId)
                .Index(t => t.MedicineId);

            // Tạo bảng Medicine để lưu thông tin thuốc
            CreateTable(
                "dbo.Medicine",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính
                    Name = c.String(nullable: false, maxLength: 200), // Tên thuốc
                    Description = c.String(), // Mô tả
                })
                .PrimaryKey(t => t.Id);

            // Tạo bảng DoctorSchedule để lưu lịch làm việc của bác sĩ
            CreateTable(
                "dbo.DoctorSchedule",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true), // Khóa chính, tự động tăng
                    DoctorId = c.Guid(nullable: false), // Khóa ngoại đến bảng Doctor
                    PatientId = c.Guid(nullable: false), // Khóa ngoại đến bảng Patient
                    ScheduleBook = c.DateTime(nullable: false), // Thời gian đặt lịch
                    Status = c.Int(nullable: false), // Trạng thái lịch (Active, Pending, Reject)
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .ForeignKey("dbo.Patient", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.DoctorId)
                .Index(t => t.PatientId);

            // Tạo bảng PatientDoctor để lưu quan hệ giữa bệnh nhân và bác sĩ
            CreateTable(
                "dbo.PatientDoctor",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true), // Khóa chính, tự động tăng
                    DoctorId = c.Guid(nullable: false), // Khóa ngoại đến bảng Doctor
                    PatientId = c.Guid(nullable: false), // Khóa ngoại đến bảng Patient
                    Status = c.Int(nullable: false), // Trạng thái quan hệ
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .ForeignKey("dbo.Patient", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.DoctorId)
                .Index(t => t.PatientId);

            // Tạo bảng PatientRecord để lưu thông tin khám bệnh của bệnh nhân
            CreateTable(
                "dbo.PatientRecord",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính
                    Title = c.String(), // Tiêu đề
                    TestDate = c.DateTime(nullable: false), // Ngày kiểm tra
                    BloodVessel = c.Double(nullable: false), // Huyết áp
                    BodyTemperature = c.Double(nullable: false), // Nhiệt độ cơ thể
                    Height = c.Double(nullable: false), // Chiều cao
                    Weight = c.Double(nullable: false), // Cân nặng
                    Breathing = c.Double(nullable: false), // Nhịp thở
                    VisionWithoutGlassesRight = c.String(), // Thị lực mắt phải không kính
                    VisionWithoutGlassesLeft = c.String(), // Thị lực mắt trái không kính
                    VisionWithGlassHoleLeft = c.String(), // Thị lực mắt trái qua kính lỗ
                    VisionWithGlassHoleRight = c.String(), // Thị lực mắt trái qua kính lỗ
                    VisionWithGlassLeft = c.String(), // Thị lực mắt trái có kính
                    VisionWithGlassRight = c.String(), // Thị lực mắt phải có kính
                    EyePressureRight = c.String(), // Nhãn áp mắt phải
                    EyePressureLeft = c.String(), // Nhãn áp mắt trái
                    ClinicalSymptoms = c.String(), // Triệu chứng lâm sàng
                    DiagnosingTwoEyes = c.String(), // Chẩn đoán cả hai mắt
                    DiagnosingLeftEyes = c.String(), // Chẩn đoán mắt trái
                    DiagnosingRightEyes = c.String(), // Chẩn đoán mắt phải
                    Status = c.Boolean(nullable: false), // Trạng thái
                    IsDelete = c.Boolean(nullable: false), // Trạng thái xóa mềm
                    DoctorId = c.Guid(), // Khóa ngoại đến bảng Doctor
                    RecordId = c.Guid(nullable: false), // Khóa ngoại đến bảng Record
                    PatientId = c.Guid(nullable: false), // Khóa ngoại đến bảng Patient
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctor", t => t.DoctorId)
                .ForeignKey("dbo.Patient", t => t.PatientId, cascadeDelete: true)
                .ForeignKey("dbo.Record", t => t.RecordId, cascadeDelete: true)
                .Index(t => t.DoctorId)
                .Index(t => t.RecordId)
                .Index(t => t.PatientId);

            // Tạo bảng Prescription để lưu thông tin đơn thuốc
            CreateTable(
                "dbo.Prescription",
                c => new
                {
                    Id = c.Guid(nullable: false), // Khóa chính
                    DetailPrescriptionId = c.Guid(nullable: false), // Khóa ngoại đến bảng DetailPrescription
                    DetailRecordId = c.Guid(nullable: false), // Khóa ngoại đến bảng DetailRecord
                    CreatedDate = c.DateTime(nullable: false), // Ngày tạo
                    CreatedBy = c.String(), // Người tạo
                    ModifiedDate = c.DateTime(nullable: false), // Ngày sửa
                    ModifiedBy = c.String(), // Người sửa
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DetailPrescription", t => t.DetailPrescriptionId, cascadeDelete: true)
                .ForeignKey("dbo.DetailRecord", t => t.DetailRecordId, cascadeDelete: true)
                .Index(t => t.DetailPrescriptionId)
                .Index(t => t.DetailRecordId);

            // Tạo bảng UserVerification để lưu thông tin xác minh người dùng
            CreateTable(
                "dbo.UserVerification",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true), // Khóa chính, tự động tăng
                    Mode = c.String(), // Chế độ xác minh
                    Token = c.String(), // Mã token
                    TokenExpirationDate = c.DateTime(), // Ngày hết hạn token
                    VerificationCode = c.String(), // Mã xác minh
                    CodeExpirationDate = c.DateTime(), // Ngày hết hạn mã xác minh
                    CreatedDate = c.DateTime(nullable: false), // Ngày tạo
                    Status = c.Int(), // Trạng thái
                    Link = c.Int(nullable: false), // Liên kết (chưa rõ mục đích)
                    AccountId = c.Int(), // Khóa ngoại đến bảng Account (nhưng kiểu Int, có thể lỗi)
                    Email = c.String(), // Email
                    DeletedDate = c.DateTime(nullable: false), // Ngày xóa
                })
                .PrimaryKey(t => t.Id);
        }

        // Phương thức Down thực hiện các thay đổi để hoàn tác (rollback) migration
        public override void Down()
        {
            // Xóa các khóa ngoại
            DropForeignKey("dbo.Prescription", "DetailRecordId", "dbo.DetailRecord");
            DropForeignKey("dbo.Prescription", "DetailPrescriptionId", "dbo.DetailPrescription");
            DropForeignKey("dbo.PatientRecord", "RecordId", "dbo.Record");
            DropForeignKey("dbo.PatientRecord", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.PatientRecord", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.PatientDoctor", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.PatientDoctor", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DoctorSchedule", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.DoctorSchedule", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DetailPrescription", "MedicineId", "dbo.Medicine");
            DropForeignKey("dbo.AttachmentAssigns", "DetailRecordId", "dbo.DetailRecord");
            DropForeignKey("dbo.Record", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DetailRecord", "RecordId", "dbo.Record");
            DropForeignKey("dbo.DetailRecord", "FacultyId", "dbo.Faculty");
            DropForeignKey("dbo.AttachmentAssigns", "AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.MedicalSupplies", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.MedicalSupplies", "ItemId", "dbo.Item");
            DropForeignKey("dbo.Item", "CategoryId", "dbo.Category");
            DropForeignKey("dbo.Account", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.Account", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.Doctor", "FacultyId", "dbo.Faculty");

            // Xóa các chỉ mục
            DropIndex("dbo.Prescription", new[] { "DetailRecordId" });
            DropIndex("dbo.Prescription", new[] { "DetailPrescriptionId" });
            DropIndex("dbo.PatientRecord", new[] { "PatientId" });
            DropIndex("dbo.PatientRecord", new[] { "RecordId" });
            DropIndex("dbo.PatientRecord", new[] { "DoctorId" });
            DropIndex("dbo.PatientDoctor", new[] { "PatientId" });
            DropIndex("dbo.PatientDoctor", new[] { "DoctorId" });
            DropIndex("dbo.DoctorSchedule", new[] { "PatientId" });
            DropIndex("dbo.DoctorSchedule", new[] { "DoctorId" });
            DropIndex("dbo.DetailPrescription", new[] { "MedicineId" });
            DropIndex("dbo.Record", new[] { "DoctorId" });
            DropIndex("dbo.DetailRecord", new[] { "RecordId" });
            DropIndex("dbo.DetailRecord", new[] { "FacultyId" });
            DropIndex("dbo.AttachmentAssigns", new[] { "DetailRecordId" });
            DropIndex("dbo.AttachmentAssigns", new[] { "AttachmentId" });
            DropIndex("dbo.Item", new[] { "CategoryId" });
            DropIndex("dbo.MedicalSupplies", new[] { "PatientId" });
            DropIndex("dbo.MedicalSupplies", new[] { "ItemId" });
            DropIndex("dbo.Doctor", new[] { "FacultyId" });
            DropIndex("dbo.Account", new[] { "DoctorId" });
            DropIndex("dbo.Account", new[] { "PatientId" });

            // Xóa các bảng
            DropTable("dbo.UserVerification");
            DropTable("dbo.Prescription");
            DropTable("dbo.PatientRecord");
            DropTable("dbo.PatientDoctor");
            DropTable("dbo.DoctorSchedule");
            DropTable("dbo.Medicine");
            DropTable("dbo.DetailPrescription");
            DropTable("dbo.Record");
            DropTable("dbo.DetailRecord");
            DropTable("dbo.Attachments");
            DropTable("dbo.AttachmentAssigns");
            DropTable("dbo.Category");
            DropTable("dbo.Item");
            DropTable("dbo.MedicalSupplies");
            DropTable("dbo.Patient");
            DropTable("dbo.Faculty");
            DropTable("dbo.Doctor");
            DropTable("dbo.Account");
        }
    }
}