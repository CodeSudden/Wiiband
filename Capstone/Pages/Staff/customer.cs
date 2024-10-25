namespace Capstone.Pages.Staff
{
    public class Customer
    {
            public int Id { get; set; }  // Primary Key
            public string CustomerName { get; set; }
            public string Email { get; set; }
            public int NumberOfJumpers { get; set; }
            public bool DiscountPWD { get; set; }
            public decimal TotalAmount { get; set; }
            public string TransactionNumber { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }

    }

