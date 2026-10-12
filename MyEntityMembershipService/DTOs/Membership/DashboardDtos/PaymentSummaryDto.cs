namespace MyEntityMembershipService.DTOs.Membership.DashboardDtos
{
    public class PaymentSummaryDto
    {
        public int Month { get; set; } // Month of the payment (1-12)

        // Read-only property to get the month name from the month number
        public string MonthName
        {
            get
            {
                return new DateTime(1, Month, 1).ToString("MMMM"); // Convert month number to month name
            }
        }

        public double TotalAmountPaid { get; set; } // Sum of the amount paid for the month
        public PaymentSummaryDto() { }

        // Optionally, you can add a constructor to initialize the Month
        public PaymentSummaryDto(int month)
        {
            Month = month;
            TotalAmountPaid = 0; // Initialize to zero or any default value
        }
    }
}
