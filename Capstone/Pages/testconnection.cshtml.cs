using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

namespace Capstone.Pages
{
    public class TestconnectionModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public TestconnectionModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string? Message { get; set; }

        public void OnGet()
        {
            // Get the connection string from appsettings.json
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                Message = "Connection string is missing!";
                return;
            }

            try
            {
                // Using ADO.NET to test the connection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Message = "Database connection successful!";
                }
            }
            catch (Exception ex)
            {
                Message = $"Database connection failed: {ex.Message}";
            }
        }
    }
}
