using Microsoft.AspNetCore.Http;
using ReservaEspectaculos_D.Models;
using System;
using System.IO;

namespace ReservaEspectaculos_D.Utils
{
    public static class ImgHelper
    {
        public static string DirectorioCarteles() => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\carteles");

        public static string GenerarNombreArchivoCartel(Pelicula pelicula, IFormFile cartel)
        {
            string extension = Path.GetExtension(cartel.FileName);
            return pelicula.Id.ToString() + extension;
        }

        public static string FilePathCartel(Pelicula pelicula, IFormFile cartel)
        {
            return Path.Combine(ImgHelper.DirectorioCarteles(), ImgHelper.GenerarNombreArchivoCartel(pelicula, cartel));
        }

        public static string HtmlPathCartel(Pelicula pelicula, IFormFile cartel)
        {
            return Config.RutaImagenPelicula + GenerarNombreArchivoCartel(pelicula, cartel);
        }
    }
}
