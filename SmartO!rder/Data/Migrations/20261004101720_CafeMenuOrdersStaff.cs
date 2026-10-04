using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartO_rder.Data.Migrations
{
    /// <inheritdoc />
    public partial class CafeMenuOrdersStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsServed",
                table: "Orders",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsReady",
                table: "Orders",
                newName: "DeliveryMethod");

            migrationBuilder.AddColumn<DateTime>(
                name: "WaiterCalledAt",
                table: "Tables",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "Orders",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "CafeOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PublicId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CafeId = table.Column<int>(type: "INTEGER", nullable: false),
                    TableId = table.Column<int>(type: "INTEGER", nullable: true),
                    TableNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReadyAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ServedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CafeOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CafeOrders_Cafes_CafeId",
                        column: x => x.CafeId,
                        principalTable: "Cafes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CafeOrders_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CafeStaff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CafeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CafeStaff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CafeStaff_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CafeStaff_Cafes_CafeId",
                        column: x => x.CafeId,
                        principalTable: "Cafes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    CafeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_Cafes_CafeId",
                        column: x => x.CafeId,
                        principalTable: "Cafes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CafeOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CafeOrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    MenuItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CafeOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CafeOrderItems_CafeOrders_CafeOrderId",
                        column: x => x.CafeOrderId,
                        principalTable: "CafeOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CafeOrderItems_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            // Existing store orders were created before checkout and payment existed:
            // give each a unique PublicId, copy the current product price, treat them as paid pickups.
            // (EF mapped IsServed/IsReady onto Status/DeliveryMethod as renames, so both are reset here.)
            migrationBuilder.Sql(@"
UPDATE Orders SET
    PublicId = upper(hex(randomblob(4))) || '-' || upper(hex(randomblob(2))) || '-' || upper(hex(randomblob(2))) || '-' ||
               upper(hex(randomblob(2))) || '-' || upper(hex(randomblob(6))),
    UnitPrice = COALESCE((SELECT Price FROM Products WHERE Products.Id = Orders.ProductId), 0),
    Status = 1,
    DeliveryMethod = 0;");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PublicId",
                table: "Orders",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CafeOrderItems_CafeOrderId",
                table: "CafeOrderItems",
                column: "CafeOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CafeOrderItems_MenuItemId",
                table: "CafeOrderItems",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CafeOrders_CafeId",
                table: "CafeOrders",
                column: "CafeId");

            migrationBuilder.CreateIndex(
                name: "IX_CafeOrders_PublicId",
                table: "CafeOrders",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CafeOrders_TableId",
                table: "CafeOrders",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_CafeStaff_CafeId",
                table: "CafeStaff",
                column: "CafeId");

            migrationBuilder.CreateIndex(
                name: "IX_CafeStaff_UserId_CafeId",
                table: "CafeStaff",
                columns: new[] { "UserId", "CafeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_CafeId",
                table: "MenuItems",
                column: "CafeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CafeOrderItems");

            migrationBuilder.DropTable(
                name: "CafeStaff");

            migrationBuilder.DropTable(
                name: "CafeOrders");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PublicId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WaiterCalledAt",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Orders",
                newName: "IsServed");

            migrationBuilder.RenameColumn(
                name: "DeliveryMethod",
                table: "Orders",
                newName: "IsReady");
        }
    }
}
