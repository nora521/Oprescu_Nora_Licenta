using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Licenta.Data;
using Licenta.Models;

namespace Licenta.Pages.Autovehicule
{
    public class IndexModel : PageModel
    {
        private readonly Licenta.Data.LicentaContext _context;

        public IndexModel(Licenta.Data.LicentaContext context)
        {
            _context = context;
        }

        public IList<Autovehicul> Autovehicul { get; set; } = default!;
        public string CurrentFilter { get; set; }

        public async Task OnGetAsync(string searchString)
        {

            CurrentFilter = searchString;

            var userEmail = User.Identity?.Name;

            var query = from row in _context.Autovehicul
                .Include(m => m.Marca)
                .Include(c => c.Combustibil)
                .Include(u => u.Utilizator)
                        select row;

                if (!string.IsNullOrEmpty(searchString))
                {
                query = query.Where(s => s.NrInmatriculare.Contains(searchString)
                      || s.Marca.NumeMarca.Contains(searchString)
                      || s.Model.Contains(searchString)
                      || s.Utilizator.Nume.Contains(searchString)
                      || s.Utilizator.Prenume.Contains(searchString));
                }

                bool esteAdmin = User.IsInRole("Admin") || userEmail == "nora_oprescu@yahoo.com";

                if (!esteAdmin)
                {
                    query = query.Where(a => a.Utilizator.Email == userEmail);
                }

                Autovehicul = await query.ToListAsync();
            
        }
    }
}
