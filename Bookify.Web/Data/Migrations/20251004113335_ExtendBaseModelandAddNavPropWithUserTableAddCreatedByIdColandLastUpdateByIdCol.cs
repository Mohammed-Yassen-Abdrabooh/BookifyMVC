using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Web.Data.Migrations
{
    public partial class ExtendBaseModelandAddNavPropWithUserTableAddCreatedByIdColandLastUpdateByIdCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdateById",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Books",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdateById",
                table: "Books",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "BookCopies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdateById",
                table: "BookCopies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Authors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdateById",
                table: "Authors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdateById",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CreatedById",
                table: "Categories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_LastUpdateById",
                table: "Categories",
                column: "LastUpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CreatedById",
                table: "Books",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Books_LastUpdateById",
                table: "Books",
                column: "LastUpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_CreatedById",
                table: "BookCopies",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_LastUpdateById",
                table: "BookCopies",
                column: "LastUpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_CreatedById",
                table: "Authors",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_LastUpdateById",
                table: "Authors",
                column: "LastUpdateById");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_AspNetUsers_CreatedById",
                table: "Authors",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_AspNetUsers_LastUpdateById",
                table: "Authors",
                column: "LastUpdateById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_AspNetUsers_CreatedById",
                table: "BookCopies",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_AspNetUsers_LastUpdateById",
                table: "BookCopies",
                column: "LastUpdateById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_CreatedById",
                table: "Books",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_LastUpdateById",
                table: "Books",
                column: "LastUpdateById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_AspNetUsers_CreatedById",
                table: "Categories",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_AspNetUsers_LastUpdateById",
                table: "Categories",
                column: "LastUpdateById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_AspNetUsers_CreatedById",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_Authors_AspNetUsers_LastUpdateById",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_AspNetUsers_CreatedById",
                table: "BookCopies");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_AspNetUsers_LastUpdateById",
                table: "BookCopies");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_CreatedById",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_LastUpdateById",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AspNetUsers_CreatedById",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AspNetUsers_LastUpdateById",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CreatedById",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_LastUpdateById",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Books_CreatedById",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_LastUpdateById",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_BookCopies_CreatedById",
                table: "BookCopies");

            migrationBuilder.DropIndex(
                name: "IX_BookCopies_LastUpdateById",
                table: "BookCopies");

            migrationBuilder.DropIndex(
                name: "IX_Authors_CreatedById",
                table: "Authors");

            migrationBuilder.DropIndex(
                name: "IX_Authors_LastUpdateById",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "LastUpdateById",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "LastUpdateById",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "BookCopies");

            migrationBuilder.DropColumn(
                name: "LastUpdateById",
                table: "BookCopies");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "LastUpdateById",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastUpdateById",
                table: "AspNetUsers");
        }
    }
}
