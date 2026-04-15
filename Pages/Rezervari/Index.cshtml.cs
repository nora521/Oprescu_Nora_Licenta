using Licenta.Data;
using Licenta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Licenta.Pages.Rezervari
{
    public class IndexModel : PageModel
    {
        private readonly Licenta.Data.LicentaContext _context;

        public IndexModel(Licenta.Data.LicentaContext context)
        {
            _context = context;
        }

        public IList<Rezervare> Rezervare { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var query = _context.Rezervare
                 .Include(r => r.Autovehicul)
                     .ThenInclude(a => a.Marca)
                 .Include(r => r.Autovehicul)
                     .ThenInclude(a => a.AutoCategorii)
                         .ThenInclude(ac => ac.Categorie)
                 .Include(r => r.Utilizator)
                 .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                var email = User.Identity.Name;

                query = query.Where(r => r.Utilizator.Email == email);
            }
            Rezervare = await query.ToListAsync();

            foreach (var rez in Rezervare)
            {
                var categorii = rez.Autovehicul.AutoCategorii
    .Select(ac => ac.Categorie.TipCategorie)
    .ToList();

                decimal garantieMax = 0;

                foreach (var cat in categorii)
                {
                    decimal garantie = 0;

                    switch (cat)
                    {
                        case "Mică/Economy":
                            garantie = 100;
                            break;

                        case "Hatchback/Compact":
                            garantie = 200;
                            break;

                        case "Sedan":
                            garantie = 300;
                            break;

                        case "SUV":
                            garantie = 500;
                            break;

                        case "Break/Wagon":
                            garantie = 300;
                            break;

                        case "Premium/Luxury":
                            garantie = 600;
                            break;

                        default:
                            garantie = 0;
                            break;
                    }

                    if (garantie > garantieMax)
                        garantieMax = garantie;
                }

                rez.Garantie = garantieMax;

            }
        }

    }
}
