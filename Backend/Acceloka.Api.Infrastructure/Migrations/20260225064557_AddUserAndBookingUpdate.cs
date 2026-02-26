using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acceloka.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndBookingUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BookedTickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoogleId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookedTickets_UserId",
                table: "BookedTickets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookedTickets_Users_UserId",
                table: "BookedTickets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookedTickets_Users_UserId",
                table: "BookedTickets");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_BookedTickets_UserId",
                table: "BookedTickets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BookedTickets");
        }
    }
}
