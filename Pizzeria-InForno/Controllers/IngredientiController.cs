using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizzeria_InForno.Data;
using Pizzeria_InForno.Models;

namespace Pizzeria_InForno.Controllers
{
    public class IngredientiController : Controller
    {
        private readonly ApplicationDbContext _db;

        public IngredientiController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Ingredienti
        public async Task<IActionResult> Index()
        {
            return View(await _db.Ingredienti.ToListAsync());
        }

        // GET: Ingredienti/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredienti = await _db.Ingredienti
                .FirstOrDefaultAsync(m => m.IdIngrediente == id);
            if (ingredienti == null)
            {
                return NotFound();
            }

            return View(ingredienti);
        }

        // GET: Ingredienti/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ingredienti/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomeIngrediente,Prezzo")] Ingredienti ingredienti)
        {
            ModelState.Remove("DettagliIngredienti");
            if (ModelState.IsValid)
            {
                if (_db.Ingredienti.Any(i => i.NomeIngrediente == ingredienti.NomeIngrediente))
                {
                    TempData["error"] = "Esiste già un ingrediente con questo nome";
                    return View(ingredienti);
                }
                _db.Add(ingredienti);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ingredienti);
        }

        // GET: Ingredienti/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredienti = await _db.Ingredienti.FindAsync(id);
            if (ingredienti == null)
            {
                return NotFound();
            }
            return View(ingredienti);
        }

        // POST: Ingredienti/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdIngrediente,NomeIngrediente,Prezzo")] Ingredienti ingredienti)
        {
            if (id != ingredienti.IdIngrediente)
            {
                return NotFound();
            }

            ModelState.Remove("DettagliIngredienti");
            if (ModelState.IsValid)
            {
                try
                {
                    if (_db.Ingredienti.Any(a => a.NomeIngrediente == ingredienti.NomeIngrediente && a.IdIngrediente != ingredienti.IdIngrediente))
                    {
                        TempData["error"] = "Questo nome è associato ad un altro articolo.";
                        return View(ingredienti);
                    }
                    _db.Update(ingredienti);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngredientiExists(ingredienti.IdIngrediente))
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
            return View(ingredienti);
        }

        // GET: Ingredienti/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredienti = await _db.Ingredienti
                .FirstOrDefaultAsync(m => m.IdIngrediente == id);
            if (ingredienti == null)
            {
                return NotFound();
            }

            return View(ingredienti);
        }

        // POST: Ingredienti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingredienti = await _db.Ingredienti.FindAsync(id);
            if (ingredienti != null)
            {
                _db.Ingredienti.Remove(ingredienti);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IngredientiExists(int id)
        {
            return _db.Ingredienti.Any(e => e.IdIngrediente == id);
        }
    }
}
