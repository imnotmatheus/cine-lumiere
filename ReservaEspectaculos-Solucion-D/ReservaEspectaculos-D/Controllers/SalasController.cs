using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservaEspectaculos_D.Data;
using ReservaEspectaculos_D.Models;
using ReservaEspectaculos_D.Models.ViewModels;
using ReservaEspectaculos_D.Utils;

namespace ReservaEspectaculos_D.Controllers
{
    [Authorize(Roles = "Empleado, Administrador")]

    public class SalasController : Controller
    {
        private readonly ReservaEspectaculosDb _context;
        private readonly ExceptionHandler _exceptionHandler;
        public SalasController(ReservaEspectaculosDb context, ExceptionHandler exceptionHandler)
        {
            _context = context;
            _exceptionHandler = exceptionHandler;
        }

        // GET: Salas
        public async Task<IActionResult> Index()
        {
            var reservaEspectaculosDb = _context.Salas.Include(s => s.TipoSala);
            return View(await reservaEspectaculosDb.ToListAsync());
        }

        // GET: Salas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context.Salas
                .Include(s => s.TipoSala)
                .Include(s => s.Funciones)
                .ThenInclude(f => f.Pelicula)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sala == null)
            {
                return NotFound();
            }

            return View(sala);
        }

        // GET: Salas/Create
        public IActionResult Create()
        {
            ViewData["TipoSalaId"] = new SelectList(_context.Set<TipoSala>(), "Id", "Nombre");
            return View();
        }

        // POST: Salas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Numero,CapacidadButacas,TipoSalaId")] Sala sala)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sala);

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch(DbUpdateException ex)
                {
                    _exceptionHandler.ProcesarIndicesUnicos(ex, ModelState, "Numero", ErrorHelper.Numero);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, ErrorHelper.ErrorGenerico(e));
                }
            }

            ViewData["TipoSalaId"] = new SelectList(_context.Set<TipoSala>(), "Id", "Nombre", sala.TipoSalaId);
            return View(sala);
        }

        // GET: Salas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context.Salas.FindAsync(id);
            if (sala == null)
            {
                return NotFound();
            }
            ViewData["TipoSalaId"] = new SelectList(_context.Set<TipoSala>(), "Id", "Nombre", sala.TipoSalaId);
            return View(sala);
        }

        // POST: Salas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Numero,CapacidadButacas,TipoSalaId")] Sala sala)
        {
            if (id != sala.Id)
            {
                return NotFound();
            }

            if (await SeModificoTipoSala(sala))
            {
                if (await VerificarReservasActivas(sala))
                {
                    ModelState.AddModelError(
                        "TipoSalaId",
                        "Esta sala tiene funciones activas, y no puede modificarse su tipo."
                        );
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sala);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaExists(sala.Id))
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
                    _exceptionHandler.ProcesarIndicesUnicos(ex, ModelState, "Numero", ErrorHelper.Numero);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, ErrorHelper.ErrorGenerico(e));
                }
            }

            ViewData["TipoSalaId"] = new SelectList(_context.Set<TipoSala>(), "Id", "Nombre", sala.TipoSalaId);
            return View(sala);
        }

        private bool SalaExists(int id)
        {
            return _context.Salas.Any(e => e.Id == id);
        }

        private async Task<bool> VerificarReservasActivas(Sala sala)
        {
            return await _context.Salas
                .Where(s => s.Id == sala.Id)
                .AnyAsync(s => s.Funciones.Any(f => f.Reservas.Any(
                    r => r.EstadoReserva == EstadoReserva.Activa
                    )
                ));
        }

        private async Task<bool> SeModificoTipoSala(Sala salaModificada)
        {
            var salaBD = await _context.Salas
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == salaModificada.Id);

            return salaBD.TipoSalaId != salaModificada.TipoSalaId;
        }
        public IActionResult NumeroDisponible(int numero)
        {
            if (_context.Salas.Any(s => s.Numero == numero))
            {
                return Json(ErrorHelper.SalaNumero);

            }

            return Json(true);
        }
        private bool SalaNumeroExists(Sala sala)
        {
            bool resultado = false;
        
                if (sala.Id != 0)
                {
                    resultado = _context.Salas.Any(s => s.Numero == sala.Numero && s.Id != sala.Id);
                }
                else
                {
                    resultado = _context.Salas.Any(s => s.Numero == sala.Numero);
                }
                return resultado;
        }
    }
}
