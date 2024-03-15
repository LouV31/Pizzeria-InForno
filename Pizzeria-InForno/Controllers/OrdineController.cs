using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IWebHostEnvironment _hostingEnvironment;
        public OrdineController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
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
            var ordine = await _db.Ordini
                .Include(o => o.DettagliOrdine)
                    .ThenInclude(d => d.Articoli)
                        .ThenInclude(a => a.DettagliIngrediente)
                            .ThenInclude(di => di.Ingredienti)
                .FirstOrDefaultAsync(o => o.IdOrdine == id);
            if (ordine == null)
            {
                return RedirectToAction("index");
            }
            foreach (var dettaglio in ordine.DettagliOrdine)
            {
                var prezzoArticolo = dettaglio.Articoli.Prezzo + dettaglio.Articoli.DettagliIngrediente.Sum(di => di.Ingredienti.Prezzo);
                dettaglio.Articoli.Prezzo = prezzoArticolo * dettaglio.Quantita;

                dettaglio.Articoli.Immagine = Path.Combine("/", dettaglio.Articoli.Immagine);
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
            // questa query include anche l'utente che ha effettuato l'ordine per poter visualizzare il suo username
            var ordine = await _db.Ordini.Include(o => o.Utenti).FirstOrDefaultAsync(o => o.IdOrdine == id);
            if (ordine == null)
            {
                return NotFound();
            }
            ViewBag.Utente = new SelectListItem
            {
                Value = ordine.Utenti.IdUtente.ToString(),
                Text = ordine.Utenti.Username
            };
            return View(ordine);
        }

        // POST Ordine/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("IdOrdine, IdUtente, IndirizzoSpedizione, IsConsegnato, Note, Totale, DataOrdine")] Ordini ordine)
        {
            if (id != ordine.IdOrdine)
            {
                return NotFound();
            }
            // per evitare che l'utente modifichi l'ID dell'utente sul quale si vuole effettuare la modifica nell'edit dell'ordine
            var checkOriginalOrder = await _db.Ordini.AsNoTracking().FirstOrDefaultAsync(o => o.IdOrdine == id);
            var utente = await _db.Utenti.FindAsync(checkOriginalOrder.IdUtente);
            if (checkOriginalOrder.IdUtente != ordine.IdUtente)
            {
                TempData["error"] = "Non puoi modificare l'ID dell'utente";

                ViewBag.Utente = new SelectListItem
                {
                    Value = utente.IdUtente.ToString(),
                    Text = utente.Username
                };
                return View(ordine);
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
            ViewBag.Utente = new SelectListItem
            {
                Value = utente.IdUtente.ToString(),
                Text = utente.Username
            };
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
                ordine.DataOrdine = DateTime.Now;

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

        // GET Ordine/TotaleFatturatoInData da fetchare
        public async Task<IActionResult> TotaleFatturatoInData(DateTime date)
        {
            var fatturato = await _db.Ordini.Where(o => o.DataOrdine.Date == date.Date && o.IsConsegnato).SumAsync(o => o.Totale);
            return Json(fatturato);
        }

        // get Ordine/TotaleOrdiniEvasi da fetchare
        public async Task<IActionResult> TotaleOrdiniEvasi()
        {
            var totaleOrdiniEvasi = await _db.Ordini.Where(o => o.IsConsegnato).CountAsync();
            return Json(totaleOrdiniEvasi);
        }
    }
}
