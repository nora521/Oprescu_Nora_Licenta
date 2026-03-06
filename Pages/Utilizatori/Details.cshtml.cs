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
    public class DetailsModel : PageModel
    {
        private readonly Licenta.Data.LicentaContext _context;

        public DetailsModel(Licenta.Data.LicentaContext context)
        {
            _context = context;
        }

        public Utilizator Utilizator { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizator = await _context.Utilizator.FirstOrDefaultAsync(m => m.ID == id);
            if (utilizator == null)
            {
                return NotFound();
            }
            else
            {
                Utilizator = utilizator;
            }
            return Page();
        }
    }
}
