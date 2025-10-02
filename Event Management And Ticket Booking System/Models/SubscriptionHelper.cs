namespace Event_Management_And_Ticket_Booking_System.Models
{
    public static class SubscriptionHelper
    {
        public static decimal GetAmount(SubscriptionType type)
        {
            return type switch
            {
                SubscriptionType.Monthly => 500,   
                SubscriptionType.Quarterly => 1400, 
                SubscriptionType.Yearly => 5000,  
                _ => 0
            };
        }

        public static int GetDurationInMonths(SubscriptionType type)
        {
            return type switch
            {
                SubscriptionType.Monthly => 1,
                SubscriptionType.Quarterly => 3,
                SubscriptionType.Yearly => 12,
                _ => 0
            };
        }
    }
}
