using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapplication.Pages
{
    public class IndexModel : PageModel
    {
        // Vlastnosti pro pøíchozí data
        [BindProperty]
        public string? Name { get; set; }

        [BindProperty]
        public string? Email { get; set; }

        [BindProperty]
        public string? Message { get; set; }

        // Metoda pro zpracování GET požadavkù
        public void OnGet()
        {
            // Sem mùžeš pøidat nìjakou logiku pro GET request, pokud je potøeba
        }

        // Metoda pro zpracování POST požadavkù
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Pokud nejsou data validní, zùstaò na stránce
            }

            // Tady mùžeš zpracovat pøijatá data, napøíklad je odeslat e-mailem nebo uložit do databáze
            // Tøeba odeslání e-mailu s poptávkou na nacenìní

            // Po úspìšném odeslání mùžeš pøesmìrovat uživatele na nìjakou "Success" stránku:
            return RedirectToPage("/Success");
        }
    }
}
