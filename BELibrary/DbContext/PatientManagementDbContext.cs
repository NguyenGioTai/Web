// Khai báo namespace BELibrary.DbContext, nõi ch?a các l?p liên quan ð?n ng? c?nh cõ s? d? li?u
namespace BELibrary.DbContext
{
    // S? d?ng namespace BELibrary.Entity ð? tham chi?u các th?c th? (entities) nhý Account, Patient, v.v.
    using BELibrary.Entity;
    // S? d?ng System.Data.Entity ð? làm vi?c v?i Entity Framework
    using System.Data.Entity;

    // L?p HospitalManagementDbContext k? th?a t? DbContext, ð?i di?n cho ng? c?nh cõ s? d? li?u
    public partial class HospitalManagementDbContext : DbContext
    {
        // Constructor m?c ð?nh, kh?i t?o ng? c?nh cõ s? d? li?u
        public HospitalManagementDbContext()
        {
            // T?t Lazy Loading ð? ngãn Entity Framework t? ð?ng n?p các th?c th? liên quan
            // Ði?u này giúp ki?m soát vi?c truy v?n d? li?u r? ràng hõn, tránh n?p dý th?a
            this.Configuration.LazyLoadingEnabled = false;
        }

        // Các thu?c tính DbSet ð?i di?n cho các b?ng trong cõ s? d? li?u
        // M?i DbSet ánh x? t?i m?t b?ng và cho phép th?c hi?n các thao tác CRUD
        public virtual DbSet<Account> Accounts { get; set; } // B?ng lýu thông tin tài kho?n ngý?i dùng
        public virtual DbSet<Category> Categories { get; set; } // B?ng lýu danh m?c (ví d?: danh m?c thi?t b? y t?)
        public virtual DbSet<DetailPrescription> DetailPrescriptions { get; set; } // B?ng lýu chi ti?t ðõn thu?c
        public virtual DbSet<DetailRecord> DetailRecords { get; set; } // B?ng lýu chi ti?t h? sõ b?nh án
        public virtual DbSet<Item> Items { get; set; } // B?ng lýu các m?c (có th? là thi?t b? ho?c v?t tý)
        public virtual DbSet<MedicalSupply> MedicalSupplies { get; set; } // B?ng lýu thông tin v?t tý y t?
        public virtual DbSet<Medicine> Medicines { get; set; } // B?ng lýu thông tin thu?c
        public virtual DbSet<Patient> Patients { get; set; } // B?ng lýu thông tin b?nh nhân
        public virtual DbSet<Prescription> Prescriptions { get; set; } // B?ng lýu thông tin ðõn thu?c
        public virtual DbSet<Record> Records { get; set; } // B?ng lýu h? sõ b?nh án
        public virtual DbSet<Attachment> Attachments { get; set; } // B?ng lýu t?p ðính kèm
        public virtual DbSet<AttachmentAssign> AttachmentAssigns { get; set; } // B?ng lýu thông tin gán t?p ðính kèm
        public virtual DbSet<Faculty> Faculties { get; set; } // B?ng lýu thông tin khoa/ph?ng ban
        public virtual DbSet<Doctor> Doctors { get; set; } // B?ng lýu thông tin bác s?
        public virtual DbSet<PatientRecord> PatientRecords { get; set; } // B?ng lýu quan h? gi?a b?nh nhân và h? sõ
        public virtual DbSet<UserVerification> UserVerifications { get; set; } // B?ng lýu thông tin xác minh ngý?i dùng
        public virtual DbSet<DoctorSchedule> DoctorSchedules { get; set; } // B?ng lýu l?ch làm vi?c c?a bác s?
        public virtual DbSet<PatientDoctor> PatientDoctors { get; set; } // B?ng lýu quan h? gi?a b?nh nhân và bác s?
        public virtual DbSet<Article> Articles { get; set; } // B?ng lýu bài vi?t (có th? là tin t?c y khoa)

        // Phýõng th?c OnModelCreating c?u h?nh ánh x? gi?a các th?c th? và cõ s? d? li?u
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // C?u h?nh quan h? 1-nhi?u gi?a Category và Item
            // M?t Category có nhi?u Item, m?i Item ph?i thu?c m?t Category
            modelBuilder.Entity<Category>()
                .HasMany(e => e.Items) // Category có t?p h?p Items
                .WithRequired(e => e.Category) // Item yêu c?u m?t Category
                .WillCascadeOnDelete(false); // Không xóa Item khi xóa Category

            // C?u h?nh quan h? 1-nhi?u gi?a Medicine và DetailPrescription
            // M?t Medicine có nhi?u DetailPrescription, m?i DetailPrescription ph?i thu?c m?t Medicine
            modelBuilder.Entity<Medicine>()
                .HasMany(e => e.DetailPrescriptions)
                .WithRequired(e => e.Medicine)
                .WillCascadeOnDelete(false); // Không xóa DetailPrescription khi xóa Medicine

            // C?u h?nh thu?c tính IndentificationCardId c?a Patient không h? tr? Unicode
            // Thý?ng dùng cho m? ð?nh danh nhý CMND/CCCD, không c?n k? t? Unicode
            modelBuilder.Entity<Patient>()
                .Property(e => e.IndentificationCardId)
                .IsUnicode(false);

            // C?u h?nh thu?c tính Phone c?a Patient không h? tr? Unicode
            // Ð?m b?o s? ði?n tho?i ch? ch?a k? t? ASCII (0-9, +, v.v.)
            modelBuilder.Entity<Patient>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            // C?u h?nh quan h? 1-nhi?u gi?a Patient và MedicalSupply
            // M?t Patient có nhi?u MedicalSupply, m?i MedicalSupply ph?i thu?c m?t Patient
            modelBuilder.Entity<Patient>()
                .HasMany(e => e.MedicalSupplies)
                .WithRequired(e => e.Patient)
                .WillCascadeOnDelete(false); // Không xóa MedicalSupply khi xóa Patient

            // C?u h?nh quan h? 1-nhi?u gi?a Record và DetailRecord
            // M?t Record có nhi?u DetailRecord, m?i DetailRecord ph?i thu?c m?t Record
            modelBuilder.Entity<Record>()
                .HasMany(e => e.DetailRecords)
                .WithRequired(e => e.Record)
                .WillCascadeOnDelete(false); // Không xworkbench xóa DetailRecord khi xóa Record
        }
    }
}