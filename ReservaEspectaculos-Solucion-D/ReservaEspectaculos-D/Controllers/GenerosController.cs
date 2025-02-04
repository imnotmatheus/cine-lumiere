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
using ReservaEspectaculos_D.Utils;

namespace ReservaEspectaculos_D.Controllers
{
    [Authorize(Roles = "Empleado, Administrador")]

    public class GenerosController : Controller
    {
        private readonly ReservaEspectaculosDb _context;
        private readonly ExceptionHandler _exceptionHandler;

        public GenerosController(ReservaEspectaculosDb context, ExceptionHandler exceptionHandler)
        {
            _context = context;
            _exceptionHandler = exceptionHandler;
        }

        // GET: Generos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Generos.ToListAsync());
        }

        // GET: Generos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = await _context.Generos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genero == null)
            {
                return NotFound();
            }

            return View(genero);
        }

        // GET: Generos/Create
        public IActionResult Create()
        {
            Genero genero = new Genero();
            genero.Nombre = "Aventura";
            return View(genero);
        }

        // POST: Generos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] Genero genero)
        {
            if (GeneroNombreExists(genero))
            {
                ModelState.AddModelError("Nombre", ErrorHelper.Nombre);
            }
            if (ModelState.IsValid)
            {
                _context.Add(genero);

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

            return View(genero);
        }

        // GET: Generos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = await _context.Generos.FindAsync(id);
            if (genero == null)
            {
                return NotFound();
            }
            return View(genero);
        }

        // POST: Generos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] Genero genero)
        {
            if (GeneroNombreExists(genero))
            {
                ModelState.AddModelError("Nombre", ErrorHelper.Nombre);
            }
            if (id != genero.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genero);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeneroExists(genero.Id))
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

            return View(genero);
        }

        // GET: Generos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = await _context.Generos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genero == null)
            {
                return NotFound();
            }

            return View(genero);
        }

        // POST: Generos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genero = await _context.Generos.FindAsync(id);
            if (genero != null)
            {
                _context.Generos.Remove(genero);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GeneroExists(int id)
        {
            return _context.Generos.Any(e => e.Id == id);
        }
        public IActionResult NombreDisponible(string nombre)
        {
            if (_context.Generos.Any(g => g.Nombre == nombre))
            {
                return Json(ErrorHelper.Nombre);

            }

            return Json(true);
        }
        private bool GeneroNombreExists(Genero genero)
        {
            bool resultado = false;
            if (string.IsNullOrEmpty(genero.Nombre))
            {
                if (genero.Id != 0)
                {
                    resultado = _context.Generos.Any(g => g.Nombre == genero.Nombre && g.Id != genero.Id);
                }
                else
                {
                    resultado = _context.Generos.Any(g => g.Nombre == genero.Nombre);
                }
            }
            return resultado;
        }
    }
}
