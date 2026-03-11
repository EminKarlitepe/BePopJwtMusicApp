using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMembershipPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Features",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Üyelik kategorisini ekle
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Name", "Description" },
                values: new object[] { 1, "Üyelik", "Bepop üyelik paketleri" });

            // 3 paketi seed et
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Name", "Sku", "CategoryId", "Price", "Level", "Features", "IsActive", "CreatedAt" },
                values: new object[,]
                {
                    { 1, "Ücretsiz", "FREE-001", 1, 0m,      1, "Reklamlı Dinleme,Standart Ses Kalitesi,Sınırlı Atlama Hakları", true, new DateTime(2026, 1, 1) },
                    { 2, "Gold",     "GOLD-001", 1, 89.99m,  2, "Reklamsız Deneyim,Yüksek Ses Kalitesi,Sınırsız Şarkı Atlama",  true, new DateTime(2026, 1, 1) },
                    { 3, "Premium",  "PREM-001", 1, 129.99m, 3, "Çevrimdışı Dinleme,Ultra HD Ses,Aynı Anda 3 Cihaz",            true, new DateTime(2026, 1, 1) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Products", keyColumn: "ProductId", keyValue: 1);
            migrationBuilder.DeleteData(table: "Products", keyColumn: "ProductId", keyValue: 2);
            migrationBuilder.DeleteData(table: "Products", keyColumn: "ProductId", keyValue: 3);
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "CategoryId", keyValue: 1);

            migrationBuilder.DropColumn(name: "Features", table: "Products");
            migrationBuilder.DropColumn(name: "IsActive", table: "Products");
            migrationBuilder.DropColumn(name: "Level", table: "Products");
        }
    }
}