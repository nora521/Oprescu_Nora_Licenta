using Licenta.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Licenta.Data;
using Licenta.Services;
using Microsoft.EntityFrameworkCore;

namespace Licenta.Pages.Permise
{
    public class PermisModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly OCRService _ocr;
        private readonly LicentaContext _db;
        private readonly PermisParser _parser;

        public PermisModel(IWebHostEnvironment env, OCRService ocr, PermisParser parser, LicentaContext db)
        {
            _env = env;
            _ocr = ocr;
            _parser = parser;
            _db = db;
        }

        public string? Mesaj { get; set; }

        public async Task<IActionResult> OnPostAsync(
            IFormFile? permisFata,
            IFormFile? permisVerso)
        {
            if (permisFata == null || permisVerso == null)
            {
                Mesaj = "Încarcă ambele poze.";
                return Page();
            }

            var folder = Path.Combine(_env.WebRootPath, "permise");
            Directory.CreateDirectory(folder);

            var fataName = Guid.NewGuid() + Path.GetExtension(permisFata.FileName);
            var versoName = Guid.NewGuid() + Path.GetExtension(permisVerso.FileName);

            var fataPath = Path.Combine(folder, fataName);
            var versoPath = Path.Combine(folder, versoName);

            using (var stream = new FileStream(fataPath, FileMode.Create))
            {
                await permisFata.CopyToAsync(stream);
            }

            using (var stream = new FileStream(versoPath, FileMode.Create))
            {
                await permisVerso.CopyToAsync(stream);
            }

            var textFata = await _ocr.ReadTextAsync(fataPath);
            var textVerso = await _ocr.ReadTextAsync(versoPath);

            var text = textFata + "\n" + textVerso;

            var permis = _parser.Parse(text);

            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
            {
                Mesaj = "Utilizator neautentificat.";
                return Page();
            }

            var utilizator = await _db.Utilizator
                .FirstOrDefaultAsync(x => x.Email == email);

            if (utilizator == null)
            {
                Mesaj = "Utilizator inexistent.";
                return Page();
            }

            utilizator.PermisFataPath =
                "/permise/" + fataName;

            utilizator.PermisVersoPath =
                "/permise/" + versoName;

            utilizator.SeriePermis =
                permis.SeriePermis;

            utilizator.CNP =
                permis.CNP;

            utilizator.DataNasterii =
                permis.DataNasterii;

            utilizator.DataEmiterePermis =
                permis.DataEmitere;

            utilizator.DataExpirarePermis =
                permis.DataExpirare;

            utilizator.CategoriiPermis =
                permis.Categorii;

            utilizator.PermisVerificat = true;

            await _db.SaveChangesAsync();

            Mesaj = "Permisul a fost verificat și salvat în baza de date.";

            return Page();
        }
    }
}