using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaEspectaculos_D.Data;
using ReservaEspectaculos_D.Models;
using ReservaEspectaculos_D.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservaEspectaculos_D.Controllers
{
    public class PrecargaController : Controller
    {
        private readonly ReservaEspectaculosDb _db;
        private readonly UserManager<Persona> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<Persona> _signInManager;
        private readonly PersonaHelper _personaHelper;
        private static readonly Random random = new Random();

        public PrecargaController(ReservaEspectaculosDb db, UserManager<Persona> userManager, RoleManager<IdentityRole<int>> roleManager, SignInManager<Persona> signInManager, PersonaHelper personaHelper)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _personaHelper = personaHelper;
        }

        public IActionResult Seed()
        {
            CrearRoles().Wait();
            CrearAdministrador().Wait();
            CrearEmpleado().Wait();
            CrearClientes().Wait();
            CrearTiposDeSalas().Wait();
            CrearGeneros().Wait();
            CrearSalas().Wait();
            CrearPeliculas().Wait();
            CrearFunciones().Wait();
            CrearReservas().Wait();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AgregarFuncionesTest()
        {
            CrearFunciones().Wait();
            return RedirectToAction("Index", "Home");
        }

        private async Task CrearRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole<int>("Administrador"));
            await _roleManager.CreateAsync(new IdentityRole<int>("Empleado"));
            await _roleManager.CreateAsync(new IdentityRole<int>("Cliente"));
        }

        private async Task CrearAdministrador()
        {
            Empleado administrador = new Empleado()
            {
                Nombre = "Admin",
                Apellido = "",
                DNI = "M0000000",
                Email = "admin@ort.edu.ar",
                UserName = "admin@ort.edu.ar"
            };

            var resultado = await _userManager.CreateAsync(administrador, Config.PasswordPorDefecto);

            if (resultado.Succeeded)
            {
                await _userManager.AddToRoleAsync(administrador, "Administrador");
            }
        }

        private async Task CrearEmpleado()
        {
            string legajo;

            do
            {
                legajo = _personaHelper.GenerarLegajo();
            }
            while (await _db.Empleados.AnyAsync(e => e.Legajo == legajo));

            Empleado empleado = new Empleado()
            {
                Legajo = legajo,
                Nombre = "Empleado",
                Apellido = "1",
                DNI = "11111111",
                Email = "empleado1@ort.edu.ar",
                UserName = "empleado1@ort.edu.ar",
            };

            var resultado = await _userManager.CreateAsync(empleado, Config.PasswordPorDefecto);

            if (resultado.Succeeded)
            {
                await _userManager.AddToRoleAsync(empleado, "Empleado");
            }
        }

        private async Task CrearClientes()
        {
            for (int i = 0; i < 20; i++)
            {
                string dni;
                if (i + 1 < 10)
                {
                    dni = "F000000";
                } else
                {
                    dni = "F00000";
                }
                Cliente cliente = new()
                {
                    Nombre = "Cliente",
                    Apellido = $"{i+1}",
                    DNI = dni + (i + 1),
                    Email = $"cliente{i+1}@ort.edu.ar",
                    UserName = $"cliente{i+1}@ort.edu.ar"
                };

                var resultado = await _userManager.CreateAsync(cliente, Config.PasswordPorDefecto);

                if (resultado.Succeeded)
                {
                    await _userManager.AddToRoleAsync(cliente, "Cliente");
                }
            }
        }

        private async Task CrearTiposDeSalas()
        {
            var tiposSala = new List<TipoSala>
    {
        new TipoSala
        {
            Nombre = "Sala Standard",
            Precio = 2500,
        },
        new TipoSala
        {
            Nombre = "Sala 3D",
            Precio = 3500,
        },
        new TipoSala
        {
            Nombre = "Sala IMAX",
            Precio = 4500,
        },
        new TipoSala
        {
            Nombre = "Sala VIP",
            Precio = 5500,
        },
        new TipoSala
        {
            Nombre = "Sala 4DX",
            Precio = 6000,
        }
    };

            _db.TipoSalas.AddRange(tiposSala);
            await _db.SaveChangesAsync();
        }

        private async Task CrearGeneros()
        {
            var generos = new List<Genero>
    {
        new Genero
        {
            Nombre = "Terror",
            Peliculas = new List<Pelicula>()
        },
        new Genero
        {
            Nombre = "Acción",
            Peliculas = new List<Pelicula>()
        },
        new Genero
        {
            Nombre = "Comedia",
            Peliculas = new List<Pelicula>()
        },
        new Genero
        {
            Nombre = "Drama",
            Peliculas = new List<Pelicula>()
        },
        new Genero
        {
            Nombre = "Ciencia Ficción",
            Peliculas = new List<Pelicula>()
        }
    };

            _db.Generos.AddRange(generos);
            await _db.SaveChangesAsync();
        }

        private async Task CrearSalas()
        {
            var tiposSala = await _db.TipoSalas.ToListAsync();

            var idSalaStandard = tiposSala.First(t => t.Nombre == "Sala Standard").Id;
            var idSala3D = tiposSala.First(t => t.Nombre == "Sala 3D").Id;
            var idSalaIMAX = tiposSala.First(t => t.Nombre == "Sala IMAX").Id;
            var idSalaVIP = tiposSala.First(t => t.Nombre == "Sala VIP").Id;
            var idSala4DX = tiposSala.First(t => t.Nombre == "Sala 4DX").Id;

            var salas = new List<Sala>
    {
        new Sala
        {
            Numero = 1,
            CapacidadButacas = 120,
            TipoSalaId = idSalaStandard,
            Funciones = new List<Funcion>()
        },
        new Sala
        {
            Numero = 2,
            CapacidadButacas = 100,
            TipoSalaId = idSalaStandard,
            Funciones = new List<Funcion>()
        },

        new Sala
        {
            Numero = 3,
            CapacidadButacas = 80,
            TipoSalaId = idSala3D,
            Funciones = new List<Funcion>()
        },
        new Sala
        {
            Numero = 4,
            CapacidadButacas = 90,
            TipoSalaId = idSala3D,
            Funciones = new List<Funcion>()
        },

        new Sala
        {
            Numero = 5,
            CapacidadButacas = 200,
            TipoSalaId = idSalaIMAX,
            Funciones = new List<Funcion>()
        },
        new Sala
        {
            Numero = 6,
            CapacidadButacas = 180,
            TipoSalaId = idSalaIMAX,
            Funciones = new List<Funcion>()
        },
        new Sala
        {
            Numero = 7,
            CapacidadButacas = 40,
            TipoSalaId = idSalaVIP,
            Funciones = new List<Funcion>()
        },
        new Sala
        {
            Numero = 8,
            CapacidadButacas = 45,
            TipoSalaId = idSalaVIP,
            Funciones = new List<Funcion>()
        },
        new Sala
        {
            Numero = 9,
            CapacidadButacas = 60,
            TipoSalaId = idSala4DX,
            Funciones = new List<Funcion>()
        },
        new Sala
        {
            Numero = 10,
            CapacidadButacas = 65,
            TipoSalaId = idSala4DX,
            Funciones = new List<Funcion>()
        }
    };

            _db.Salas.AddRange(salas);
            await _db.SaveChangesAsync();
        }

        private async Task CrearPeliculas()
        {
            var generos = await _db.Generos.ToListAsync();

            var idTerror = generos.First(g => g.Nombre == "Terror").Id;
            var idAccion = generos.First(g => g.Nombre == "Acción").Id;
            var idComedia = generos.First(g => g.Nombre == "Comedia").Id;
            var idDrama = generos.First(g => g.Nombre == "Drama").Id;
            var idCienciaFiccion = generos.First(g => g.Nombre == "Ciencia Ficción").Id;

            var peliculas = new List<Pelicula>
    {
        new Pelicula
        {
            Titulo = "El Conjuro",
            FechaLanzamiento = new DateOnly(2013, 07, 15),
            Descripcion = "Basada en hechos reales, narra una aterradora historia sobre una pareja de parapsicólogos Lorraine y Ed Warren.",
            GeneroId = idTerror,
            Funciones = new List<Funcion>(),
            PathCartel = Config.RutaImagenPelicula + "el-conjuro.jpg"
        },
        new Pelicula
        {
            Titulo = "Hereditary",
            FechaLanzamiento = new DateOnly(2018, 06, 21),
            Descripcion = "Cuando la matriarca de los Graham fallece, su hija y sus nietos comienzan a desentrañar secretos crípticos y terroríficos.",
            GeneroId = idTerror,
            Funciones = new List<Funcion>(),
            PathCartel = Config.RutaImagenPelicula + "hereditary.jpg"
        },

        new Pelicula
        {
            Titulo = "John Wick 4",
            FechaLanzamiento = new DateOnly(2023, 03, 24),
            Descripcion = "John Wick descubre un camino para derrotar a La Mesa. Pero debe enfrentarse a un nuevo enemigo con poderosas alianzas.",
            GeneroId = idAccion,
            Funciones = new List<Funcion>(),
            PathCartel = Config.RutaImagenPelicula + "john-wick.jpg"
        },
        new Pelicula
        {
            Titulo = "Misión Imposible: Sentencia Mortal",
            FechaLanzamiento = new DateOnly(2023, 07, 14),
            Descripcion = "Ethan Hunt y su equipo del FMI se embarcan en su misión más peligrosa hasta la fecha.",
            GeneroId = idAccion,
            Funciones = new List<Funcion>(),
            PathCartel = Config.RutaImagenPelicula + "mision-imposible.jpg"
        },

        new Pelicula
        {
            Titulo = "Super Cool",
            FechaLanzamiento = new DateOnly(2007, 08, 17),
            Descripcion = "Dos amigos inseparables del instituto viven una aventura épica cuando intentan comprar alcohol para una fiesta.",
            GeneroId = idComedia,
            Funciones = new List<Funcion>(),
            PathCartel = Config.RutaImagenPelicula + "supercool.jpg"
        },
        new Pelicula
        {
            Titulo = "¿Qué Pasó Ayer?",
            FechaLanzamiento = new DateOnly(2009, 06, 05),
            Descripcion = "Cuatro amigos viajan a Las Vegas para una despedida de soltero que no olvidarán... si logran recordarla.",
            GeneroId = idComedia,
            Funciones = new List<Funcion>(),
            PathCartel = Config.RutaImagenPelicula + "que-paso-ayer.jpg"
        },

        new Pelicula
        {
            Titulo = "El Padrino",
            FechaLanzamiento = new DateOnly(1972, 03, 14),
            Descripcion = "La historia de la familia Corleone, una de las más poderosas dinastías de la mafia italiana en Nueva York.",
            GeneroId = idDrama,
            Funciones = new List<Funcion>(),
            PathCartel = Config.RutaImagenPelicula + "el-padrino.jpg"
        },
        new Pelicula
        {
            Titulo = "Forrest Gump",
            FechaLanzamiento = new DateOnly(1994, 07, 06),
            Descripcion = "La historia de un hombre sureño con un bajo coeficiente intelectual que vive una serie de aventuras extraordinarias.",
            GeneroId = idDrama,
            Funciones = new List<Funcion>(),
            PathCartel = Config.RutaImagenPelicula + "forrest-gump.jpg"
        },

        new Pelicula
        {
            Titulo = "Interestelar",
            FechaLanzamiento = new DateOnly(2014, 11, 07),
            Descripcion = "Un grupo de exploradores emprende una misión interestelar para encontrar un nuevo hogar para la humanidad.",
            GeneroId = idCienciaFiccion,
            Funciones = new List<Funcion>(),
            PathCartel = Config.RutaImagenPelicula + "interestelar.jpg"
        },
        new Pelicula
        {
            Titulo = "Matrix",
            FechaLanzamiento = new DateOnly(1999, 03, 31),
            Descripcion = "Un programador descubre que la realidad en la que vive es una simulación creada por máquinas.",
            GeneroId = idCienciaFiccion,
            Funciones = new List<Funcion>(),
            PathCartel = Config.RutaImagenPelicula + "matrix.jpg"
        }
    };

            _db.Peliculas.AddRange(peliculas);
            await _db.SaveChangesAsync();
        }

        private async Task CrearFunciones()
        {
            var peliculas = await _db.Peliculas.ToListAsync();
            var salas = await _db.Salas.ToListAsync();

            var horarios = new TimeOnly[]
            {
                new(14, 30), // 14:30 hs
                new(17, 30), // 17:30 hs
                new(20, 30)  // 20:30 hs
            };

            // rango de dias para crear funciones en el pasado y futuro
            int rangoDias = 8;

            var fechaInicial = DateOnly.FromDateTime(DateTime.Now).AddDays(rangoDias * -1);

            foreach (var pelicula in peliculas)
            {
                // Para el mes anterior y actual
                for (int dia = 0; dia < (rangoDias * 2); dia++)
                {
                    var fecha = fechaInicial.AddDays(dia);

                    // 3 funciones por día
                    foreach (var hora in horarios)
                    {
                        var salaAleatoria = salas[random.Next(0, salas.Count)];

                        var funcion = new Funcion
                        {
                            PeliculaId = pelicula.Id,
                            SalaId = salaAleatoria.Id,
                            Fecha = fecha,
                            Hora = hora,
                            Descripcion = $"Función de {pelicula.Titulo}",
                            Confirmada = true,
                            Reservas = new List<Reserva>()
                        };

                        _db.Funciones.Add(funcion);
                    }
                }
            }

            await _db.SaveChangesAsync();
        }

        private async Task CrearReservas()
        {
            IEnumerable<Cliente> clientes = await _db.Clientes.ToListAsync();
            IEnumerable<Funcion> funciones = await _db.Funciones.ToListAsync();
            var fechaActual = DateOnly.FromDateTime(DateTime.Now);

            // crear reservas pasadas para testeo de recaudacion etc
            foreach (Cliente cliente in clientes)
            {
                foreach (Funcion funcion in funciones.Where(f => f.Fecha < fechaActual))
                {
                    // hacer reservas aleatorias en el pasado para todos los clientes y funciones
                    if (random.Next(0, 100) < 30)
                    {
                        Reserva reserva = new()
                        {
                            // 1 o 2 butacas por cliente
                            CantidadButacas = random.Next(1, 3),
                            ClienteId = cliente.Id,
                            FuncionId = funcion.Id,
                            EstadoReserva = EstadoReserva.Activa
                        };
                            _db.Reservas.Add(reserva);
                    }
                }
            }

            // crear cinco reservas futuras
            for (int i = 1; i < 6; i++)
            {
                var cliente = clientes.ElementAt(clientes.Count() - i);
                // agrego un dia a la fecha para testear cancelar la reserva
                var funcion = funciones.Where(f => f.Fecha >= fechaActual.AddDays(1)).FirstOrDefault();

                Reserva reserva = new()
                {
                    CantidadButacas = random.Next(1, 3),
                    ClienteId = cliente.Id,
                    FuncionId = funcion.Id,
                    EstadoReserva = EstadoReserva.Activa
                };
                _db.Reservas.Add(reserva);
            }


            await _db.SaveChangesAsync();
        }
    }
}
