using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservaEspectaculos_D.Data;
using ReservaEspectaculos_D.Models;
using ReservaEspectaculos_D.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReservaEspectaculos_D.Controllers
{
    [Authorize(Roles = "Empleado, Administrador")]

    public class TipoSalasController : Controller
    {
        private readonly ReservaEspectaculosDb _context;
        private readonly ExceptionHandler _exceptionHandler;

        public TipoSalasController(ReservaEspectaculosDb context, ExceptionHandler exceptionHandler)
        {
            _context = context;
            _exceptionHandler = exceptionHandler;
        }

        // GET: TipoSalas
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoSalas.ToListAsync());
        }

        // GET: TipoSalas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoSala = await _context.TipoSalas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoSala == null)
            {
                return NotFound();
            }

            return View(tipoSala);
        }

        // GET: TipoSalas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoSalas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Precio")] TipoSala tipoSala)
        {
            if (TipoSalaNombreExists(tipoSala))
            {
                ModelState.AddModelError("Nombre", ErrorHelper.Nombre);
            }
            if (ModelState.IsValid)
            {
                _context.Add(tipoSala);

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _exceptionHandler.ProcesarIndicesUnicos(ex, ModelState, "Nombre", ErrorHelper.Nombre);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, ErrorHelper.ErrorGenerico(e));
                }
            }

            return View(tipoSala);
        }

        // GET: TipoSalas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {   
            if (id == null)
            {
                return NotFound();
            }

            var tipoSala = await _context.TipoSalas.FindAsync(id);
            if (tipoSala == null)
            {
                return NotFound();
            }
            return View(tipoSala);
        }

        // POST: TipoSalas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Precio")] TipoSala tipoSala)
        {
            if (TipoSalaNombreExists(tipoSala))
            {
                ModelState.AddModelError("Nombre", ErrorHelper.Nombre);
            } 

            if (id != tipoSala.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoSala);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoSalaExists(tipoSala.Id))
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
                    _exceptionHandler.ProcesarIndicesUnicos(ex, ModelState, "Nombre", ErrorHelper.Nombre);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, ErrorHelper.ErrorGenerico(e));
                }
            }

            return View(tipoSala);
        }

        // GET: TipoSalas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoSala = await _context.TipoSalas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoSala == null)
            {
                return NotFound();
            }

            return View(tipoSala);
        }

        // POST: TipoSalas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoSala = await _context.TipoSalas.FindAsync(id);
            if (tipoSala != null)
            {
                _context.TipoSalas.Remove(tipoSala);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoSalaExists(int id)
        {
            return _context.TipoSalas.Any(e => e.Id == id);
        }
        public IActionResult NombreDisponible(string nombre, int? id)
        {
            if (_context.TipoSalas.Any(ts => ts.Nombre == nombre && ts.Id != id))
            {
                return Json(ErrorHelper.Nombre);

            }

            return Json(true);
        }

        private bool TipoSalaNombreExists(TipoSala tipoSala)
        {
            bool resultado = false;
            if (string.IsNullOrEmpty(tipoSala.Nombre))
            {
                if (tipoSala.Id != 0)
                {
                    resultado = _context.TipoSalas.Any(g => g.Nombre == tipoSala.Nombre && g.Id != tipoSala.Id);
                }
                else
                {
                    resultado = _context.TipoSalas.Any(g => g.Nombre == tipoSala.Nombre);
                }
            }
            return resultado;
        }
    }
}
