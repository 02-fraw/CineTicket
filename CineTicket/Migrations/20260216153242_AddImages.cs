using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineTicket.Migrations
{
    /// <inheritdoc />
    public partial class AddImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Movies",
                newName: "VerticalImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "HorizontalImageUrl",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HorizontalImageUrl",
                table: "Movies");

            migrationBuilder.RenameColumn(
                name: "VerticalImageUrl",
                table: "Movies",
                newName: "ImageUrl");
        }
    }
}
