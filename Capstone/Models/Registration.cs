using System;
using System.ComponentModel.DataAnnotations;

namespace Capstone.Models
{
    public class Registration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int NumberOfJumpers { get; set; }

        [Required]
        public string SelectedPromo { get; set; }

        public bool DiscountPWD { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}
