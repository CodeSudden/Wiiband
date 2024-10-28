using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;

namespace Capstone.Data // Replace with your actual namespace if different
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Ensure this property is named "Users" (plural)
        public DbSet<Users> Users { get; set; }
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
}
