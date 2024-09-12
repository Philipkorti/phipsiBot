using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbConnection.Migrations
{
    /// <inheritdoc />
    public partial class LoggingUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lgger",
                table: "Loggings"
                );
            migrationBuilder.AddColumn<string>(
                name: "Logger",
                table: "Loggings",
                nullable: true
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Logger",
                table: "Loggings",
                newName: "Lgger");
        }
    }
}
