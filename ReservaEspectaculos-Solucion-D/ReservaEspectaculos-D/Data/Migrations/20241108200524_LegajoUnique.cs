using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservaEspectaculos_D.Data.Migrations
{
    /// <inheritdoc />
    public partial class LegajoUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Legajo",
                table: "Personas",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_Legajo",
                table: "Personas",
                column: "Legajo",
                unique: true,
                filter: "[Legajo] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Personas_Legajo",
                table: "Personas");

            migrationBuilder.AlterColumn<string>(
                name: "Legajo",
                table: "Personas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
