using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
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

    public class FuncionesController : Controller
    {
        private readonly ReservaEspectaculosDb _context;
        private readonly ExceptionHandler _exceptionHandler;

        public FuncionesController(ReservaEspectaculosDb context, ExceptionHandler exceptionHandler)
        {
            _context = context;
            _exceptionHandler = exceptionHandler;
        }

        // GET: Funciones
        public async Task<IActionResult> Index()
        {
            var funciones = await _context.Funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Reservas)
                .Include(f => f.Sala)
                .ThenInclude(s => s.TipoSala).ToListAsync();

            List<FuncionEnIndex> verFunciones = [];

            foreach (Funcion f in funciones)
            {
                FuncionEnIndex model = new()
                {
                    Funcion = f,
                    Recaudacion = FuncionHelper.calcularRecaudacion(f)
                };

                verFunciones.Add(model);
            }

            return View(verFunciones);
        }

        // GET: Funciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcion = await _context.Funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .ThenInclude(s => s.TipoSala)
                .FirstOrDefaultAsync(m => m.Id == id);


            if (funcion == null)
            {
                return NotFound();
            }
            int butacasDisponibles = funcion.ButacasDisponibles;
            int capacidad = funcion.Sala.CapacidadButacas;
            float ocupacion = 0;

            if (capacidad > 0)
            {
                ocupacion = ((capacidad - butacasDisponibles) / (float)capacidad) * 100;
            }
            ViewData["Ocupacion"] = ocupacion;

            return View(funcion);
        }

        // GET: Funciones/Create
        public IActionResult Create()
        {
            Funcion funcion = new();
            DateTime ahora = DateTime.Now;
            funcion.Fecha = DateOnly.FromDateTime(ahora);
            funcion.Hora = TimeOnly.FromDateTime(ahora);
            funcion.Confirmada = true;
            funcion.Descripcion = "Estreno de gran peli pochoclera";
            ViewData["PeliculaId"] = new SelectList(_context.Set<Pelicula>(), "Id", "Titulo");
            ViewData["SalaId"] = new SelectList(_context.Set<Sala>()
                                .Join(_context.Set<TipoSala>(),
                                sala => sala.TipoSalaId,
                                tipoSala => tipoSala.Id,
                                (sala, tipoSala) => new { sala.Id, DisplayText = $"{sala.Numero} - {tipoSala.Nombre}" }), "Id", "DisplayText");
            return View(funcion);
        }

        // POST: Funciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fecha,Hora,Descripcion,ButacasDisponibles,Confirmada,PeliculaId,SalaId")] Funcion funcion)
        {
            var salaSeleccionada = await _context.Salas.FirstOrDefaultAsync(s => s.Id == funcion.SalaId);

            if (funcion.Fecha < DateOnly.FromDateTime(DateTime.Now))
            {
                ModelState.AddModelError("Fecha", "No es posible crear una función en el pasado");
            }
            else if (funcion.Fecha == DateOnly.FromDateTime(DateTime.Now) && funcion.Hora < TimeOnly.FromDateTime(DateTime.Now))
            {
                ModelState.AddModelError("Hora", "No es posible crear una función en el pasado");
            }

            if (salaSeleccionada == null)
            {
                ModelState.AddModelError("SalaId", "La sala seleccionada no es válida.");
            }

            if (FuncionDuplicada(funcion))
            {
                ModelState.AddModelError(string.Empty, ErrorHelper.FuncionDuplicada);
            }

            if (ModelState.IsValid)
            {
                funcion.ButacasDisponibles = salaSeleccionada.CapacidadButacas;
                funcion.Confirmada = true;
                _context.Add(funcion);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _exceptionHandler.ProcesarIndicesUnicos(ex, ModelState, "Fecha", ErrorHelper.FuncionDuplicada);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, ErrorHelper.ErrorGenerico(e));
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["PeliculaId"] = new SelectList(_context.Set<Pelicula>(), "Id", "Titulo", funcion.PeliculaId);
            ViewData["SalaId"] = new SelectList(_context.Set<Sala>(), "Id", "Numero", funcion.SalaId);
            return View(funcion);
        }

        public async Task<IActionResult> VerReservas(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcion = await _context.Funciones
                .Include(f => f.Reservas)
                .ThenInclude(r => r.Cliente)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (funcion == null)
            {
                return NotFound();
            }
            var reservas = funcion.Reservas;

            return View(reservas);
        }

        public async Task<IActionResult> CambiarConfirmacionFuncion(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcion = await _context.Funciones.FindAsync(id);
            if (funcion == null)
            {
                return NotFound();
            }

            // no permitir cambiar la confirmacion de funciones que ya pasaron
            if (!await FuncionPasada((int)id))
            {
                funcion.Confirmada = !funcion.Confirmada;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, ErrorHelper.ErrorGenerico(e));
                }
            }



            return RedirectToAction(nameof(Index));
        }


        // GET: Funciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcion = await _context.Funciones.FindAsync(id);
            if (funcion == null)
            {
                return NotFound();
            }
            ViewData["PeliculaId"] = new SelectList(_context.Set<Pelicula>(), "Id", "Titulo", funcion.PeliculaId);
            ViewData["SalaId"] = new SelectList(_context.Set<Sala>(), "Id", "Id", funcion.SalaId);
            return View(funcion);
        }

        // POST: Funciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fecha,Hora,Descripcion,ButacasDisponibles,Confirmada,PeliculaId,SalaId")] Funcion funcion)
        {
            if (id != funcion.Id)
            {
                return NotFound();
            }

            if (FuncionDuplicada(funcion))
            {
                ModelState.AddModelError(string.Empty, ErrorHelper.FuncionDuplicada);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funcion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FuncionExists(funcion.Id))
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
                    _exceptionHandler.ProcesarIndicesUnicos(ex, ModelState, "Fecha", ErrorHelper.FuncionDuplicada);
                    ViewData["PeliculaId"] = new SelectList(_context.Set<Pelicula>(), "Id", "Titulo", funcion.PeliculaId);
                    ViewData["SalaId"] = new SelectList(_context.Set<Sala>(), "Id", "Id", funcion.SalaId);
                    return View(funcion);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, ErrorHelper.ErrorGenerico(e));
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PeliculaId"] = new SelectList(_context.Set<Pelicula>(), "Id", "Titulo", funcion.PeliculaId);
            ViewData["SalaId"] = new SelectList(_context.Set<Sala>(), "Id", "Id", funcion.SalaId);
            return View(funcion);
        }

        // GET: Funciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcion = await _context.Funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funcion == null)
            {
                return NotFound();
            }

            return View(funcion);
        }

        // POST: Funciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var funcion = await _context.Funciones.FindAsync(id);

            if (funcion != null)
            {
                if (!await _context.Reservas.AnyAsync(r => r.FuncionId == id))
                {
                    _context.Funciones.Remove(funcion);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {

                    TempData["ErrorMessage"] = "No es posible eliminar una función con reservas";
                    return RedirectToAction(nameof(Delete), new { id });
                }
            }
            return NotFound();
        }

        private bool FuncionExists(int id)
        {
            return _context.Funciones.Any(e => e.Id == id);
        }

        private bool FuncionDuplicada(Funcion funcion)
        {
            if (funcion.Id != 0)
            {
                return _context.Funciones.Any(f =>
                        (funcion.Id != f.Id) && (
                        f.Fecha == funcion.Fecha &&
                        f.Hora == funcion.Hora &&
                        f.PeliculaId == funcion.PeliculaId &&
                        f.SalaId == funcion.SalaId)
                        );
            }
            else
            {
                return _context.Funciones.Any(f =>
                        f.Fecha == funcion.Fecha &&
                        f.Hora == funcion.Hora &&
                        f.PeliculaId == funcion.PeliculaId &&
                        f.SalaId == funcion.SalaId);
            }
        }

        private async Task<bool> FuncionPasada(int id)
        {
            bool pasada = false;
            if (FuncionExists(id))
            {
                var (horaActual, fechaActual) = DateTimeHelper.ObtenerInfoDateTime();
                Funcion funcion = await _context.Funciones.FindAsync(id);
                pasada = funcion.Fecha < fechaActual || (funcion.Fecha == fechaActual && funcion.Hora < horaActual);
            }
            return pasada;
        }
    }
}
