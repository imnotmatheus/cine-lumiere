using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservaEspectaculos_D.Data;
using ReservaEspectaculos_D.Models;
using ReservaEspectaculos_D.Models.ViewModels;
using ReservaEspectaculos_D.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReservaEspectaculos_D.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly ReservaEspectaculosDb _context;
        private readonly UserManager<Persona> _userManager;

        public ReservasController(ReservaEspectaculosDb context,UserManager<Persona> userManager)
        {
            _context = context;
            this._userManager = userManager;
        }

        // GET: Reservas
        [Authorize(Roles = "Empleado, Administrador")]
        public async Task<IActionResult> Index(int? clienteId, int? peliculaId, int? salaId, DateOnly? fecha)
        {
            var reservas = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Funcion)
                .ToListAsync();

            if (clienteId != null)
            {
                if (PersonaHelper.ClienteExists((int)clienteId, _context))
                {
                    reservas = reservas.Where(r => r.ClienteId == clienteId).ToList();
                } else
                {
                    reservas = null;
                }
            }

            if (peliculaId != null && reservas != null)
            {
                if (PeliculaHelper.PeliculaExists((int)peliculaId, _context))
                {
                    reservas = reservas.Where(r => r.Funcion.PeliculaId == peliculaId).ToList();
                } else
                {
                    reservas = null;
                }
            }

            if (salaId != null && reservas != null)
            {
                if (SalaHelper.SalaExists((int)salaId, _context))
                {
                    reservas = reservas.Where(r => r.Funcion.SalaId == salaId).ToList();
                }
                else
                {
                    reservas = null;
                }
            }

            if (fecha != null && reservas != null)
            {
                reservas = reservas.Where(r => r.Funcion.Fecha == fecha).ToList();
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido");
            ViewData["PeliculaId"] = new SelectList(_context.Peliculas, "Id", "Titulo");
            ViewData["SalaId"] = new SelectList(_context.Salas
                                .Join(_context.Set<TipoSala>(),
                                sala => sala.TipoSalaId,
                                tipoSala => tipoSala.Id,
                                (sala, tipoSala) => new { sala.Id, DisplayText = $"{sala.Numero} - {tipoSala.Nombre}" }), "Id", "DisplayText");
            
            return View(reservas);
        }

        // GET: Reservas/Details/5
        [Authorize(Roles = "Empleado, Administrador, Cliente")]
        public async Task<IActionResult> Detalles(int? id)
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
            if (user is Cliente && user.Id != reserva.ClienteId)
            {
                return Unauthorized();
            }

            return View(reserva);
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
        public async Task<IActionResult> SeleccionButacas(int? peliculaId, int? funcionId)
        {
            int clienteId = -1;
            await ActualizarEstadoReservas();

            if (peliculaId == null)
            {
                return NotFound("No seleccionaste una película");
            }

            if (User.IsInRole("Cliente"))
            {
                clienteId = Int32.Parse(_userManager.GetUserId(User));
            }

            if (clienteId == -1)
            {
                return NotFound("No existe un cliente con ese Id");
            }

            if (await ClienteTieneReservaActiva(clienteId))
            {
                return BadRequest("Ya tenes una reserva activa.");
            }

            Pelicula pelicula = await _context.Peliculas.FindAsync(peliculaId);

            if (pelicula == null)
            {
                return NotFound("No se encontró esa película en cartelera");
            }

            Funcion funcion = null;
            FuncionEnIndex funcionEnIndex = null;

            if (funcionId != null) {

                funcion = await _context.Funciones
                    .Include(f => f.Reservas)
                    .Include(f => f.Sala)
                    .FirstOrDefaultAsync(f => f.Id == funcionId);

                if (funcion == null || !funcion.Confirmada)
                {
                    return NotFound("No se encontró una función con ese Id");
                }

                    funcionEnIndex = new()
                {
                    Funcion = funcion,
                    ButacasDisponibles = FuncionHelper.ButacasDisponibles(funcion)
                };

            }

            HacerReserva modelo = new()
            {
                PeliculaId = peliculaId.Value,
                ClienteId = clienteId,
                FuncionSeleccionadaId = funcionId,
                FuncionSeleccionada = funcionEnIndex,
            };

            return View(modelo);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> SeleccionFuncion(HacerReserva model)
        {
            if (await ClienteTieneReservaActiva(model.ClienteId))
            {
                return BadRequest("Ya tenes una reserva activa.");
            }

            if (model.CantidadButacas <= 0)
            {
                return BadRequest("Al menos debes reservar una butaca");
            }

            if (!PeliculaHelper.PeliculaExists(model.PeliculaId, _context))
            {
                return NotFound("No existe una película con ese Id.");
            }

            if (model.FuncionSeleccionadaId != null)
            {
                var funcion = await _context.Funciones
                             .Include(f => f.Reservas)
                             .Include(f => f.Sala)
                             .FirstOrDefaultAsync(f => f.Id == model.FuncionSeleccionadaId);
                              

                if (funcion == null)
                {
                    return NotFound("No se encontró una función con ese Id");
                }

                if (!ReservaHelper.VerificarDisponibilidad(funcion, model.CantidadButacas))
                {
                    return BadRequest("Funcion no tiene esta cantidad de butacas disponibles");
                }

                FuncionEnIndex funcionEnIndex = new()
                {
                    Funcion = funcion,
                    ButacasDisponibles = FuncionHelper.ButacasDisponibles(funcion)
                };

                model.FuncionSeleccionada = funcionEnIndex;

                return View(model);
            }

            var funciones = await ObtenerFuncionesPorPelicula(model.PeliculaId, model.CantidadButacas);

            List<FuncionEnIndex> funcionesEnIndex = [];
            foreach (var funcion in funciones)
            {
                FuncionEnIndex funcionEnIndex = new()
                {
                    Funcion = funcion,
                    ButacasDisponibles = FuncionHelper.ButacasDisponibles(funcion),
                };
                if (funcionEnIndex.ButacasDisponibles >= model.CantidadButacas) 
                    {
                    funcionesEnIndex.Add(funcionEnIndex);
                    }
            }

            model.Funciones = funcionesEnIndex; 

            return View(model);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> ConfirmarReserva(HacerReserva model, int? funcionId)
        {
            if (await ClienteTieneReservaActiva(model.ClienteId))
            {
                return BadRequest("Ya tenes una reserva activa.");
            }

            if (model.CantidadButacas <= 0)
            {
                return BadRequest("Al menos debes reservar una butaca");
            }

            if (funcionId == null)
            {
                return BadRequest("Función no válida");
            }

            Funcion funcion = await _context.Funciones
                .Include(f => f.Sala)
                    .ThenInclude(s => s.TipoSala)
                .Include(f => f.Pelicula)
                .FirstOrDefaultAsync(f => f.Id == funcionId.Value);

            if (funcion == null)
            {
                return NotFound("No se encontró una función con ese Id");
            }

            ConfirmarReserva confirmarReserva = new()
            {
                Pelicula = funcion.Pelicula,
                Fecha = funcion.Fecha,
                Hora = funcion.Hora,
                Sala = funcion.Sala,
                PrecioTotal = funcion.Sala.TipoSala.Precio * model.CantidadButacas,
                CantidadButacas = model.CantidadButacas,
                FuncionId = funcionId.Value,
                ClienteId = model.ClienteId,
                PeliculaId = funcion.Pelicula.Id,
                SalaId = funcion.Sala.Id
            };

            return View(confirmarReserva);
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> ConfirmarReserva(ConfirmarReserva model)
        {
            if (ModelState.IsValid)
            {
                Reserva reserva = new()
                {
                    CantidadButacas = model.CantidadButacas,
                    FechaAlta = DateTime.Now,
                    ClienteId = model.ClienteId,
                    FuncionId = model.FuncionId,
                    EstadoReserva = EstadoReserva.Activa
                };

                var funcion = await _context.Funciones.FindAsync(model.FuncionId);

                try
                {
                    _context.Update(funcion);
                    _context.Add(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ReservaExists(reserva.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, ErrorHelper.ErrorGenerico(e));
                }

                return RedirectToAction("MisReservas");
            }

            return View(model);
        }


        // GET: Reservas/Create
        [Authorize(Roles = "Cliente")]
        public IActionResult Create()
        {
            
            var peliculas = PeliculaHelper.ObtenerPeliculasEnExibicion(_context);
            ViewBag.peliculas = peliculas;
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido");

            return View();
        }


        [Authorize(Roles = "Empleado, Administrador, Cliente")]
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
            if (user is Cliente && user.Id != reserva.ClienteId)
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
        [HttpPost, ActionName("EliminarConfirmado")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Empleado, Administrador, Cliente")]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var reserva = await _context.Reservas
                    .Include(r => r.Funcion)
                    .FirstOrDefaultAsync(m => m.Id == id);

            var user = await _userManager.GetUserAsync(User);
            if (user is Cliente && user.Id != reserva.ClienteId)
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

            if (user is Cliente)
            {
                return RedirectToAction("MisReservas");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        private async Task<bool> ReservaExists(int id)
        {
            return await _context.Reservas.AnyAsync(e => e.Id == id);
        }

        private async Task<bool> ClienteTieneReservaActiva(int clienteId)
        {
            return await _context.Clientes
                .Where(c => c.Id == clienteId)
                .SelectMany(c => c.Reservas)
                .AnyAsync(r => r.EstadoReserva == EstadoReserva.Activa);
        }

        private async Task<List<Funcion>> ObtenerFuncionesPorPelicula(int peliculaId, int cantidadButacas)
        {
            var (horaActual, fechaActual) = DateTimeHelper.ObtenerInfoDateTime();

            var funciones = await _context.Funciones.Where(f =>
                    f.PeliculaId == peliculaId && f.Fecha <= fechaActual.AddDays(7) &&
                    (f.Fecha > fechaActual || (f.Fecha == fechaActual && f.Hora > horaActual)) &&
                    f.Confirmada)
                .Include(f => f.Reservas)
                .Include(f => f.Sala)
                .ThenInclude(s => s.TipoSala)
                .ToListAsync();


            return funciones.Where(f => FuncionHelper.ButacasDisponibles(f) >= cantidadButacas).ToList();
        }

        private async Task ActualizarEstadoReservas()
        {
            var reservas = await _context.Reservas.Include(r => r.Funcion).ToListAsync();

            foreach (Reserva reserva in reservas)
            {
                ReservaHelper.ActualizarEstadoReserva(reserva);
            }

            await _context.SaveChangesAsync();
        }
    }
}