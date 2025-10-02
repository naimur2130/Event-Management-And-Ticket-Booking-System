using Event_Management_And_Ticket_Booking_System.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

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

                        page.Background("#ffffff");

                        page.Header()
                            .Text("🎫 Event Ticket")
                            .SemiBold()
                            .FontSize(24)
                            .AlignCenter();

                        page.Content()
                            .Padding(10)
                            .Border(1)
                            .BorderColor("#0d6efd")
                            .Border(10)
                            .Background("#f8f9fa")
                            .Row(row =>
                            {
                                
                                row.RelativeColumn(3)
                                    .Column(col =>
                                    {
                                        col.Item().Text($"Event: {ticket.Booking?.Event?.Title ?? "N/A"}")
                                            .Bold()
                                            .FontSize(18);
                                        col.Item().Text($"Attendee: {ticket.AttendeeName}").FontSize(14);
                                        col.Item().Text($"Email: {ticket.AttendeeEmail}").FontSize(14);
                                        col.Item().Text($"Phone: {ticket.AttendeePhone}").FontSize(14);
                                        col.Item().Text($"Ticket Code: {ticket.TicketCode}")
                                            .Bold()
                                            .FontSize(16);
                                        col.Item().Text($"Booking ID: {ticket.BookingId}").FontSize(14);
                                        col.Item().Text($"Status: {ticket.Booking?.Status.ToString() ?? "N/A"}").FontSize(14);
                                    });

                               
                                row.ConstantColumn(150)
                                    .AlignMiddle()
                                    .AlignCenter()
                                    .Image(GenerateQrCode(ticket.TicketCode), QuestPDF.Infrastructure.ImageScaling.FitWidth);
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text($"Generated on {DateTime.Now:g}")
                            .FontSize(10)
                            .SemiBold();
                    });
                }
            });

            return pdf.GeneratePdf();
        }

        private static byte[] GenerateQrCode(string qrText)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData); 
            return qrCode.GetGraphic(20); 
        }
    }
}
