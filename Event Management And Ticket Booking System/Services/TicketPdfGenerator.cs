using Event_Management_And_Ticket_Booking_System.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Event_Management_And_Ticket_Booking_System.Services
{
    public class TicketPdfGenerator
    {
        public static byte[] GeneratePdf(List<Tickets> tickets)
        {
            var pdf = Document.Create(container =>
            {
                foreach (var ticket in tickets)
                {
                    container.Page(page =>
                    {
                        page.Margin(20);

                        page.Header()
                            .Text("Event Ticket")
                            .SemiBold().FontSize(24).AlignCenter();

                        page.Content()
                            .Column(col =>
                            {
                                col.Item().Text($"Event: {ticket.Booking?.Event?.Title ?? "N/A"}").Bold().FontSize(18);
                                col.Item().Text($"Attendee: {ticket.AttendeeName}");
                                col.Item().Text($"Email: {ticket.AttendeeEmail}");
                                col.Item().Text($"Phone: {ticket.AttendeePhone}");
                                col.Item().Text($"Ticket Code: {ticket.TicketCode}").Bold();
                                col.Item().Text($"Booking ID: {ticket.BookingId}");
                                col.Item().Text("----------------------------------------");
                            });
                    });
                }
            });

            return pdf.GeneratePdf();
        }
    }
}
