using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservaEspectaculos_D.Data.Migrations
{
    /// <inheritdoc />
    public partial class CartelPelicula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PathCartel",
                table: "Peliculas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PathCartel",
                table: "Peliculas");
        }
    }
}
