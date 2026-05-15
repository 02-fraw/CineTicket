using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineTicket.Migrations
{
    /// <inheritdoc />
    public partial class FixEmailColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "EmailAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Users",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                table: "Users",
                newName: "Email");
        }
    }
}
