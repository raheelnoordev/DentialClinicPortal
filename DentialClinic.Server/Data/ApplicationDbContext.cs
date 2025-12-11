using Microsoft.EntityFrameworkCore;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Company;
using DentialClinic.Server.Models.Banking;
using DentialClinic.Server.Models.UserManagement;
using DentialClinic.Server.Models.Lookup;
using DentialClinic.Server.Models.Cashbook;
using DentialClinic.Server.Models.MoneyAccount;
using DentialClinic.Server.Models.Branch;
using DentialClinic.Server.Models.Patient;
using DentialClinic.Server.Models.Doctor;
using DentialClinic.Server.Models.Treatment;
using DentialClinic.Server.Models.Appointment;
using DentialClinic.Server.Models.Visit;
using DentialClinic.Server.Models.Billing;
using DentialClinic.Server.Models.Expense;
using DentialClinic.Server.Models.WhatsApp;

namespace DentialClinic.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Core User Management
        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Menus> Menus { get; set; }
        public DbSet<RoleMenus> RoleMenus { get; set; }
        //public DbSet<VisitDto> VisitDtos { get; set; }

        // Dental Clinic Core
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<VDoctorDetails> VDoctorDetails { get; set; }
        public DbSet<VPatientAppointmentsDetails> VPatientAppointmentsDetails { get; set; }
        public DbSet<TreatmentCatalog> TreatmentCatalogs { get; set; }
        
        // Appointments & Visits
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<VisitTreatment> VisitTreatments { get; set; }
        
        // Billing
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoicePayment> InvoicePayments { get; set; }
        
        // Expenses
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<SalaryPayment> SalaryPayments { get; set; }
        
        // WhatsApp
        public DbSet<WhatsAppScreen> WhatsAppScreens { get; set; }
        public DbSet<WhatsAppScreenOption> WhatsAppScreenOptions { get; set; }
        public DbSet<WhatsAppConversationState> WhatsAppConversationStates { get; set; }
        public DbSet<WhatsAppBooking> WhatsAppBookings { get; set; }
        public DbSet<WhatsAppSettings> WhatsAppSettings { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<LoginLog> LoginLogs { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Lookup> Lookups { get; set; }
        public DbSet<Cashbook> Cashbooks { get; set; }
        public DbSet<MoneyAccount> MoneyAccounts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.BranchId).HasColumnName("branch_id");
                entity.Property(e => e.FullName).HasColumnName("full_name").IsRequired().HasMaxLength(150);
                entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(150);
                entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(50);
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
                entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
                entity.Property(e => e.RoleId).HasColumnName("role_id").IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                
                entity.HasMany(e => e.UserRoles)
                      .WithOne(ur => ur.User)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.ToTable("roles");
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.RoleName).HasColumnName("role_name").IsRequired().HasMaxLength(50);
                
                entity.HasIndex(e => e.RoleName).IsUnique();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("user_roles");
                entity.HasKey(e => e.UserRoleId);
                entity.Property(e => e.UserRoleId).HasColumnName("user_role_id");
                entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
                entity.Property(e => e.RoleId).HasColumnName("role_id").IsRequired();

                entity.HasOne(e => e.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
            });

            modelBuilder.Entity<Menus>(entity =>
            {
                entity.ToTable("menus");
                entity.HasKey(e => e.MenuId);
                entity.Property(e => e.MenuId).HasColumnName("menu_id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Url).HasColumnName("url").HasMaxLength(150);
                entity.Property(e => e.Icon).HasColumnName("icon").HasMaxLength(100);
                entity.Property(e => e.SortOrder).HasColumnName("sort_order").IsRequired();
                entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();
            });

            modelBuilder.Entity<RoleMenus>(entity =>
            {
                entity.ToTable("role_menus");
                entity.HasKey(e => e.RoleMenuId);
                entity.Property(e => e.RoleMenuId).HasColumnName("role_menu_id");
                entity.Property(e => e.RoleId).HasColumnName("role_id").IsRequired();
                entity.Property(e => e.MenuId).HasColumnName("menu_id").IsRequired();

                entity.HasOne(e => e.Role)
                      .WithMany(r => r.RoleMenus)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Menu)
                      .WithMany(m => m.RoleMenus)
                      .HasForeignKey(e => e.MenuId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasIndex(e => new { e.RoleId, e.MenuId }).IsUnique();
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.Property(b => b.CreatedAt)
                      .HasColumnName("created_at")
                      .HasColumnType("timestamp with time zone");
                
            });

            modelBuilder.Entity<LoginLog>(entity =>
            {
                entity.HasKey(ll => ll.LogID);

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(ll => ll.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.ToTable("bank_accounts");
                entity.HasKey(e => e.BankAccountId);
                entity.Property(e => e.BranchCode).HasMaxLength(100);
                entity.Property(e => e.BranchAddress).HasMaxLength(100);
                entity.Property(e => e.AccountName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AccountIbanNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.BankName).HasMaxLength(100);
                entity.Property(e => e.AccountType).HasMaxLength(50);
                entity.Property(e => e.OpeningBalance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<VLoginLog>(entity =>
            {
                entity.ToView("VLoginLog");
                entity.HasNoKey();
            });

            modelBuilder.Entity<VDoctorDetails>(entity =>
            {
                entity.ToView("v_doctor_details");
                entity.HasNoKey();
            });

            modelBuilder.Entity<VPatientAppointmentsDetails>(entity =>
            {
                entity.ToView("v_patient_appointments_details");
                entity.HasNoKey();
            });
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("appointments");

                entity.HasKey(e => e.AppointmentId)
                    .HasName("appointments_pkey");

                entity.Property(e => e.AppointmentId)
                    .HasColumnName("appointment_id");

                entity.Property(e => e.PatientId)
                    .HasColumnName("patient_id")
                    .IsRequired();

                entity.Property(e => e.DoctorId)
                    .HasColumnName("doctor_id")
                    .IsRequired();

                entity.Property(e => e.BranchId)
                    .HasColumnName("branch_id")
                    .IsRequired();

                entity.Property(e => e.AppointmentAt)
                    .HasColumnName("appointment_at")
                    .HasColumnType("timestamp without time zone")
                    .IsRequired();

                entity.Property(e => e.DurationMin)
                    .HasColumnName("duration_min")
                    .IsRequired()
                    .HasDefaultValue(10);

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasMaxLength(20)
                    .IsRequired()
                    .HasDefaultValueSql("'booked'::character varying");

                entity.Property(e => e.Source)
                    .HasColumnName("source")
                    .HasMaxLength(20)
                    .IsRequired()
                    .HasDefaultValueSql("'call'::character varying");

                entity.Property(e => e.Reason)
                    .HasColumnName("reason");

                entity.Property(e => e.CreatedByUserId)
                    .HasColumnName("created_by_user_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp without time zone")
                    .IsRequired()
                    .HasDefaultValueSql("now()");
            });

            // Lookups
            modelBuilder.Entity<Lookup>(entity =>
            {
                entity.ToTable("lookups");
                entity.HasKey(e => e.LookupId);
                entity.Property(e => e.LookupId).HasColumnName("lookup_id");
                entity.Property(e => e.LookupName).HasColumnName("lookup_name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.LookupDomain).HasColumnName("lookup_domain").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Enabled).HasColumnName("enabled").IsRequired();
                entity.Property(e => e.SortOrder).HasColumnName("sort_order").IsRequired();
                entity.Property(e => e.CreatedOn).HasColumnName("created_on").IsRequired();
                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.ShowInDispatch).HasColumnName("show_in_dispatch");
            });

          

            modelBuilder.Entity<MoneyAccount>(entity =>
            {
                entity.ToTable("money_accounts");
                entity.HasKey(e => e.MoneyAccountId);
                entity.Property(e => e.MoneyAccountId).HasColumnName("money_account_id");
                entity.Property(e => e.AccountCode).HasColumnName("account_code").HasMaxLength(100);
                entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
                entity.Property(e => e.KindLookupId).HasColumnName("kind_lookup_id").IsRequired();
                entity.Property(e => e.AccountHolder).HasColumnName("account_holder").HasMaxLength(200);
                entity.Property(e => e.CompanyRegNo).HasColumnName("company_reg_no").HasMaxLength(100);
                entity.Property(e => e.BankName).HasColumnName("bank_name").HasMaxLength(200);
                entity.Property(e => e.BranchName).HasColumnName("branch_name").HasMaxLength(200);
                entity.Property(e => e.BranchCode).HasColumnName("branch_code").HasMaxLength(100);
                entity.Property(e => e.AccountNumber).HasColumnName("account_number").HasMaxLength(100);
                entity.Property(e => e.Iban).HasColumnName("iban").HasMaxLength(100);
                entity.Property(e => e.SwiftBic).HasColumnName("swift_bic").HasMaxLength(100);
                entity.Property(e => e.WalletProvider).HasColumnName("wallet_provider").HasMaxLength(200);
                entity.Property(e => e.WalletPhone).HasColumnName("wallet_phone").HasMaxLength(50);
                entity.Property(e => e.Currency).HasColumnName("currency").IsRequired().HasMaxLength(10);
                entity.Property(e => e.OpeningBalance).HasColumnName("opening_balance").IsRequired().HasColumnType("numeric(18,2)");
                entity.Property(e => e.OpeningBalanceAsOf).HasColumnName("opening_balance_as_of");
                entity.Property(e => e.IsDefault).HasColumnName("is_default").IsRequired();
                entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();
                entity.Property(e => e.Notes).HasColumnName("notes");
                entity.Property(e => e.Meta).HasColumnName("meta").HasColumnType("jsonb");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();
                entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);

                // Foreign key relationship with lookups
                entity.HasOne(e => e.KindLookup)
                      .WithMany()
                      .HasForeignKey(e => e.KindLookupId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Cashbook
            modelBuilder.Entity<Cashbook>(entity =>
            {
                entity.ToTable("cashbook");
                entity.HasKey(e => e.CashId);
                entity.Property(e => e.CashId).HasColumnName("cash_id");
                entity.Property(e => e.HappenedAt).HasColumnName("happened_at").IsRequired();
                entity.Property(e => e.CashKindId).HasColumnName("cash_kind_id").IsRequired();
                entity.Property(e => e.Amount).HasColumnName("amount").IsRequired().HasColumnType("numeric(18,2)");
                entity.Property(e => e.Currency).HasColumnName("currency").IsRequired().HasMaxLength(10);
                entity.Property(e => e.MoneyAccountId).HasColumnName("money_account_id");
                entity.Property(e => e.WalletEmployeeId).HasColumnName("wallet_employee_id");
                entity.Property(e => e.CategoryId).HasColumnName("category_id").IsRequired();
                entity.Property(e => e.CostCenterId).HasColumnName("cost_center_id");
                entity.Property(e => e.PaymentModeId).HasColumnName("payment_mode_id");
                entity.Property(e => e.ReferenceNo).HasColumnName("reference_no").HasMaxLength(100);
                entity.Property(e => e.CounterpartyName).HasColumnName("counterparty_name").HasMaxLength(200);
                entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);
                entity.Property(e => e.Meta).HasColumnName("meta").HasColumnType("jsonb");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20);
                entity.Property(e => e.ReceiptPath).HasColumnName("receipt_path").HasMaxLength(500);
            });
        }
    }
}