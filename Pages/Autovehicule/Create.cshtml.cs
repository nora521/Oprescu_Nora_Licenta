using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Licenta.Data;
using Licenta.Models;

namespace Licenta.Pages.Autovehicule
{
    public class CreateModel : PageModel
    {
        private readonly Licenta.Data.LicentaContext _context;

        public CreateModel(Licenta.Data.LicentaContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["MarcaID"] = new SelectList(_context.Set<Marca>(), "ID","NumeMarca");
            ViewData["CombustibilID"] = new SelectList(_context.Set<Combustibil>(), "ID","TipCombustibil");
            return Page();
        }

        [BindProperty]
        public Autovehicul Autovehicul { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Autovehicul.Add(Autovehicul);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
