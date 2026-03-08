using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Licenta.Data;
using Licenta.Models;

namespace Licenta.Pages.Utilizatori
{
    public class IndexModel : PageModel
    {
        private readonly Licenta.Data.LicentaContext _context;

        public IndexModel(Licenta.Data.LicentaContext context)
        {
            _context = context;
        }

        public IList<Utilizator> Utilizator { get;set; } = default!;

        public string CurrentFilter { get; set; }

        public async Task OnGetAsync(string searchString)
        {
            CurrentFilter = searchString;
            var query = from u in _context.Utilizator
                        select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(u => u.Nume.Contains(searchString)
                                      || u.Prenume.Contains(searchString)
                                      || u.Email.Contains(searchString)
                                      || u.CNP.Contains(searchString)
                                      || u.NrTelefon.Contains(searchString));
            }

            Utilizator = await query.ToListAsync();
        }
    }
}
