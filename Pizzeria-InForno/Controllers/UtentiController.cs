using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizzeria_InForno.Data;
using Pizzeria_InForno.Models;
using System.Security.Claims;

namespace Pizzeria_InForno.Controllers
{
    public class UtentiController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UtentiController(ApplicationDbContext db)
        {

            _db = db;

        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        // Elimino dal bind di utente l'attributo Ruolo
        public IActionResult SignUp([Bind("Username, Password")] Utenti utente)
        {
            // Rimuovo l'errore relativo all'attributo ordini non passandolo al model
            ModelState.Remove("ordini");
            // Faccio un check del model, se è valido proseguo
            if (ModelState.IsValid)
            {
                // Controllo se c'è già un utente registrato con lo stesso username
                if (_db.Utenti.Any(u => u.Username == utente.Username))
                {
                    TempData["error"] = "Nome utente già in uso";
                    return View(utente);
                }
                // Se non c'è un utente con lo stesso username procedo con la registrazione e faccio una insert nel DB
                _db.Utenti.Add(utente);
                _db.SaveChanges();
                TempData["success"] = "Registrazione avvenuta con successo";
                return RedirectToAction("Index", "Login");

            }
            else
            {
                TempData["error"] = "Errore nella registrazione";
                return View(utente);
            }
        }

        // Get Utente/Ordine/RiepilogoOrdini
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> RiepilogoOrdini()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var ordini = await _db.Ordini.Include(o => o.DettagliOrdine).ThenInclude(d => d.Articoli).Where(o => o.IdUtente == int.Parse(userId)).ToListAsync();
            return View(ordini);
        }

        // GET Utente/Ordine/DettagliOrdine
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> DettagliOrdine(int? idOrdine)
        {
            if (idOrdine == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var ordine = await _db.Ordini.Include(o => o.DettagliOrdine).ThenInclude(d => d.Articoli).ThenInclude(a => a.DettagliIngrediente).ThenInclude(di => di.Ingredienti).FirstOrDefaultAsync(o => o.IdOrdine == idOrdine && o.IdUtente == int.Parse(userId));
            if (ordine == null)
            {
                return RedirectToAction("RiepilogoOrdini");
            }
            foreach (var dettaglio in ordine.DettagliOrdine)
            {
                var prezzoArticolo = dettaglio.Articoli.Prezzo + dettaglio.Articoli.DettagliIngrediente.Sum(di => di.Ingredienti.Prezzo);
                dettaglio.Articoli.Prezzo = prezzoArticolo * dettaglio.Quantita;

                dettaglio.Articoli.Immagine = Path.Combine("/", dettaglio.Articoli.Immagine);
            }
            return View(ordine);
        }

        // GET Utente/Ordine/DeleteOrdine   
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> DeleteOrdine(int? idOrdine)
        {
            if (idOrdine == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var ordine = await _db.Ordini.Include(o => o.DettagliOrdine).FirstOrDefaultAsync(o => o.IdOrdine == idOrdine && o.IdUtente == int.Parse(userId));
            if (ordine == null)
            {
                return RedirectToAction("RiepilogoOrdini");
            }

            return View(ordine);
        }

        // POST Utente/Ordine/DeleteOrdine
        [HttpPost, ActionName("DeleteOrdine")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "user, admin")]
        public async Task<IActionResult> DeleteOrdineConfirmed(int idOrdine)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var ordine = await _db.Ordini.Include(o => o.DettagliOrdine).FirstOrDefaultAsync(o => o.IdOrdine == idOrdine && o.IdUtente == int.Parse(userId));
            if (ordine == null)
            {
                return RedirectToAction("RiepilogoOrdini");
            }
            _db.Ordini.Remove(ordine);
            await _db.SaveChangesAsync();
            return RedirectToAction("RiepilogoOrdini");
        }
    }
}
