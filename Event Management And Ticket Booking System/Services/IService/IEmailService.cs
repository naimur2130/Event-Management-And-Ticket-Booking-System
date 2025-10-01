namespace Event_Management_And_Ticket_Booking_System.Services.IService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
    }
}
