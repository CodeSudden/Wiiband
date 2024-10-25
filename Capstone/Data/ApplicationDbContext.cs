using Microsoft.EntityFrameworkCore;
using System;

namespace Capstone.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for your entities
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<WiibandRegistration> WiibandRegistrations { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Customer entity configuration
            modelBuilder.Entity<Customer>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Customer>()
                .Property(c => c.TotalAmount)
                .HasColumnType("decimal(18, 2)"); // Specify the precision and scale for decimal

            // Staff entity configuration
            modelBuilder.Entity<Staff>()
                .HasKey(s => s.StaffId);

            // WiibandRegistration entity configuration
            modelBuilder.Entity<WiibandRegistration>()
                .HasKey(w => w.Id);

            modelBuilder.Entity<WiibandRegistration>()
                .HasOne(w => w.Staff)
                .WithMany()
                .HasForeignKey(w => w.StaffId);

            modelBuilder.Entity<WiibandRegistration>()
                .Property(w => w.TotalAmount)
                .HasColumnType("decimal(18, 2)"); // Specify the precision and scale for decimal

            // Transaction entity configuration
            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.TransactionId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.WiibandRegistration)
                .WithMany()
                .HasForeignKey(t => t.WiibandRegistrationId); // Assuming you have this property in Transaction

            modelBuilder.Entity<Transaction>()
                .Property(t => t.TotalAmount)
                .HasColumnType("decimal(18, 2)"); // Specify the precision and scale for decimal
        }
    }

    public class Customer
    {
        public int Id { get; set; }  // Primary key
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int NumberOfJumpers { get; set; }
        public bool DiscountPWD { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransactionNumber { get; set; }  // Transaction number field
        public DateTime CreatedAt { get; set; } = DateTime.Now; // To track when the customer was created
    }

    public class Staff
    {
        public int StaffId { get; set; }  // Primary key
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class WiibandRegistration
    {
        public int Id { get; set; }  // Primary key
        public int StaffId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int NumberOfJumpers { get; set; }
        public string SelectedPromo { get; set; }
        public bool DiscountPWD { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime RegistrationDate { get; set; }

        public Staff Staff { get; set; }  // Navigation property
    }

    public class Transaction
    {
        public int TransactionId { get; set; }  // Primary key
        public string TransactionNumber { get; set; }
        public string Wiiband { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string RemainingTime { get; set; }
        public string Status { get; set; }
        public string Battery { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public WiibandRegistration WiibandRegistration { get; set; }  // Navigation property
        public int WiibandRegistrationId { get; set; }  // Foreign key
    }
}
