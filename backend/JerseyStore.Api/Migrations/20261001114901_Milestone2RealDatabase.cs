using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JerseyStore.Api.Migrations
{
    /// <inheritdoc />
    public partial class Milestone2RealDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Slug = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Club = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Season = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    KitType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "product_images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    AltText = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_images_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_variants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SizeOrder = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    Sku = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_variants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_variants_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "Id", "Club", "Description", "DisplayOrder", "IsActive", "KitType", "Name", "Season", "Slug" },
                values: new object[,]
                {
                    { 1, "Real Madrid", "White home shirt with the club crest and Adidas styling.", 1, true, "Home", "Real Madrid 2025/26 Home Jersey", "2025/26", "real-madrid-2025-26-home" },
                    { 2, "Liverpool", "Classic red home kit with the Liver Bird crest.", 2, true, "Home", "Liverpool 2025/26 Home Jersey", "2025/26", "liverpool-2025-26-home" },
                    { 3, "Arsenal", "Yellow away kit with navy accents.", 3, true, "Away", "Arsenal 2025/26 Away Jersey", "2025/26", "arsenal-2025-26-away" },
                    { 4, "Manchester City", "Black third kit with teal accents.", 4, true, "Third", "Manchester City 2025/26 Third Jersey", "2025/26", "manchester-city-2025-26-third" },
                    { 5, "Barcelona", "Blaugrana stripes with gold trim.", 5, true, "Home", "Barcelona 2024/25 Home Jersey", "2024/25", "barcelona-2024-25-home" },
                    { 6, "Bayern Munich", "White away kit with red sleeves.", 6, true, "Away", "Bayern Munich 2024/25 Away Jersey", "2024/25", "bayern-munich-2024-25-away" },
                    { 7, "Juventus", "Black and white stripes with adidas branding.", 7, true, "Home", "Juventus 2024/25 Home Jersey", "2024/25", "juventus-2024-25-home" },
                    { 8, "PSG", "Pink third kit with Jordan branding.", 8, true, "Third", "PSG 2024/25 Third Jersey", "2024/25", "psg-2024-25-third" }
                });

            migrationBuilder.InsertData(
                table: "product_images",
                columns: new[] { "Id", "AltText", "DisplayOrder", "IsPrimary", "ProductId", "Url" },
                values: new object[,]
                {
                    { 1, "Real Madrid 2025/26 home jersey", 1, true, 1, "https://placehold.co/800x1000/png?text=Real+Madrid+2025%2F26+Home" },
                    { 2, "Liverpool 2025/26 home jersey", 1, true, 2, "https://placehold.co/800x1000/png?text=Liverpool+2025%2F26+Home" },
                    { 3, "Arsenal 2025/26 away jersey", 1, true, 3, "https://placehold.co/800x1000/png?text=Arsenal+2025%2F26+Away" },
                    { 4, "Manchester City 2025/26 third jersey", 1, true, 4, "https://placehold.co/800x1000/png?text=Manchester+City+2025%2F26+Third" },
                    { 5, "Barcelona 2024/25 home jersey", 1, true, 5, "https://placehold.co/800x1000/png?text=Barcelona+2024%2F25+Home" },
                    { 6, "Bayern Munich 2024/25 away jersey", 1, true, 6, "https://placehold.co/800x1000/png?text=Bayern+Munich+2024%2F25+Away" },
                    { 7, "Juventus 2024/25 home jersey", 1, true, 7, "https://placehold.co/800x1000/png?text=Juventus+2024%2F25+Home" },
                    { 8, "PSG 2024/25 third jersey", 1, true, 8, "https://placehold.co/800x1000/png?text=PSG+2024%2F25+Third" }
                });

            migrationBuilder.InsertData(
                table: "product_variants",
                columns: new[] { "Id", "IsActive", "Price", "ProductId", "RowVersion", "Size", "SizeOrder", "Sku", "Stock" },
                values: new object[,]
                {
                    { 101, true, 350.00m, 1, null, "S", 1, null, 4 },
                    { 102, true, 350.00m, 1, null, "M", 2, null, 6 },
                    { 103, true, 350.00m, 1, null, "L", 3, null, 0 },
                    { 104, true, 380.00m, 1, null, "XL", 4, null, 2 },
                    { 105, true, 340.00m, 2, null, "S", 1, null, 5 },
                    { 106, true, 340.00m, 2, null, "M", 2, null, 7 },
                    { 107, true, 340.00m, 2, null, "L", 3, null, 3 },
                    { 108, true, 370.00m, 2, null, "XL", 4, null, 1 },
                    { 109, true, 330.00m, 3, null, "S", 1, null, 6 },
                    { 110, true, 330.00m, 3, null, "M", 2, null, 0 },
                    { 111, true, 330.00m, 3, null, "L", 3, null, 4 },
                    { 112, true, 360.00m, 3, null, "XL", 4, null, 2 },
                    { 113, true, 360.00m, 4, null, "S", 1, null, 3 },
                    { 114, true, 360.00m, 4, null, "M", 2, null, 5 },
                    { 115, true, 360.00m, 4, null, "L", 3, null, 2 },
                    { 116, true, 390.00m, 4, null, "XL", 4, null, 0 },
                    { 117, true, 350.00m, 5, null, "S", 1, null, 4 },
                    { 118, true, 350.00m, 5, null, "M", 2, null, 6 },
                    { 119, true, 350.00m, 5, null, "L", 3, null, 0 },
                    { 120, true, 380.00m, 5, null, "XL", 4, null, 2 },
                    { 121, true, 340.00m, 6, null, "S", 1, null, 5 },
                    { 122, true, 340.00m, 6, null, "M", 2, null, 0 },
                    { 123, true, 340.00m, 6, null, "L", 3, null, 3 },
                    { 124, true, 370.00m, 6, null, "XL", 4, null, 1 },
                    { 125, true, 330.00m, 7, null, "S", 1, null, 6 },
                    { 126, true, 330.00m, 7, null, "M", 2, null, 4 },
                    { 127, true, 330.00m, 7, null, "L", 3, null, 0 },
                    { 128, true, 360.00m, 7, null, "XL", 4, null, 2 },
                    { 129, true, 380.00m, 8, null, "S", 1, null, 3 },
                    { 130, true, 380.00m, 8, null, "M", 2, null, 5 },
                    { 131, true, 380.00m, 8, null, "L", 3, null, 0 },
                    { 132, true, 410.00m, 8, null, "XL", 4, null, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_product_images_ProductId_DisplayOrder",
                table: "product_images",
                columns: new[] { "ProductId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_product_images_ProductId_IsPrimary",
                table: "product_images",
                columns: new[] { "ProductId", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_product_variants_IsActive",
                table: "product_variants",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_product_variants_ProductId_Size",
                table: "product_variants",
                columns: new[] { "ProductId", "Size" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_variants_ProductId_SizeOrder",
                table: "product_variants",
                columns: new[] { "ProductId", "SizeOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_products_Club_Season",
                table: "products",
                columns: new[] { "Club", "Season" });

            migrationBuilder.CreateIndex(
                name: "IX_products_Slug",
                table: "products",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_images");

            migrationBuilder.DropTable(
                name: "product_variants");

            migrationBuilder.DropTable(
                name: "products");
        }
    }
}
