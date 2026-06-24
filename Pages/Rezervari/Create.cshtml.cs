using Licenta.Data;
using Licenta.Models;
using Licenta.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Licenta.Pages.Rezervari
{
    public class CreateModel : PageModel
    {
        private readonly Licenta.Data.LicentaContext _context;
        private readonly EmailService _emailService;
        [TempData]
        public string? ErrorMessage { get; set; }

        public CreateModel(Licenta.Data.LicentaContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        private async Task IncarcaMasinaAsync()
        {
            Masina = await _context.Autovehicul
                .Include(a => a.Marca)
                .FirstOrDefaultAsync(a => a.ID == Rezervare.AutovehiculID);
        }

        [BindProperty]
        public Rezervare Rezervare { get; set; }

        public Autovehicul Masina { get; set; }

        public IActionResult OnGet(int autovehiculId)
        {
            Masina = _context.Autovehicul
                .Include(a => a.Marca)
                .FirstOrDefault(a => a.ID == autovehiculId);

            if (Masina == null)
                return NotFound();

            Rezervare = new Rezervare
            {
                AutovehiculID = autovehiculId,
                DataStart = DateTime.Today,
                DataFinal = DateTime.Today.AddDays(1)
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
            {
                await IncarcaMasinaAsync();
                ModelState.AddModelError("", "Trebuie să fiți autentificat pentru a face o rezervare.");
                return Page();
            }

            var user = await _context.Utilizator
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                await IncarcaMasinaAsync();
                ModelState.AddModelError("", "Pentru a putea face o rezervare trebuie să încărcați datele permisului de conducere.");
                return Page();
            }

            if (!user.PermisVerificat)
            {
                return RedirectToPage("/Permise/Permis");
            }

            if (user.DataExpirarePermis == null || user.DataExpirarePermis < DateTime.Today)
            {
                ErrorMessage = "Permisul este expirat.";
            }

            if (string.IsNullOrEmpty(user.CategoriiPermis) || !user.CategoriiPermis.Contains("B"))
            {
                ErrorMessage = "Nu aveți categoria B necesară pentru rezervare.";
            }

            Rezervare.UtilizatorID = user.ID;

            var masina = await _context.Autovehicul
                 .Include(a => a.Marca)
                 .FirstOrDefaultAsync(a => a.ID == Rezervare.AutovehiculID);

            if (masina == null)
                return NotFound();

            var zile = (Rezervare.DataFinal - Rezervare.DataStart).Days;

            if (zile <= 0)
            {
                ModelState.AddModelError("", "Interval invalid!");
                return Page();
            }

            Rezervare.PretZi = (decimal)masina.PretZi;

            var categorii = await (from ac in _context.AutoCategorie
                       join c in _context.Categorie on ac.CategorieID equals c.ID
                       where ac.AutovehiculID == masina.ID
                       select c.TipCategorie)
                      .ToListAsync();

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

            Rezervare.Garantie = garantieMax;
            Rezervare.PretTotal = (zile * Rezervare.PretZi) + garantieMax;


            _context.Rezervare.Add(Rezervare);
            await _context.SaveChangesAsync();

            Rezervare = await _context.Rezervare
                .Include(r => r.Autovehicul)
                    .ThenInclude(a => a.Marca)
                .Include(r => r.Utilizator)
                .FirstOrDefaultAsync(r => r.ID == Rezervare.ID);

            await _emailService.TrimiteEmailRezervareAsync(
                Rezervare.Utilizator.Email,
                Rezervare.Utilizator.FullName,
                Rezervare
            );

            return RedirectToPage("./Index");
        }
    }
}
