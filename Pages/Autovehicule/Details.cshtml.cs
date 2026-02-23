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
    public class DetailsModel : PageModel
    {
        private readonly Licenta.Data.LicentaContext _context;

        public DetailsModel(Licenta.Data.LicentaContext context)
        {
            _context = context;
        }

        public Autovehicul Autovehicul { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autovehicul = await _context.Autovehicul.Include(a=>a.Marca).Include(c=>c.Combustibil).FirstOrDefaultAsync(m => m.ID == id);
            if (autovehicul == null)
            {
                return NotFound();
            }
            else
            {
                Autovehicul = autovehicul;
            }
            return Page();
        }
    }
}
