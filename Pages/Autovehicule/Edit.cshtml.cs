using Licenta.Data;
using Licenta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Licenta.Pages.Autovehicule
{
    [Authorize(Roles = "Admin")]
    public class EditModel : AutoCategoriesPageModel
    {
        private readonly Licenta.Data.LicentaContext _context;

        public EditModel(Licenta.Data.LicentaContext context) : base(context)
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

            var autovehicul =  await _context.Autovehicul
                .Include(a => a.Marca)
                .Include(a => a.AutoCategorii).ThenInclude(ac => ac.Categorie)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (autovehicul == null)
            {
                return NotFound();
            }
            // use the loaded local variable (avoid using the page property before assignment)
            PopulateAssignedCategoryData(_context, autovehicul);
            Autovehicul = autovehicul;
            ViewData["MarcaID"] = new SelectList(_context.Set<Marca>(), "ID", "NumeMarca");
            ViewData["CombustibilID"] = new SelectList(_context.Set<Combustibil>(), "ID", "TipCombustibil");
            ViewData["TransmisieID"] = new SelectList(_context.Set<Transmisie>(), "ID", "TipTransmisie");
            ViewData["CategorieID"] = new SelectList(_context.Set<Categorie>(), "ID", "TipCategorie");
            ViewData["UtilizatorID"] = new SelectList(_context.Set<Utilizator>(), "ID", "FullName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedCategories)
        {
            if (id==null)
            {
                return NotFound();
            }

            var autoToUpdate = await _context.Autovehicul
                .Include(i => i.Marca)
                .Include(i => i.AutoCategorii)
                .ThenInclude(i => i.Categorie)
                .FirstOrDefaultAsync(s => s.ID == id); 
            
            if (autoToUpdate == null) { return NotFound(); }

            if (await TryUpdateModelAsync<Autovehicul>(
            autoToUpdate,
            "Autovehicul"))
            {
                UpdateAutoCategories(_context, selectedCategories, autoToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            UpdateAutoCategories(_context, selectedCategories, autoToUpdate); 
            PopulateAssignedCategoryData(_context, autoToUpdate); 
       
            return Page();
        }

        private bool AutovehiculExists(int id)
        {
            return _context.Autovehicul.Any(e => e.ID == id);
        }
    }
}
