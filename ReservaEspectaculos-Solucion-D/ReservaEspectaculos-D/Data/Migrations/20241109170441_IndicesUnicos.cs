using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservaEspectaculos_D.Data.Migrations
{
    /// <inheritdoc />
    public partial class IndicesUnicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TipoSalas_Nombre",
                table: "TipoSalas",
                column: "Nombre",
                unique: true,
                filter: "[Nombre] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Salas_Numero",
                table: "Salas",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_Email",
                table: "Personas",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Peliculas_Titulo",
                table: "Peliculas",
                column: "Titulo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Generos_Nombre",
                table: "Generos",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TipoSalas_Nombre",
                table: "TipoSalas");

            migrationBuilder.DropIndex(
                name: "IX_Salas_Numero",
                table: "Salas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_Email",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Peliculas_Titulo",
                table: "Peliculas");

            migrationBuilder.DropIndex(
                name: "IX_Generos_Nombre",
                table: "Generos");
        }
    }
}
