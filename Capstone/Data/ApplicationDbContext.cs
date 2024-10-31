using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Data // Replace with your actual namespace if different
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Ensure this property is named "Users" (plural)
        public DbSet<Users> Users { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Events> Events { get; set; }
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
        public int Total_amount { get; set; }
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
        [Column(TypeName = "int")]
        public int TrampolineGames { get; set; } = 0;
        [Column(TypeName = "int")]
        public int PartyGuest { get; set; } = 0;
        [Column(TypeName = "int")]
        public int PartyHours { get; set; } = 0;
        [Column(TypeName = "int")]
        public int PartyDecorations { get; set; } = 0;
        [Column(TypeName = "int")]
        public int ElecFoodCart { get; set; } = 0;
        [Column(TypeName = "int")]
        public int PartyEquipCD { get; set; } = 0;
        [Column(TypeName = "int")]
        public int PartyEquipUtils { get; set; } = 0;
        public DateTime Created_at { get; set; }
    }
}