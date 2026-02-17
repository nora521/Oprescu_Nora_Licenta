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

namespace Licenta.Pages.Combustibili
{
    public class EditModel : PageModel
    {
        private readonly Licenta.Data.LicentaContext _context;

        public EditModel(Licenta.Data.LicentaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Combustibil Combustibil { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var combustibil =  await _context.Combustibil.FirstOrDefaultAsync(m => m.ID == id);
            if (combustibil == null)
            {
                return NotFound();
            }
            Combustibil = combustibil;
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

            _context.Attach(Combustibil).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CombustibilExists(Combustibil.ID))
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

        private bool CombustibilExists(int id)
        {
            return _context.Combustibil.Any(e => e.ID == id);
        }
    }
}
