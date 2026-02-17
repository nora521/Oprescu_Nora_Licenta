using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Licenta.Data;
using Licenta.Models;

namespace Licenta.Pages.Autovehicule
{
    public class EditModel : PageModel
    {
        private readonly Licenta.Data.LicentaContext _context;

        public EditModel(Licenta.Data.LicentaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Autovehicul Autovehicul { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autovehicul =  await _context.Autovehicul.FirstOrDefaultAsync(m => m.ID == id);
            if (autovehicul == null)
            {
                return NotFound();
            }
            Autovehicul = autovehicul;
            ViewData["MarcaID"] = new SelectList(_context.Set<Marca>(), "ID", "NumeMarca");
            ViewData["CombustibilID"] = new SelectList(_context.Set<Combustibil>(), "ID", "TipCombustibil");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Autovehicul).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AutovehiculExists(Autovehicul.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AutovehiculExists(int id)
        {
            return _context.Autovehicul.Any(e => e.ID == id);
        }
    }
}
