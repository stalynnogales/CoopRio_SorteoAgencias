using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using Web_Sorteo_Agencias.Config;
using Web_Sorteo_Agencias.Models;

namespace Web_Sorteo_Agencias.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        Repositorio _repos = new Repositorio();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            IEnumerable<Sucursal> Sucursales = _repos.ListadoSucursales();
            var listSucursales = Sucursales.Select(x => new SelectListItem
            {
                Text = $"{x.DESCRIPCION}",
                Value = x.SUCURSAL.ToString(),
                Disabled = x.SUCURSAL.Equals(0),
                Selected = x.SUCURSAL.Equals(0),
            });
            ViewBag.sucursales = listSucursales;
            ViewBag.sucursal = null;
            ViewBag.sociosseleccionados = null;
            ViewBag.totalsocios = 0;

            return View();
        }

        [HttpPost]
        public ActionResult sucursalSelected(int codigo)
        {
            var sucursal = _repos.BuscarSucursal(codigo);
            IEnumerable<Socio> sociosRegistrados = _repos.BuscarSociosSucursales(codigo);

            var result = new { sucursal, sociosRegistrados };
            return Json(result);
        }

        [HttpPost]
        public ActionResult seleccionAleatoria(int codigo)
        {
            var sucursal = _repos.ActualizarEstadosucursal(codigo);

            IEnumerable<Sucursal> Sucursales = _repos.ListadoSucursales();
            var listSucursales = Sucursales.Select(x => new SelectListItem
            {
                Text = $"{x.DESCRIPCION}",
                Value = x.SUCURSAL.ToString(),
                Disabled = x.SUCURSAL.Equals(0),
                Selected = x.SUCURSAL.Equals(codigo),
            });
            ViewBag.sucursales = listSucursales;
            ViewBag.sucursal = sucursal;

            IEnumerable<Socio> Socios = (IEnumerable<Socio>)_repos.ListadoSociosSucursal(codigo);
            ViewBag.sociosseleccionados = Socios;
            ViewBag.totalsocios = Socios.Count();

            return Json(Socios);
        }

        [HttpPost]
        public ActionResult guardarGanador(int codigo)
        {
            var correcto = _repos.GuardarGanadorAgencia(codigo);
            IEnumerable<Socio> Socios = _repos.BuscarSociosSucursales(codigo);
            return Json(Socios);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}