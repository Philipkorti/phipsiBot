using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbConnection.Migrations
{
    /// <inheritdoc />
    public partial class FilterWords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilterWords",
                columns: table => new
                {
                    Words = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterWords", x => x.Words);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilterWords");
        }
    }
}
