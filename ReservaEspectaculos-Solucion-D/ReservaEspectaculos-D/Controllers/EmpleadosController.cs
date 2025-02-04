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
using ReservaEspectaculos_D.Models.ViewModels;

namespace ReservaEspectaculos_D.Controllers
{
    [Authorize(Roles = "Empleado, Administrador")]
    public class EmpleadosController : Controller
    {
        private readonly ReservaEspectaculosDb _context;
        private readonly ExceptionHandler _exceptionHandler;

        public EmpleadosController(ReservaEspectaculosDb context, ExceptionHandler exceptionHandler)
        {
            _context = context;
            _exceptionHandler = exceptionHandler;
        }

        // GET: Empleados
        public async Task<IActionResult> Index()
        {
            return View(await _context.Empleados.ToListAsync());
        }

        // GET: Empleados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // GET: Empleados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empleados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Legajo,Id,Nombre,Apellido,DNI,Telefono,Direccion,Email,UserName,Password,FechaAlta")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                empleado.FechaAlta = DateTime.Now;
                _context.Add(empleado);

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _exceptionHandler.ProcesarIndicesUnicos(ex, ModelState, "Legajo", ErrorHelper.Legajo);
                }
            }
            return View(empleado);
        }

        // GET: Empleados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            CambioPerfil perfil = new ()
            {
                PersonaId = empleado.Id,
                Nombre = empleado.Nombre,
                Apellido = empleado.Apellido,
                DNI = empleado.DNI,
                Telefono = empleado.Telefono,
                Direccion = empleado.Direccion,
                Email = empleado.Email
            };

            ViewBag.Legajo = empleado.Legajo;
            return View(perfil);
        }

        // POST: Empleados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("PersonaId,Nombre,Apellido,DNI,Telefono,Direccion")] CambioPerfil perfil)
        {
            if (id != perfil.PersonaId)
            {
                return Unauthorized();
            }

            if (id == null)
            {
                return BadRequest();
            }

            var empleado = await _context.Empleados.FindAsync(perfil.PersonaId);

            if (empleado == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    empleado.Nombre = perfil.Nombre;
                    empleado.Apellido = perfil.Apellido;
                    empleado.DNI = perfil.DNI;
                    empleado.Telefono = perfil.Telefono;
                    empleado.Direccion = perfil.Direccion;

                    _context.Update(empleado);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleado.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(perfil);
        }

        // GET: Empleados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado != null)
            {
                _context.Empleados.Remove(empleado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.Id == id);
        }
    }
}
