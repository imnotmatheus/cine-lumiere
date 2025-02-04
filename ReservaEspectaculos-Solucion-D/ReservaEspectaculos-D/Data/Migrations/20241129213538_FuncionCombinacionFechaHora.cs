using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservaEspectaculos_D.Data.Migrations
{
    /// <inheritdoc />
    public partial class FuncionCombinacionFechaHora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Funciones_SalaId",
                table: "Funciones");

            migrationBuilder.CreateIndex(
                name: "IX_Funciones_SalaId_Fecha_Hora_PeliculaId",
                table: "Funciones",
                columns: new[] { "SalaId", "Fecha", "Hora", "PeliculaId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Funciones_SalaId_Fecha_Hora_PeliculaId",
                table: "Funciones");

            migrationBuilder.CreateIndex(
                name: "IX_Funciones_SalaId",
                table: "Funciones",
                column: "SalaId");
        }
    }
}
