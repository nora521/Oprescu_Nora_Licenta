using Licenta.Data;
using Licenta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Licenta.Pages.Autovehicule
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : AutoCategoriesPageModel
    {
        private readonly Licenta.Data.LicentaContext _context;

        public CreateModel(Licenta.Data.LicentaContext context) : base(context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["MarcaID"] = new SelectList(_context.Set<Marca>(), "ID","NumeMarca");
            ViewData["CombustibilID"] = new SelectList(_context.Set<Combustibil>(), "ID","TipCombustibil");
            ViewData["TransmisieID"] = new SelectList(_context.Set<Transmisie>(), "ID", "TipTransmisie");
            ViewData["UtilizatorID"] = new SelectList(_context.Set<Utilizator>(), "ID","FullName");

            var auto = new Autovehicul(); 
            auto.AutoCategorii = new List<AutoCategorie>(); 
            PopulateAssignedCategoryData(_context, auto);

            return Page();
        }

        [BindProperty]
        public Autovehicul Autovehicul { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string[] selectedCategories)
        {
            var newAuto = new Autovehicul();
            if (selectedCategories != null)
            {
                newAuto.AutoCategorii = new List<AutoCategorie>();
                foreach (string category in selectedCategories)
                {
                    var catToAdd = new AutoCategorie
                    {
                        CategorieID = int.Parse(category)
                    };
                    newAuto.AutoCategorii.Add(catToAdd);
                }
            }
            Autovehicul.AutoCategorii = newAuto.AutoCategorii; 
            _context.Autovehicul.Add(Autovehicul); 
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
