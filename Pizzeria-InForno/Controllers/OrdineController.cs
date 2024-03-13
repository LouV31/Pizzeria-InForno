using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pizzeria_InForno.Data;
using Pizzeria_InForno.Models;
using System.Security.Claims;

namespace Pizzeria_InForno.Controllers
{
    public class OrdineController : Controller
    {
        private readonly ApplicationDbContext _db;
        public OrdineController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "user, admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "user,admin")]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("IdUtente, IndirizzoSpedizione, isConsegnato, note")] Ordini ordine)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return RedirectToAction("Login", "Index");
            }

            ordine.IdUtente = int.Parse(userId);

            // Prendi il carrello dalla sessione
            var cartSession = HttpContext.Session.GetString("cartList");
            if (!string.IsNullOrEmpty(cartSession))
            {
                var cartList = JsonConvert.DeserializeObject<List<Carrello>>(cartSession);
                double prezzoTotale = 0;
                foreach (var item in cartList)
                {

                    var articolo = _db.Articoli.Include(a => a.DettagliIngrediente).ThenInclude(di => di.Ingredienti).Single(a => a.IdArticolo == item.Articoli.IdArticolo);
                    var prezzoIngredienti = articolo.DettagliIngrediente.Sum(di => di.Ingredienti.Prezzo);
                    prezzoTotale += (articolo.Prezzo + prezzoIngredienti) * item.Quantita;
                }

                // Imposta il prezzo dell'ordine
                ordine.Totale = prezzoTotale;

                _db.Ordini.Add(ordine);
                await _db.SaveChangesAsync();

                foreach (var item in cartList)
                {
                    var dettagliOrdine = new DettagliOrdine
                    {
                        IdOrdine = ordine.IdOrdine,
                        IdArticolo = item.Articoli.IdArticolo,
                        Quantita = item.Quantita
                    };
                    _db.DettagliOrdine.Add(dettagliOrdine);
                }
                await _db.SaveChangesAsync();
                HttpContext.Session.Remove("cartList");
                TempData["success"] = "Ordine effettuato con successo";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
