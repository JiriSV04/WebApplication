using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webapplication.Pages
{
    public class IndexModel : PageModel
    {
        // Vlastnosti pro p��choz� data
        [BindProperty]
        public string? Name { get; set; }

        [BindProperty]
        public string? Email { get; set; }

        [BindProperty]
        public string? Message { get; set; }

        // Metoda pro zpracov�n� GET po�adavk�
        public void OnGet()
        {
            // Sem m��e� p�idat n�jakou logiku pro GET request, pokud je pot�eba
        }

        // Metoda pro zpracov�n� POST po�adavk�
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Pokud nejsou data validn�, z�sta� na str�nce
            }

            // Tady m��e� zpracovat p�ijat� data, nap��klad je odeslat e-mailem nebo ulo�it do datab�ze
            // T�eba odesl�n� e-mailu s popt�vkou na nacen�n�

            // Po �sp�n�m odesl�n� m��e� p�esm�rovat u�ivatele na n�jakou "Success" str�nku:
            return RedirectToPage("/Success");
        }
    }
}
