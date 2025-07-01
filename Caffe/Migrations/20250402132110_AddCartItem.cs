using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caffe.Migrations
{
    /// <inheritdoc />
    public partial class AddCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_Carts_CartId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_CartId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "CartId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "quantity",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "quantity",
                table: "Carts");

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CartId = table.Column<Guid>(type: "uuid", nullable: false),
                    MenuItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_MenuItemId",
                table: "CartItems",
                column: "MenuItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.AddColumn<Guid>(
                name: "CartId",
                table: "MenuItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "MenuItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "Carts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_CartId",
                table: "MenuItems",
                column: "CartId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_Carts_CartId",
                table: "MenuItems",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id");
        }
    }
}
