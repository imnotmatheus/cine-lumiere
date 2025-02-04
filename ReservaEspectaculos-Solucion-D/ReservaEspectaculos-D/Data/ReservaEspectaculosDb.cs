using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReservaEspectaculos_D.Models;

namespace ReservaEspectaculos_D.Data
{
    public class ReservaEspectaculosDb : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
    {
        public ReservaEspectaculosDb(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TipoSala>().Property(ts => ts.Precio).HasPrecision(38, 18);

            modelBuilder.Entity<IdentityUser<int>>().ToTable("Personas");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("PersonasRoles");

            #region Unique
            modelBuilder.Entity<Empleado>().HasIndex(e => e.Legajo).IsUnique();
            modelBuilder.Entity<Pelicula>().HasIndex(p => p.Titulo).IsUnique();
            modelBuilder.Entity<TipoSala>().HasIndex(tp => tp.Nombre).IsUnique();
            modelBuilder.Entity<Genero>().HasIndex(g => g.Nombre).IsUnique();
            modelBuilder.Entity<Sala>().HasIndex(s => s.Numero).IsUnique();
            modelBuilder.Entity<Persona>().HasIndex(p => p.Email).IsUnique();
            modelBuilder.Entity<Funcion>().HasIndex(f => new { f.SalaId, f.Fecha, f.Hora, f.PeliculaId}).IsUnique();
            #endregion
        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Funcion> Funciones { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<TipoSala> TipoSalas { get; set; }
    }
}
