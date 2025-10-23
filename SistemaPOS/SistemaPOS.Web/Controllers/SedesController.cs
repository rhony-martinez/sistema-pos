using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using SistemaPOS.Application.Sedes;

namespace SistemaPOS.Web.Controllers
{
    public class SedesController : Controller
    {
        private readonly ListarSedesQuery _listar;
        private readonly CrearSedeCommand _crear;

        public SedesController(ListarSedesQuery listar, CrearSedeCommand crear)
        {
            _listar = listar;
            _crear = crear;
        }

        // GET: /Sedes
        public async Task<IActionResult> Index()
        {
            var res = await _listar.ExecuteAsync();
            var model = res.Success && res.Value != null
                ? res.Value
                : Enumerable.Empty<SistemaPOS.Application.Sedes.SedeDto>();
            return View(model);
        }

        // GET: /Sedes/Create
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new SedeCreateVm
            {
                CodigoUi = GenerarCodigoUi(),
                AdministradorResponsable = "Juan Pérez",
                Estado = "ACTIVA"
            };
            return View(vm);
        }

        // POST: /Sedes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SedeCreateVm vm)
        {
            // Validación mínima del lado servidor (campos obligatorios)
            if (string.IsNullOrWhiteSpace(vm.Nombre))
            {
                ModelState.AddModelError(nameof(vm.Nombre), "El nombre es obligatorio.");
                TempData["Modal"] = "invalid";
                vm.CodigoUi ??= GenerarCodigoUi();
                vm.AdministradorResponsable ??= "Juan Pérez";
                vm.Estado ??= "ACTIVA";
                return View(vm);
            }

            var result = await _crear.ExecuteAsync(
                vm.Nombre!, vm.Direccion, vm.Ciudad, vm.Departamento,
                vm.Ubicacion, vm.Telefono, vm.Correo, "ACTIVA");

            if (result.Success)
            {
                TempData["Modal"] = "success";
                vm.CodigoUi ??= GenerarCodigoUi();
                vm.AdministradorResponsable ??= "Juan Pérez";
                vm.Estado = "ACTIVA";
                ModelState.Clear();
                return View(vm); // muestra modal y redirige por JS/anchor
            }

            // Manejo de errores del sprint
            var err = result.Error ?? string.Empty;
            if (err.Equals(CrearSedeError.Duplicada.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                TempData["Modal"] = "duplicate";
            }
            else if (err.Equals(CrearSedeError.DatosInvalidos.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                TempData["Modal"] = "invalid";
                ModelState.AddModelError(nameof(vm.Correo), "Formato de correo no válido.");
            }
            else
            {
                TempData["Modal"] = "fail";
            }

            vm.CodigoUi ??= GenerarCodigoUi();
            vm.AdministradorResponsable ??= "Juan Pérez";
            vm.Estado ??= "ACTIVA";
            return View(vm);
        }

        private static string GenerarCodigoUi() => $"SN{DateTime.Now:HHmmss}";

        // ----------------- VIEWMODEL PARA CREATE -----------------
        public class SedeCreateVm
        {
            // Visual
            public string? CodigoUi { get; set; }
            public string AdministradorResponsable { get; set; } = "Juan Pérez";

            // Datos
            [Required, StringLength(100)]
            public string? Nombre { get; set; }

            [StringLength(150)]
            public string? Direccion { get; set; }

            [StringLength(80)]
            public string? Ciudad { get; set; } = "Cali";

            [StringLength(80)]
            public string? Departamento { get; set; } = "Valle del Cauca";

            [StringLength(100)]
            public string? Ubicacion { get; set; }

            [StringLength(20)]
            public string? Telefono { get; set; }

            [EmailAddress, StringLength(100)]
            public string? Correo { get; set; }

            public string? Estado { get; set; } = "ACTIVA";
        }
    }
}