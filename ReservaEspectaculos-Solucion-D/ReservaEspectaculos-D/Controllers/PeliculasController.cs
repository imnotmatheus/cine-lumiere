using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReservaEspectaculos_D.Data;
using ReservaEspectaculos_D.Models;
using ReservaEspectaculos_D.Models.ViewModels;
using ReservaEspectaculos_D.Utils;

namespace ReservaEspectaculos_D.Controllers
{
    [Authorize(Roles = "Empleado, Administrador")]

    public class PeliculasController : Controller
    {
        private readonly ReservaEspectaculosDb _context;
        private readonly ExceptionHandler _exceptionHandler;

        public PeliculasController(ReservaEspectaculosDb context, ExceptionHandler exceptionHandler)
        {
            _context = context;
            _exceptionHandler = exceptionHandler;
        }

        // GET: Peliculas
        public async Task<IActionResult> Index(int? generoId)
        {
            IEnumerable<Pelicula> peliculas = await _context.Peliculas.Include(p => p.Genero).ToListAsync();

            if (generoId != null && generoId != -1)
            {
                peliculas = peliculas.Where(p =>
                p.GeneroId == generoId);
                if (peliculas.IsNullOrEmpty())
                {
                    return BadRequest("Ninguna pelicula con el genero seleccionado");
                }
            }

            ViewData["generos"] = await _context.Generos.ToListAsync();
            return View(peliculas);
        }

        // GET: Peliculas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var fechaActual = DateTimeHelper.ObtenerFechaActual();
            var ultimos30Dias = fechaActual.AddDays(-30);

            var pelicula = await _context.Peliculas
                     .Include(p => p.Genero)
                     .Include(p => p.Funciones)
                          .ThenInclude(f => f.Reservas)
                     .Include(p => p.Funciones)
                          .ThenInclude(f => f.Sala) 
                          .ThenInclude(s => s.TipoSala) 
                     .FirstOrDefaultAsync(p => p.Id == id);


            if (pelicula == null)
            {
                return NotFound();
            }
            decimal recaudacionTotal = 0;

            var funcionesUltimos30Dias = pelicula.Funciones.Where(f =>
                     (f.Fecha <= fechaActual && f.Fecha >= ultimos30Dias)).ToList();
            if (!funcionesUltimos30Dias.IsNullOrEmpty())
            {
                foreach (Funcion funcion in funcionesUltimos30Dias)
                {
                    recaudacionTotal += FuncionHelper.calcularRecaudacion(funcion);
                }
            }
            DetallesPelicula model = new()
            {
                Pelicula = pelicula,
                RecaudacionTotal = recaudacionTotal,
                Funciones = pelicula.Funciones
            };

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> InfoPelicula(int? peliculaId)
        {
            if (peliculaId == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Peliculas
                .Include(p => p.Genero)
                .Include(p => p.Funciones)
                .ThenInclude(f => f.Sala)
                .ThenInclude(s => s.TipoSala)
                .FirstOrDefaultAsync(p => p.Id == peliculaId);

            if (pelicula == null)
            {
                return NotFound();
            }

            InfoPelicula model = new()
            {
                PeliculaId = pelicula.Id,
                Titulo = pelicula.Titulo,
                FechaLanzamiento = pelicula.FechaLanzamiento,
                Descripcion = pelicula.Descripcion,
                Funciones = PeliculaHelper.ObtenerFuncionesFuturasDePelicula(pelicula),
                Genero = pelicula.Genero,
                PathCartel = pelicula.PathCartel
            };

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Cartelera(int? generoId)
        {
            var generos = await _context.Generos.ToListAsync();
            var peliculas = PeliculaHelper.ObtenerPeliculasEnExibicion(_context);

            if (peliculas.IsNullOrEmpty())
            {
                return BadRequest("Ninguna pelicula con funciones disponibles");
            }

            if (generoId != null && generoId != -1)
            {
                peliculas = peliculas.Where(p =>
                p.GeneroId == generoId);
                if (peliculas.IsNullOrEmpty())
                {
                    return BadRequest("Ninguna pelicula con el genero seleccionado");
                }
            }

            Cartelera model = new()
            {
                Peliculas = peliculas,
                Generos = generos,
            };

            return View(model);
        }

        // GET: Peliculas/Create
        public IActionResult Create()
        {
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre");
            return View();
        }

        // POST: Peliculas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,FechaLanzamiento,Descripcion,GeneroId")] Pelicula pelicula, IFormFile cartel)
        {
            if (TituloPeliculaExiste(pelicula.Titulo))
            {
                ModelState.AddModelError("Titulo", ErrorHelper.Titulo);
            }

            if (ModelState.IsValid)
            {
                _context.Add(pelicula);

                try
                {
                    if (cartel != null)
                    {
                        // string del directorio de la imagen (identificada por el Id de pelicula en ~/images/carteles)
                        string filePath = ImgHelper.FilePathCartel(pelicula, cartel);
                        string htmlPath = ImgHelper.HtmlPathCartel(pelicula, cartel);

                        // guarda en el sistema
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await cartel.CopyToAsync(stream);
                        }
                        // guarda el path en Pelicula
                        pelicula.PathCartel = htmlPath;
                    }
                    else
                    {
                        // Asigna un path por defecto si no se proporciona un cartel
                        pelicula.PathCartel = "/images/carteles/default_cartel.png";
                    }

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _exceptionHandler.ProcesarIndicesUnicos(ex, ModelState, "Titulo", ErrorHelper.Titulo);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, ErrorHelper.ErrorGenerico(e));
                }
            }

            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", pelicula.GeneroId);
            return View(pelicula);
        }

        // GET: Peliculas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula == null)
            {
                return NotFound();
            }
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", pelicula.GeneroId);
            return View(pelicula);
        }

        // POST: Peliculas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,FechaLanzamiento,Descripcion,GeneroId")] Pelicula pelicula, IFormFile cartel)
        {
            if (id != pelicula.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pelicula);
                    await _context.SaveChangesAsync();

                    if (cartel != null)
                    {
                        // string del directorio de la imagen (identificada por el Id de pelicula en ~/images/carteles)
                        string filePath = ImgHelper.FilePathCartel(pelicula, cartel);
                        string htmlPath = ImgHelper.HtmlPathCartel(pelicula, cartel);

                        // guarda en el sistema
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await cartel.CopyToAsync(stream);
                        }
                        // guarda el path en Pelicula
                        pelicula.PathCartel = htmlPath;
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PeliculaExists(pelicula.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException ex)
                {
                    _exceptionHandler.ProcesarIndicesUnicos(ex, ModelState, "Titulo", ErrorHelper.Titulo);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, ErrorHelper.ErrorGenerico(e));
                }
            }

            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", pelicula.GeneroId);

            return View(pelicula);
        }

        // GET: Peliculas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Peliculas
                .Include(p => p.Genero)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        // POST: Peliculas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula != null)
            {
                _context.Peliculas.Remove(pelicula);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PeliculaExists(int id)
        {
            return _context.Peliculas.Any(e => e.Id == id);
        }

        public IActionResult TituloDisponible(string titulo)
        {
            if (TituloPeliculaExiste(titulo))
            {
                return Json(ErrorHelper.Titulo);

            }

            return Json(true);
        }

        private bool TituloPeliculaExiste(string titulo)
        {
            return _context.Peliculas.Any(p => p.Titulo == titulo);
        }
    }
}
