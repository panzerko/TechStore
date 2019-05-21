using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TechStore.Migrations
{
    public partial class AddedCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Goods");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Goods",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    ImageURL = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Goods_CategoryId",
                table: "Goods",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Goods_Categories_CategoryId",
                table: "Goods",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goods_Categories_CategoryId",
                table: "Goods");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Goods_CategoryId",
                table: "Goods");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Goods");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Goods",
                nullable: true);
        }
    }
}
