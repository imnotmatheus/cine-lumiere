using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaEspectaculos_D.Data;
using ReservaEspectaculos_D.Models;
using ReservaEspectaculos_D.Models.ViewModels;
using ReservaEspectaculos_D.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReservaEspectaculos_D.Controllers
{
    public class AccountController : Controller
    {
        private readonly ReservaEspectaculosDb _contexto;
        private readonly UserManager<Persona> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<Persona> _signInManager;
        private readonly PersonaHelper _personaHelper;
        private readonly ExceptionHandler _exceptionHandler;

        public AccountController(ReservaEspectaculosDb contexto,
               UserManager<Persona> userManager,
               RoleManager<IdentityRole<int>> roleManager,
               SignInManager<Persona> signInManager,
               PersonaHelper personaHelper,
               ExceptionHandler exceptionHandler)
        {
            this._contexto = contexto;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
            this._personaHelper = personaHelper;
            this._exceptionHandler = exceptionHandler;
        }

        public IActionResult RegistroCliente()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistroCliente(RegistrarUsuario nuevoCliente)
        {
            if (EmailExiste(nuevoCliente.Email))
            {
                ModelState.AddModelError("Email", ErrorHelper.Email);
            }

            if (ModelState.IsValid)
            {
                Cliente cliente = new()
                {
                    Email = nuevoCliente.Email,
                    UserName = nuevoCliente.Email,
                    Nombre = nuevoCliente.Nombre,
                    Apellido = nuevoCliente.Apellido,
                    DNI = nuevoCliente.DNI,
                    Direccion = nuevoCliente.Direccion,
                    Telefono = nuevoCliente.Telefono
                };

                try
                {
                    var resultadoCreate = await _userManager.CreateAsync(cliente, nuevoCliente.Password);

                    if (resultadoCreate.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(cliente, "Cliente");
                        await _signInManager.SignInAsync(cliente, nuevoCliente.RememberMe);

                        return RedirectToAction("CheckIn", "Clientes");
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

            return View();
        }

        [Authorize(Roles = "Empleado, Administrador")]
        public IActionResult RegistrarNuevoEmpleado()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Empleado, Administrador")]
        public async Task<IActionResult> RegistrarNuevoEmpleado(RegistrarEmpleado nuevoEmpleado)
        {
            string legajo;

            do
            {
                legajo = _personaHelper.GenerarLegajo();
            }
            while (await _contexto.Empleados.AnyAsync(e => e.Legajo == legajo));

            if (EmailExiste(nuevoEmpleado.Email))
            {
                ModelState.AddModelError("Email", ErrorHelper.Email);
            }

            if (ModelState.IsValid)
            {
                Empleado empleado = new()
                {
                    Email = nuevoEmpleado.Email,
                    UserName = nuevoEmpleado.Email,
                    Telefono = nuevoEmpleado.Telefono,
                    DNI = nuevoEmpleado?.DNI,
                    Nombre = nuevoEmpleado?.Nombre,
                    Apellido = nuevoEmpleado?.Apellido,
                    Direccion = nuevoEmpleado?.Direccion,
                    Legajo = legajo
                };

                var resultadoCreate = await _userManager.CreateAsync(empleado, Config.PasswordPorDefecto);

                if (resultadoCreate.Succeeded)
                {
                    await _userManager.AddToRoleAsync(empleado, "Empleado");
                    return RedirectToAction("Index", "Empleados");
                }

                foreach (var error in resultadoCreate.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
            }

            return View();
        }

        public IActionResult IniciarSesion(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(Login modelo)
        {
            string ReturnUrl = TempData["ReturnUrl"] as string;

            if (ModelState.IsValid)
            {
                var resultadoInicioSesion = await _signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.RememberMe, false);

                if (resultadoInicioSesion.Succeeded)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl)) { return Redirect(ReturnUrl); }
                    if (User.IsInRole("Cliente")) { return RedirectToAction("CheckIn", "Clientes"); }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Inicio de sesión inválido.");
            }

            return View(modelo);
        }

        [Authorize]
        public async Task<IActionResult> CerrarSesion()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccesoDenegado(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public async Task<IActionResult> EmailDisponible(string email)
        {
            var persona = await _userManager.FindByEmailAsync(email);

            if (persona != null)
            {
                return Json(ErrorHelper.Email);

            }

            return Json(true);
        }

        private bool EmailExiste(string email)
        {
            bool resultado = false;

            if (!string.IsNullOrEmpty(email))
            {
                resultado = _contexto.Personas.Any(p => p.Email == email);
            }

            return resultado;
        }
    }
}
