using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizzeria_InForno.Data;
using Pizzeria_InForno.Models;
using System.Diagnostics;

namespace Pizzeria_InForno.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            var articoli = _db.Articoli.Include(a => a.DettagliIngrediente).ThenInclude(di => di.Ingredienti).ToList();

            foreach (var articolo in articoli)
            {
                articolo.Prezzo += articolo.DettagliIngrediente.Sum(di => di.Ingredienti.Prezzo);
            }

            return View(articoli);
        }

        [AllowAnonymous]
        public IActionResult WelcomePage()
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
