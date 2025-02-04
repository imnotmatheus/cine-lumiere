using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservaEspectaculos_D.Data.Migrations
{
    /// <inheritdoc />
    public partial class EstadoReserva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstadoReserva",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoReserva",
                table: "Reservas");
        }
    }
}
