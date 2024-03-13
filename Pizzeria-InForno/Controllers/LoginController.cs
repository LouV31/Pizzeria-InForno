using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Pizzeria_InForno.Data;
using Pizzeria_InForno.Models;
using System.Security.Claims;

namespace Pizzeria_InForno.Controllers
{
    public class LoginController : Controller
    {
        // Memorizzo l'istanza di ApplicationDbContext in _db
        // Privata perché così è accessibile solo all'interno di questa classe
        // readonly significa che non posso midificarla
        private readonly ApplicationDbContext _db;
        // IAuthenticationSchemeProvider è un servizio che fornisce le informazioni sui provider di autenticazione disponibili
        // Tramite uno schema di autenticazione che possono esseere Cookie o Bearer Tokens, possiamo associare un nome ad una particolare configurazione di autenticazione
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        // Questo è un costruttore che assegna i valori dei parametri ai campi privati corrispondenti
        public LoginController(ApplicationDbContext db, IAuthenticationSchemeProvider schemeProvider)
        {
            _db = db;
            _schemeProvider = schemeProvider;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ActionName("Index")]
        [HttpPost]
        public async Task<IActionResult> Login(Utenti utente)
        {
            // Query per trovare l'utente nel db
            var dbUser = _db.Utenti.FirstOrDefault(u => u.Username == utente.Username);

            // Se la query non trova niente ci restituisce il temp data da stampare nella view
            if (dbUser == null)
            {
                TempData["error"] = "Questo Nome Utente non esiste";
                return View();
            }
            if (dbUser.Password != utente.Password)
            {
                TempData["error"] = "Credenziali non valide";
                return View();
            }
            // Trovato l'utente, se la password che inseriamo coincide con quella presente sul db possiamo procedere
            // Salviamo nei claims le informazioni sull'utente autenticato
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, dbUser.Username),
                new Claim(ClaimTypes.Role, dbUser.Ruolo),
                new Claim(ClaimTypes.NameIdentifier, dbUser.IdUtente.ToString())
            };
            // Salviamo in questa variabile l'identità dell'utente autenticato
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            TempData["success"] = "Login effettuato con successo";

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["success"] = "Logout effettuato con successo";
            return RedirectToAction("Index", "Home");
        }
    }
}
