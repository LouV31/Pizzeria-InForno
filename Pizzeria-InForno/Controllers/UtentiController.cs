using Microsoft.AspNetCore.Mvc;
using Pizzeria_InForno.Data;
using Pizzeria_InForno.Models;

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
    }
}
