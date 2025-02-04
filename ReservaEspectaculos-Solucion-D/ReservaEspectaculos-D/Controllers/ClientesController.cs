﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ReservaEspectaculos_D.Data;
using ReservaEspectaculos_D.Models;
using ReservaEspectaculos_D.Models.ViewModels;
using ReservaEspectaculos_D.Utils;

namespace ReservaEspectaculos_D.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ReservaEspectaculosDb _context;
        private readonly UserManager<Persona> _userManager;
        private readonly ExceptionHandler _exceptionHandler;

        public ClientesController(ReservaEspectaculosDb context, UserManager<Persona> userManager, ExceptionHandler exceptionHandler)
        {
            _context = context;
            _userManager = userManager;
            _exceptionHandler = exceptionHandler;
        }

        // GET: Clientes
        [Authorize(Roles = "Empleado, Administrador")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        // GET: Clientes/Details/5
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user.Id != id)
            {
                return Unauthorized();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> MisReservas()
        {
            var cliente = await _userManager.GetUserAsync(User);
            int id = cliente.Id;
            var reservas = _context.Reservas
                .Where(r => r.ClienteId == id)
                .ToList();

            foreach (Reserva r in reservas)
            {
                r.Funcion = _context.Funciones.Find(r.FuncionId);
                ReservaHelper.ActualizarEstadoReserva(r);
            }

            MisReservasCliente model = new()
            {
                ClienteId = id,
                //(f.Fecha > fechaActual || (f.Fecha == fechaActual && f.Hora > horaActual))
                ReservasVigentes = reservas.Where(r =>
                    r.EstadoReserva == EstadoReserva.Activa)
                .OrderBy(r => r.Funcion.Fecha),
                ReservasPasadas = reservas.Where(r =>
                    r.EstadoReserva == EstadoReserva.Inactiva)
                .OrderBy(r => r.Funcion.Fecha)
            }; ;
            return View(model);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> DetalleReserva(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Funcion)
                    .ThenInclude(f => f.Sala)
                        .ThenInclude(s => s.TipoSala)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user.Id != reserva.ClienteId)
            {
                return Unauthorized();
            }

            return View(reserva);
        }

        [Authorize(Roles = "Cliente")]
        // GET: Reservas/Delete/5
        public async Task<IActionResult> EliminarReserva(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Funcion)
                .FirstOrDefaultAsync(m => m.Id == id);

            var user = await _userManager.GetUserAsync(User);
            if (user.Id != reserva.ClienteId)
            {
                return Unauthorized();
            }

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("EliminarReserva")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> EliminarReservaConfirmado(int id)
        {
            var reserva = await _context.Reservas
                    .Include(r => r.Funcion)
                    .FirstOrDefaultAsync(m => m.Id == id);


            var user = await _userManager.GetUserAsync(User);
            if (user.Id != reserva.ClienteId)
            {
                return Unauthorized();
            }

            DateTime fechaReserva = reserva.Funcion.Fecha.ToDateTime(reserva.Funcion.Hora);
            if (fechaReserva.AddHours(-24) < DateTime.Now)
            {
                TempData["ErrorMessage"] = "No es posible eliminar reserva 24hs previas a la función";
                return RedirectToAction(nameof(EliminarReserva), new { id });

            }

            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("MisReservas", "Clientes");
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,DNI,Telefono,Direccion,Email,UserName,FechaAlta")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.FechaAlta = DateTime.Now;
                _context.Add(cliente);

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                } catch(DbUpdateException ex)
                {
                    _exceptionHandler.ProcesarIndicesUnicos(ex, ModelState, "Email", ErrorHelper.Email);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, ErrorHelper.ErrorGenerico(e));
                }
            }

            return View(cliente);
        }

        // GET: Clientes/Edit/5
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // no permitir que el cliente cambie la info de otro cliente
            var user = await _userManager.GetUserAsync(User);

            if (user.Id != id)
            {
                return Unauthorized();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            CambioPerfil perfil = new CambioPerfil()
            {
                PersonaId = cliente.Id,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                DNI = cliente.DNI,
                Telefono = cliente.Telefono,
                Direccion = cliente.Direccion,
                Email = cliente.Email
            };
            return View(perfil);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Edit(int? id, [Bind("PersonaId,Nombre,Apellido,Telefono,Direccion")] CambioPerfil perfil)
        {

            if (id == null)
            {
                return BadRequest();
            }
        
            if (id != perfil.PersonaId)
            {
                return Unauthorized();
            }

            var cliente = await _context.Clientes.FindAsync(perfil.PersonaId);

            if (cliente == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    cliente.Telefono = perfil.Telefono;
                    cliente.Direccion = perfil.Direccion;

                    _context.Update(cliente);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(PerfilGuardado));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
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
                    _exceptionHandler.ProcesarIndicesUnicos(ex, ModelState, "Email", ErrorHelper.Email);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, ErrorHelper.ErrorGenerico(e));
                }
            }

            return View(cliente);
        }

        public IActionResult PerfilGuardado()
        {
            return View();
        }

        // GET: Clientes/Delete/5
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Cliente")]
        public IActionResult CheckIn()
        {
            return View();
        }
    }
}
