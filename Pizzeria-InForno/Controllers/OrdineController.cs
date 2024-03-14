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
        // BACKOFFICE SECTION

        // GET Ordine/Index - Tutti gli ordini
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var ordini = await _db.Ordini.Include(o => o.DettagliOrdine).ThenInclude(d => d.Articoli).Include(o => o.Utenti).ToListAsync();

            return View(ordini);
        }

        // GET Ordine/Dettagli
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Dettagli(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ordine = _db.Ordini.Include(o => o.DettagliOrdine).ThenInclude(d => d.Articoli).Single(o => o.IdOrdine == id);
            if (ordine == null)
            {
                return NotFound();
            }
            return View(ordine);
        }
        // GET Ordine/Edit 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ordine = await _db.Ordini.FindAsync(id);
            if (ordine == null)
            {
                return NotFound();
            }
            return View(ordine);
        }

        // POST Ordine/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("IdOrdine, IdUtente, IndirizzoSpedizione, IsConsegnato, Note, Totale")] Ordini ordine)
        {
            if (id != ordine.IdOrdine)
            {
                return NotFound();
            }
            ModelState.Remove("Utenti");
            ModelState.Remove("DettagliOrdine");
            ModelState.Remove("DettagliIngredienti");
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(ordine);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdineExists(ordine.IdOrdine))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ordine);
        }

        private bool OrdineExists(int id)
        {
            return _db.Ordini.Any(e => e.IdOrdine == id);
        }

        // GET Ordine/Delete
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ordine = await _db.Ordini.FirstOrDefaultAsync(m => m.IdOrdine == id);
            if (ordine == null)
            {
                return NotFound();
            }
            return View(ordine);
        }

        // POST Ordine/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordine = await _db.Ordini.FindAsync(id);
            if (ordine != null)
            {
                _db.Ordini.Remove(ordine);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
