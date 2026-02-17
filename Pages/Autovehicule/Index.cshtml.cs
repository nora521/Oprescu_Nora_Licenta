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

        public IList<Autovehicul> Autovehicul { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Autovehicul = await _context.Autovehicul.Include(m =>m.Marca).Include(c => c.Combustibil).ToListAsync();
        }
    }
}
