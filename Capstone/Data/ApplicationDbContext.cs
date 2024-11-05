using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using Capstone.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace Capstone.Data // Replace with your actual namespace if different
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public decimal GetTotalSales()
        {
            return Customers.Sum(c => c.Total_amount);
        }

        public decimal GetTotalSalesForToday()
        {
            var today = DateTime.Today;
            return Customers
                .Where(c => c.Created_at.Date == today)
                .Sum(c => c.Total_amount);
        }

        public decimal GetWeeklySales()
        {
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            return Customers
                .Where(c => c.Created_at >= startOfWeek)
                .Sum(c => c.Total_amount);
        }

        public decimal GetMonthlySales()
        {
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            return Customers
                .Where(c => c.Created_at >= startOfMonth)
                .Sum(c => c.Total_amount);
        }

        public decimal GetYearlySales()
        {
            var startOfYear = new DateTime(DateTime.Today.Year, 1, 1);
            return Customers
                .Where(c => c.Created_at >= startOfYear)
                .Sum(c => c.Total_amount);
        }


        public class ApplicationUser : IdentityUser
        {
            public string? Type { get; set; } 
            public string? DisplayName { get; set; }
        }


    /* DbSets for your entities
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Staff> Staff { get; set; }
    public DbSet<WiibandRegistration> WiibandRegistrations { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Dashboard> Dashboard { get; set; }
    public DbSet<Registration> Registrations { get; set; }  // Add this line


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
    }*/
    public DbSet<DashboardDisplay> Dashboards { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
    }

    public class Transactions
    {
        public int Id { get; set; }
        public string? TransactionNumber { get; set; }
        public int WiibandID { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? RemainingTime { get; set; }
        public decimal Amount { get; set; } // assuming this is used for sales calculation
        public DateTime Date { get; set; } // assuming for GetTotalSalesForToday logic
        public string? Status { get; set; } // e.g., "Online" or "Offline"
    }

    public class DashboardDisplay
    {
        public int Id { get; set; }
        public required string TransactionNumber { get; set; }
        public int WiibandID { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public required string RemainingTime { get; set; }
        public required string Status { get; set; }
        public decimal? BatteryLevel { get; set; }

        // New properties for customer details
        public required string CustomerName { get; set; }
        public required string Email { get; set; }
    }
    public class Users
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Type { get; set; }
        public string? Display_name { get; set; }
        public string? Email_address { get; set; }
        public bool Email_notify { get; set; }
        public bool Sms_notify { get; set; }
        public bool Visibility { get; set; }
        public bool Third_party {  get; set; }
        public string? Theme { get; set; }
        public int ContactNum { get; set; }
        public string? Shift { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }

    public class Customers
    {
        public int Id { get; set; }
        public string? Customer_name { get; set; }
        public string? Customer_email { get; set; }
        public int Num_jumpers { get; set; }
        public int Promo { get; set; }
        public int Discount { get; set; }
        public decimal Total_amount { get; set; }
        public string? SignatureData { get; set; }
        public DateTime Created_at { get; set; }
    }

    public class Events
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int Phone { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public int Duration { get; set; }
        public int Jumpers { get; set; }
        public int Socks { get; set; }
        public string? Addons { get; set; }
        public int TrampolineGames { get; set; }
        public int PartyGuest { get; set; }
        public int PartyHours { get; set; }
        public string? PartyDecorations { get; set; }
        public int ElecFoodCart { get; set; }
        public int PartyEquipCD { get; set; }
        public int PartyEquipUtils { get; set; }
        public DateTime Created_at { get; set; }
    }
}