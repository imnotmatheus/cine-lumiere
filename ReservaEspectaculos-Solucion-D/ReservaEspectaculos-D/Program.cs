using ReservaEspectaculos_D;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using ReservaEspectaculos_D.Data;
using Microsoft.Extensions.Configuration;
using ReservaEspectaculos_D.Models;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using ReservaEspectaculos_D.Utils;


namespace ReservaEspectaculos_D 
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adaptation for Martin´s computer running sql instance with Docker
            string machineName = Environment.MachineName;
            string sqlInstance;

            if (machineName.Equals("MARTIN-BOOK"))
            {
                sqlInstance = "ReservaEspectaculosMartin";
            } else
            {
                sqlInstance = "ReservaEspectaculosDBCS";
            }

            // Add services to the container.
            string dbConnectionString = builder.Configuration.GetConnectionString(sqlInstance);
            builder.Services.AddDbContext<ReservaEspectaculosDb>(options => options.UseSqlServer(dbConnectionString));

            builder.Services.AddIdentity<Persona, IdentityRole<int>>().AddEntityFrameworkStores<ReservaEspectaculosDb>();

            builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, opciones =>
            {
                opciones.LoginPath = "/Account/IniciarSesion";
                opciones.AccessDeniedPath = "/Account/AccesoDenegado";
                opciones.Cookie.Name = "ReservaEspectaculos";
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<PersonaHelper>();
            builder.Services.AddScoped<ExceptionHandler>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
            {

                var contexto = serviceScope.ServiceProvider.GetRequiredService<ReservaEspectaculosDb>();
                contexto.Database.Migrate();

            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
