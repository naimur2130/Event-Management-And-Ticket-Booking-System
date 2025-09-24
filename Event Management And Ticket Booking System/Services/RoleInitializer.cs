using Event_Management_And_Ticket_Booking_System.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;

namespace Event_Management_And_Ticket_Booking_System.Services
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Organizer", "Attendee" };

            foreach (var i in roles)
            {
                if (!await roleManager.RoleExistsAsync(i))
                {
                    await roleManager.CreateAsync(new IdentityRole(i));
                }
            }

            var adminEmail = "admin@gmail.com";
            var adminPassword = "@Admin1234";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await userManager.CreateAsync(user, adminPassword);
                await userManager.AddToRoleAsync(user, "Admin");
            }

        }
    }
}