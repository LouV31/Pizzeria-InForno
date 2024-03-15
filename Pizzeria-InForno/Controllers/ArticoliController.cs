using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pizzeria_InForno.Data;
using Pizzeria_InForno.Models;

namespace Pizzeria_InForno.Controllers
{

    public class ArticoliController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ArticoliController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Articoli
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var articoli = await _db.Articoli.Include(a => a.DettagliIngrediente).ThenInclude(di => di.Ingredienti).ToListAsync();
            foreach (var articolo in articoli)
            {
                articolo.Prezzo += articolo.DettagliIngrediente.Sum(di => di.Ingredienti.Prezzo);
            }

            return View(articoli);
        }


        // GET: Articoli/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articoli = await _db.Articoli
                .Include(a => a.DettagliIngrediente)
                .ThenInclude(di => di.Ingredienti)
                .FirstOrDefaultAsync(m => m.IdArticolo == id);
            if (articoli == null)
            {
                return NotFound();
            }

            return View(articoli);
        }

        // GET: Articoli/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewBag.Ingredienti = new MultiSelectList(_db.Ingredienti, "IdIngrediente", "NomeIngrediente");
            return View();
        }

        // POST: Articoli/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("NomeArticolo,Descrizione,Prezzo,TempoConsegna")] Articoli articoli, List<int> ingredientiSelezionati, IFormFile Immagine)
        {
            ModelState.Remove("DettagliOrdine");
            ModelState.Remove("DettagliIngrediente");
            ModelState.Remove("Immagine");
            if (ModelState.IsValid)
            {
                if (_db.Articoli.Any(a => a.NomeArticolo == articoli.NomeArticolo))
                {
                    TempData["error"] = "Esiste già un articolo con questo nome";
                    ViewBag.Ingredienti = new MultiSelectList(_db.Ingredienti, "IdIngrediente", "NomeIngrediente");
                    return View(articoli);
                }
                if (Immagine != null && Immagine.Length > 0)
                {
                    var fileName = Path.GetFileName(Immagine.FileName);
                    var path = Path.Combine(_hostEnvironment.WebRootPath, "img", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await Immagine.CopyToAsync(fileStream);
                    }
                    articoli.Immagine = Path.Combine("img", fileName);
                }

                _db.Articoli.Add(articoli);
                await _db.SaveChangesAsync();
                foreach (var idIngrediente in ingredientiSelezionati)
                {
                    var dettagliIngrediente = new DettagliIngredienti
                    {
                        IdArticolo = articoli.IdArticolo,
                        IdIngrediente = idIngrediente
                    };

                    _db.DettagliIngrediente.Add(dettagliIngrediente);
                }
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Ingredienti = new MultiSelectList(_db.Ingredienti, "IdIngrediente", "NomeIngrediente");
            return View(articoli);
        }

        // GET: Articoli/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articoli = await _db.Articoli.FindAsync(id);
            if (articoli == null)
            {
                return NotFound();
            }
            return View(articoli);
        }

        // POST: Articoli/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("IdArticolo,NomeArticolo,Descrizione,Prezzo,Immagine,TempoConsegna")] Articoli articoli, IFormFile Immagine)
        {
            if (id != articoli.IdArticolo)
            {
                return NotFound();
            }

            ModelState.Remove("DettagliOrdine");
            ModelState.Remove("DettagliIngrediente");

            if (ModelState.IsValid)
            {
                try
                {
                    if (_db.Articoli.Any(a => a.NomeArticolo == articoli.NomeArticolo && a.IdArticolo != articoli.IdArticolo))
                    {
                        TempData["error"] = "Questo nome è associato ad un altro articolo.";
                        return View(articoli);
                    }
                    if (Immagine != null && Immagine.Length > 0)
                    {
                        var fileName = Path.GetFileName(Immagine.FileName);
                        var path = Path.Combine(_hostEnvironment.WebRootPath, "img", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await Immagine.CopyToAsync(fileStream);
                        }
                        articoli.Immagine = Path.Combine("img", fileName);
                    }
                    _db.Update(articoli);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticoliExists(articoli.IdArticolo))
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
            return View(articoli);
        }

        // GET: Articoli/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articoli = await _db.Articoli
                .FirstOrDefaultAsync(m => m.IdArticolo == id);
            if (articoli == null)
            {
                return NotFound();
            }

            return View(articoli);
        }

        // POST: Articoli/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var articoli = await _db.Articoli.FindAsync(id);
            if (articoli != null)
            {
                _db.Articoli.Remove(articoli);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticoliExists(int id)
        {
            return _db.Articoli.Any(e => e.IdArticolo == id);
        }

        [Authorize(Roles = "user, admin")]
        [HttpPost]
        // Questo metodo aggiunge un articolo al carrello

        public void AddToCart([FromBody] Carrello carrello)
        {
            int id = carrello.Articoli.IdArticolo;
            int quantity = carrello.Quantita;
            // Variabile che ci dice se l'articolo è già presente nel carrello
            bool isExist = false;
            // Query per trovare l'articolo nel db
            var articolo = _db.Articoli.Find(id);

            if (articolo != null)
            {
                // Prendo il carrello dalla sessione
                var cartSession = HttpContext.Session.GetString("cartList");
                if (cartSession != null)
                {
                    // dal momendo che il carrello è una lista di articoli, deserializziamo la stringa in una lista di articoli

                    List<Carrello> cart = JsonConvert.DeserializeObject<List<Carrello>>(cartSession);
                    // scorriamo la lista di articoli e se l'articolo è già presente nel carrello incrementiamo la quantità
                    foreach (var item in cart)
                    {
                        if (item.Articoli.IdArticolo == id)
                        {
                            item.Quantita += quantity;
                            // Serializziamo la lista di articoli in una stringa e la salviamo nella sessione
                            HttpContext.Session.SetString("cartList", JsonConvert.SerializeObject(cart));
                            isExist = true;
                        }
                    }
                }
                // Se l'articolo non è presente nel carrello, lo aggiungiamo
                if (!isExist)
                {
                    // Creiamo un nuovo oggetto articoliPerCarrello che è la mia classe boilerplate di articoli e gli assegniamo i valori dell'articolo
                    ArticoliPerCarrello articoliPerCarrello = new ArticoliPerCarrello
                    {
                        IdArticolo = articolo.IdArticolo,
                        NomeArticolo = articolo.NomeArticolo,
                        Descrizione = articolo.Descrizione,
                        Prezzo = articolo.Prezzo,
                        Immagine = Path.Combine("/", articolo.Immagine),
                        TempoConsegna = articolo.TempoConsegna
                    };
                    // Creiamo un nuovo oggetto carrello e gli assegniamo i valori dell'articoloPerCarrello e la quantità
                    Carrello cart = new Carrello
                    {
                        Articoli = articoliPerCarrello,
                        Quantita = quantity
                    };
                    // Prendiamo il carrello dalla sessione
                    List<Carrello> cartList = new List<Carrello>();
                    if (!string.IsNullOrEmpty(cartSession))
                    {

                        cartList = JsonConvert.DeserializeObject<List<Carrello>>(cartSession);
                    }
                    // Aggiungiamo il nuovo articolo al carrello
                    cartList.Add(cart);
                    HttpContext.Session.SetString("cartList", JsonConvert.SerializeObject(cartList));
                }
            }

        }

        [HttpPost]
        [Authorize(Roles = "user, admin")]
        public IActionResult RemoveFromCart([FromBody] Carrello carrello)
        {
            int id = carrello.Articoli.IdArticolo;
            var cartSession = HttpContext.Session.GetString("cartList");
            if (cartSession != null)
            {
                List<Carrello> cart = JsonConvert.DeserializeObject<List<Carrello>>(cartSession);
                var itemToRemove = cart.SingleOrDefault(item => item.Articoli.IdArticolo == id);
                if (itemToRemove != null)
                {
                    cart.Remove(itemToRemove);
                    HttpContext.Session.SetString("cartList", JsonConvert.SerializeObject(cart));
                    return Json(itemToRemove);
                }
            }
            return BadRequest();
        }

        [Authorize(Roles = "user, admin")]
        public IActionResult CartView()
        {
            var cartSession = HttpContext.Session.GetString("cartList");
            List<Carrello> cartList = new List<Carrello>();
            if (!string.IsNullOrEmpty(cartSession))
            {
                cartList = JsonConvert.DeserializeObject<List<Carrello>>(cartSession);
                double prezzoTotale = 0;
                foreach (var item in cartList)
                {
                    var articolo = _db.Articoli.Include(a => a.DettagliIngrediente).ThenInclude(di => di.Ingredienti).Single(a => a.IdArticolo == item.Articoli.IdArticolo);
                    var prezzoIngredienti = articolo.DettagliIngrediente.Sum(di => di.Ingredienti.Prezzo);
                    item.Totale = (articolo.Prezzo + prezzoIngredienti) * item.Quantita;
                }
                ViewBag.PrezzoTotale = cartList.Sum(i => i.Totale);
            }
            return View(cartList);
        }
    }
}
