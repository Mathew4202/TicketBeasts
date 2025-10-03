using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TicketBeasts.Migrations
{
    /// <inheritdoc />
    public partial class CreateSports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Owners_OwnerId",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Owners",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "Sports");

            migrationBuilder.RenameIndex(
                name: "IX_Events_OwnerId",
                table: "Sports",
                newName: "IX_Sports_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_CategoryId",
                table: "Sports",
                newName: "IX_Sports_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sports",
                table: "Sports",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sports_Categories_CategoryId",
                table: "Sports",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sports_Owners_OwnerId",
                table: "Sports",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sports_Categories_CategoryId",
                table: "Sports");

            migrationBuilder.DropForeignKey(
                name: "FK_Sports_Owners_OwnerId",
                table: "Sports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sports",
                table: "Sports");

            migrationBuilder.RenameTable(
                name: "Sports",
                newName: "Events");

            migrationBuilder.RenameIndex(
                name: "IX_Sports_OwnerId",
                table: "Events",
                newName: "IX_Events_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Sports_CategoryId",
                table: "Events",
                newName: "IX_Events_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Concert" },
                    { 2, "Sports" },
                    { 3, "Comedy" }
                });

            migrationBuilder.InsertData(
                table: "Owners",
                columns: new[] { "Id", "ContactEmail", "Name" },
                values: new object[,]
                {
                    { 1, "events@school.edu", "Campus Events" },
                    { 2, "su@school.edu", "Student Union" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Owners_OwnerId",
                table: "Events",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
