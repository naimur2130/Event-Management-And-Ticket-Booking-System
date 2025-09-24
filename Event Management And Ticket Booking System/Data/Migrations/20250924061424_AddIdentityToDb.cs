using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event_Management_And_Ticket_Booking_System.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_AspNetUsers_OrganizerId",
                table: "Event");

            migrationBuilder.RenameColumn(
                name: "OrganizerId",
                table: "Event",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_OrganizerId",
                table: "Event",
                newName: "IX_Event_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_AspNetUsers_UserId",
                table: "Event",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_AspNetUsers_UserId",
                table: "Event");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Event",
                newName: "OrganizerId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_UserId",
                table: "Event",
                newName: "IX_Event_OrganizerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_AspNetUsers_OrganizerId",
                table: "Event",
                column: "OrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
