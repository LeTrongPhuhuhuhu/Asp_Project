using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _6TL.Migrations
{
    /// <inheritdoc />
    public partial class FixProductColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Admins__RoleID__0F624AF8",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK__Customers__RoleI__10566F31",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK__Orders__Customer__1332DBDC",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK__Products__Catego__14270015",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK__Products__Discou__151B244E",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK__Products__Suppli__160F4887",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK__Wishlist__Custom__00200768",
                table: "Wishlist");

            migrationBuilder.DropForeignKey(
                name: "FK__Wishlist__Produc__01142BA1",
                table: "Wishlist");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Wishlist__233189CB9F977389",
                table: "Wishlist");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Supplier__4BE66694FDA050D9",
                table: "Suppliers");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Roles__8AFACE3A550A5E75",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Reviews__74BC79AEF8BC184D",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Products__B40CC6ED4AE28430",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Orders__C3905BAFCD230F61",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Discount__E43F6DF62B330B64",
                table: "Discounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Customer__A4AE64B8451829F3",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Contacts__5C66259BB0F8617E",
                table: "Contacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Categori__19093A2B821E2B94",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Admins__719FE4E86E1F0F57",
                table: "Admins");

            migrationBuilder.DropPrimaryKey(
                name: "PK__OrderDet__D3B9D30C4BBCA0BD",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WarrantyPeriod",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OrderDetailID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OrderDetails");

            migrationBuilder.RenameTable(
                name: "OrderDetails",
                newName: "OrderDetail");

            migrationBuilder.RenameIndex(
                name: "UQ__Products__BC7B5FB6EE12F04C",
                table: "Products",
                newName: "UQ__Products__BC7B5FB6C1215808");

            migrationBuilder.RenameIndex(
                name: "UQ__Products__BC7B5FB67C683897",
                table: "Products",
                newName: "UQ__Products__BC7B5FB6A0C31C3B");

            migrationBuilder.RenameIndex(
                name: "UQ__Products__BC7B5FB65AD48B1A",
                table: "Products",
                newName: "UQ__Products__BC7B5FB689904554");

            migrationBuilder.RenameIndex(
                name: "UQ__Categori__BC7B5FB6BA69DACE",
                table: "Categories",
                newName: "UQ__Categori__BC7B5FB6B7D58DA3");

            migrationBuilder.RenameIndex(
                name: "UQ__Categori__BC7B5FB650ABD715",
                table: "Categories",
                newName: "UQ__Categori__BC7B5FB6998E1E3B");

            migrationBuilder.RenameIndex(
                name: "UQ__Categori__BC7B5FB64683A223",
                table: "Categories",
                newName: "UQ__Categori__BC7B5FB6643E41AB");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "OrderDetail",
                newName: "TotalPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Wishlist",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductImage",
                table: "Wishlist",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "Wishlist",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Wishlist",
                type: "float",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Rating",
                table: "Products",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "ParentCategoryId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "OrderDetail",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "OrderDetail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "OrderDetail",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductID",
                table: "OrderDetail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderID",
                table: "OrderDetail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "OrderDetail",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "OrderDetail",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK__Wishlist__233189CB6CA3273E",
                table: "Wishlist",
                column: "WishlistID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Supplier__4BE66694C31EF2A9",
                table: "Suppliers",
                column: "SupplierID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Roles__8AFACE3A424D3D7D",
                table: "Roles",
                column: "RoleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Reviews__74BC79AE995156EF",
                table: "Reviews",
                column: "ReviewID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Products__B40CC6ED8C764A37",
                table: "Products",
                column: "ProductID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Orders__C3905BAFA9130D07",
                table: "Orders",
                column: "OrderID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Discount__E43F6DF6AF17C81A",
                table: "Discounts",
                column: "DiscountID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Customer__A4AE64B82B00D0AE",
                table: "Customers",
                column: "CustomerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Contacts__5C66259B389365BA",
                table: "Contacts",
                column: "ContactId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Categori__19093A2BBDAF0536",
                table: "Categories",
                column: "CategoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Admins__719FE4E87A0B2101",
                table: "Admins",
                column: "AdminID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__OrderDet__08D097C15615C8DE",
                table: "OrderDetail",
                columns: new[] { "OrderID", "ProductID" });

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cart__51BCD7B7F4978FC7", x => x.CartId);
                    table.ForeignKey(
                        name: "FK_Cart_Product",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    ColorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Colors__8DA7674DB28303E4", x => x.ColorId);
                });

            migrationBuilder.CreateTable(
                name: "ProductColor",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductColor", x => new { x.ProductID, x.ColorId });
                    table.ForeignKey(
                        name: "FK_ProductColor_Color",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "ColorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ProductID",
                table: "OrderDetail",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_ProductId",
                table: "Cart",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductColor_ColorId",
                table: "ProductColor",
                column: "ColorId");

            migrationBuilder.AddForeignKey(
                name: "FK__Admins__RoleID__6C190EBB",
                table: "Admins",
                column: "RoleID",
                principalTable: "Roles",
                principalColumn: "RoleID");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_ParentCategory",
                table: "Categories",
                column: "ParentCategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK__Customers__RoleI__70DDC3D8",
                table: "Customers",
                column: "RoleID",
                principalTable: "Roles",
                principalColumn: "RoleID");

            migrationBuilder.AddForeignKey(
                name: "FK__OrderDeta__Order__73BA3083",
                table: "OrderDetail",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__OrderDeta__Produ__74AE54BC",
                table: "OrderDetail",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK__Orders__Customer__75A278F5",
                table: "Orders",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK__Products__Catego__7A672E12",
                table: "Products",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK__Products__Discou__7D439ABD",
                table: "Products",
                column: "DiscountID",
                principalTable: "Discounts",
                principalColumn: "DiscountID");

            migrationBuilder.AddForeignKey(
                name: "FK__Products__Suppli__00200768",
                table: "Products",
                column: "SupplierID",
                principalTable: "Suppliers",
                principalColumn: "SupplierID");

            migrationBuilder.AddForeignKey(
                name: "FK__Wishlist__Custom__04E4BC85",
                table: "Wishlist",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK__Wishlist__Produc__07C12930",
                table: "Wishlist",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Admins__RoleID__6C190EBB",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK_Category_ParentCategory",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK__Customers__RoleI__70DDC3D8",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK__OrderDeta__Order__73BA3083",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK__OrderDeta__Produ__74AE54BC",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK__Orders__Customer__75A278F5",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK__Products__Catego__7A672E12",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK__Products__Discou__7D439ABD",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK__Products__Suppli__00200768",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK__Wishlist__Custom__04E4BC85",
                table: "Wishlist");

            migrationBuilder.DropForeignKey(
                name: "FK__Wishlist__Produc__07C12930",
                table: "Wishlist");

            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "ProductColor");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Wishlist__233189CB6CA3273E",
                table: "Wishlist");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Supplier__4BE66694C31EF2A9",
                table: "Suppliers");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Roles__8AFACE3A424D3D7D",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Reviews__74BC79AE995156EF",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Products__B40CC6ED8C764A37",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Orders__C3905BAFA9130D07",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Discount__E43F6DF6AF17C81A",
                table: "Discounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Customer__A4AE64B82B00D0AE",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Contacts__5C66259B389365BA",
                table: "Contacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Categori__19093A2BBDAF0536",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Admins__719FE4E87A0B2101",
                table: "Admins");

            migrationBuilder.DropPrimaryKey(
                name: "PK__OrderDet__08D097C15615C8DE",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_ProductID",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Wishlist");

            migrationBuilder.DropColumn(
                name: "ProductImage",
                table: "Wishlist");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "Wishlist");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Wishlist");

            migrationBuilder.DropColumn(
                name: "ParentCategoryId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "OrderDetail");

            migrationBuilder.RenameTable(
                name: "OrderDetail",
                newName: "OrderDetails");

            migrationBuilder.RenameIndex(
                name: "UQ__Products__BC7B5FB6C1215808",
                table: "Products",
                newName: "UQ__Products__BC7B5FB6EE12F04C");

            migrationBuilder.RenameIndex(
                name: "UQ__Products__BC7B5FB6A0C31C3B",
                table: "Products",
                newName: "UQ__Products__BC7B5FB67C683897");

            migrationBuilder.RenameIndex(
                name: "UQ__Products__BC7B5FB689904554",
                table: "Products",
                newName: "UQ__Products__BC7B5FB65AD48B1A");

            migrationBuilder.RenameIndex(
                name: "UQ__Categori__BC7B5FB6B7D58DA3",
                table: "Categories",
                newName: "UQ__Categori__BC7B5FB6BA69DACE");

            migrationBuilder.RenameIndex(
                name: "UQ__Categori__BC7B5FB6998E1E3B",
                table: "Categories",
                newName: "UQ__Categori__BC7B5FB650ABD715");

            migrationBuilder.RenameIndex(
                name: "UQ__Categori__BC7B5FB6643E41AB",
                table: "Categories",
                newName: "UQ__Categori__BC7B5FB64683A223");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "OrderDetails",
                newName: "Total");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rating",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarrantyPeriod",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "OrderDetails",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "OrderDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "OrderDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "ProductID",
                table: "OrderDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OrderID",
                table: "OrderDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "OrderDetailID",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OrderDetails",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OrderDetails",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK__Wishlist__233189CB9F977389",
                table: "Wishlist",
                column: "WishlistID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Supplier__4BE66694FDA050D9",
                table: "Suppliers",
                column: "SupplierID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Roles__8AFACE3A550A5E75",
                table: "Roles",
                column: "RoleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Reviews__74BC79AEF8BC184D",
                table: "Reviews",
                column: "ReviewID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Products__B40CC6ED4AE28430",
                table: "Products",
                column: "ProductID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Orders__C3905BAFCD230F61",
                table: "Orders",
                column: "OrderID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Discount__E43F6DF62B330B64",
                table: "Discounts",
                column: "DiscountID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Customer__A4AE64B8451829F3",
                table: "Customers",
                column: "CustomerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Contacts__5C66259BB0F8617E",
                table: "Contacts",
                column: "ContactId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Categori__19093A2B821E2B94",
                table: "Categories",
                column: "CategoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Admins__719FE4E86E1F0F57",
                table: "Admins",
                column: "AdminID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__OrderDet__D3B9D30C4BBCA0BD",
                table: "OrderDetails",
                column: "OrderDetailID");

            migrationBuilder.AddForeignKey(
                name: "FK__Admins__RoleID__0F624AF8",
                table: "Admins",
                column: "RoleID",
                principalTable: "Roles",
                principalColumn: "RoleID");

            migrationBuilder.AddForeignKey(
                name: "FK__Customers__RoleI__10566F31",
                table: "Customers",
                column: "RoleID",
                principalTable: "Roles",
                principalColumn: "RoleID");

            migrationBuilder.AddForeignKey(
                name: "FK__Orders__Customer__1332DBDC",
                table: "Orders",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK__Products__Catego__14270015",
                table: "Products",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK__Products__Discou__151B244E",
                table: "Products",
                column: "DiscountID",
                principalTable: "Discounts",
                principalColumn: "DiscountID");

            migrationBuilder.AddForeignKey(
                name: "FK__Products__Suppli__160F4887",
                table: "Products",
                column: "SupplierID",
                principalTable: "Suppliers",
                principalColumn: "SupplierID");

            migrationBuilder.AddForeignKey(
                name: "FK__Wishlist__Custom__00200768",
                table: "Wishlist",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK__Wishlist__Produc__01142BA1",
                table: "Wishlist",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID");
        }
    }
}
