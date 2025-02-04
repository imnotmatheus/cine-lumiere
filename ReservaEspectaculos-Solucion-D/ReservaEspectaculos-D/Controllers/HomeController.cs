using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservaEspectaculos_D.Data;
using ReservaEspectaculos_D.Models;
using ReservaEspectaculos_D.Models.ViewModels;
using ReservaEspectaculos_D.Utils;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ReservaEspectaculos_D.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ReservaEspectaculosDb _context;

        public HomeController(ILogger<HomeController> logger, ReservaEspectaculosDb context)
        {
            _logger = logger;
            this._context = context;
        }

        public IActionResult Index()
        {
            var peliculas = PeliculaHelper.ObtenerPeliculasEnExibicion(_context);

            return View(peliculas);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Precios()
        {
            return View(await _context.TipoSalas.ToListAsync());
        }

        public IActionResult Corporativo()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
