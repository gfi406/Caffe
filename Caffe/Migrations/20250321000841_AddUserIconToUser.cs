using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caffe.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIconToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "UserIcon",
                table: "Users",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserIcon",
                table: "Users");
        }
    }
}
