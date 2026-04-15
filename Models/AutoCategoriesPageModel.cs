using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Licenta.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Licenta.Models
{
    public class AutoCategoriesPageModel : PageModel
    {
        public List<AssignedCategoryData> AssignedCategoryDataList;
        public void PopulateAssignedCategoryData(LicentaContext context, Autovehicul autovehicul)
        {
            // Materialize categories to avoid deferred execution issues and improve clarity
            var allCategories = context.Categorie.AsNoTracking().ToList();
            var autovehiculCategories = new HashSet<int>(
                autovehicul?.AutoCategorii?.Select(c => c.CategorieID)
                ?? Enumerable.Empty<int>());

            AssignedCategoryDataList = allCategories
                .Select(category => new AssignedCategoryData
                {
                    CategoryID = category.ID,
                    Nume = category.TipCategorie,
                    Assigned = autovehiculCategories.Contains(category.ID)
                })
                .ToList();
        }

        public void UpdateAutoCategories(LicentaContext context, string[] selectedCategories, Autovehicul autovehiculToUpdate)
        {
            if (selectedCategories == null)
            {
                autovehiculToUpdate.AutoCategorii = new List<AutoCategorie>();
                return;
            }
            var selectedCategoriesHS = new HashSet<string>(selectedCategories);
            autovehiculToUpdate.AutoCategorii ??= new List<AutoCategorie>();
            var autovehiculCategories = new HashSet<int>(autovehiculToUpdate.AutoCategorii.Select(c => c.CategorieID));
            foreach (var category in context.Categorie)
            {
                if (selectedCategoriesHS.Contains(category.ID.ToString()))
                {
                    if (!autovehiculCategories.Contains(category.ID))
                    {
                        autovehiculToUpdate.AutoCategorii.Add(new AutoCategorie
                        {
                            AutovehiculID = autovehiculToUpdate.ID,
                            CategorieID = category.ID
                        });
                    }
                }
                else
                {
                    if (autovehiculCategories.Contains(category.ID))
                    {
                        AutoCategorie autoToRemove = autovehiculToUpdate.AutoCategorii.SingleOrDefault(i => i.CategorieID == category.ID);
                        context.Remove(autoToRemove);
                    }
                }
            }
        }

        private readonly LicentaContext _context;
        public AutoCategoriesPageModel(LicentaContext context)
        {
            _context = context;
        }
        public Autovehicul Autovehicul { get; set; }

        // Provide a protected helper to load an Autovehicul and populate category data.
        // This is not a page handler so derived PageModel classes can define their own
        // OnGet/OnPost handlers without causing ambiguous handler selection.
        protected async Task<IActionResult> LoadAutovehiculAndPopulateAsync(int id)
        {
            var autovehicul = await _context.Autovehicul
                .Include(a => a.Marca)
                .Include(a => a.AutoCategorii).ThenInclude(ac => ac.Categorie)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (autovehicul == null) return NotFound();

            PopulateAssignedCategoryData(_context, autovehicul);
            Autovehicul = autovehicul;

            return Page();
        }
    }
}