using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizzeria_InForno.Data;
using Pizzeria_InForno.Models;

namespace Pizzeria_InForno.Controllers
{
    public class ArticoliController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ArticoliController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Articoli
        public async Task<IActionResult> Index()
        {
            return View(await _db.Articoli.ToListAsync());
        }

        // GET: Articoli/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Articoli/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Articoli/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomeArticolo,Descrizione,Prezzo,Immagine,TempoConsegna")] Articoli articoli)
        {
            ModelState.Remove("DettagliOrdine");
            ModelState.Remove("Ingredienti");
            if (ModelState.IsValid)
            {
                if (_db.Articoli.Any(a => a.NomeArticolo == articoli.NomeArticolo))
                {
                    TempData["error"] = "Esiste già un articolo con questo nome";
                    return View(articoli);
                }
                _db.Add(articoli);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(articoli);
        }

        // GET: Articoli/Edit/5
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
        public async Task<IActionResult> Edit(int id, [Bind("IdArticolo,NomeArticolo,Descrizione,Prezzo,Immagine,TempoConsegna")] Articoli articoli)
        {
            if (id != articoli.IdArticolo)
            {
                return NotFound();
            }

            ModelState.Remove("DettagliOrdine");
            ModelState.Remove("Ingredienti");
            if (ModelState.IsValid)
            {
                try
                {
                    if (_db.Articoli.Any(a => a.NomeArticolo == articoli.NomeArticolo && a.IdArticolo != articoli.IdArticolo))
                    {
                        TempData["error"] = "Questo nome è associato ad un altro articolo.";
                        return View(articoli);
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
    }
}
