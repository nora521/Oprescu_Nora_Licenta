using Azure;
using Licenta.Data;
using Licenta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task OnGetAsync(string searchString, int? categoryID, string sortOrder)
        {
            ViewData["PriceSort"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            ViewData["CurrentSort"] = sortOrder;


            CurrentFilter = searchString;

            var userEmail = User.Identity?.Name;

            var query = _context.Autovehicul
                .Include(m => m.Marca)
                .Include(c => c.Combustibil)
                .Include(c => c.Transmisie)
                .Include(u => u.Utilizator)
                .Include(ac => ac.AutoCategorii).ThenInclude(c => c.Categorie)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                {
                query = query.Where(s => s.NrInmatriculare.Contains(searchString)
                      || s.Marca.NumeMarca.Contains(searchString)
                      || s.Model.Contains(searchString)
                      || s.Utilizator.Nume.Contains(searchString)
                      || s.Utilizator.Prenume.Contains(searchString));
                }
            if(categoryID != null)
    {
                query = query.Where(a =>
                    a.AutoCategorii.Any(ac => ac.CategorieID == categoryID));
            }

            switch (sortOrder)
            {
                case "price_asc":
                    query = query.OrderBy(a => a.PretZi);
                    break;

                case "price_desc":
                    query = query.OrderByDescending(a => a.PretZi);
                    break;
            }

            var list = await query.ToListAsync();

            var rezervari = await _context.Rezervare
                .Select(r => new {
                     r.AutovehiculID,
                     r.DataStart,
                     r.DataFinal
                })
                .ToListAsync();

            if (!User.IsInRole("Admin"))
            {
                list = list
                    .Where(a => !rezervari.Any(r =>
                        r.AutovehiculID == a.ID &&
                        r.DataStart <= DateTime.Today &&
                        r.DataFinal >= DateTime.Today
                        ))
                    .ToList();

                list = list
                    .GroupBy(a => new { a.MarcaID, a.Model })
                    .Select(g => g.First())
                    .ToList();
            }

            var rezervateIds = rezervari
                .Where(r => r.AutovehiculID.HasValue &&
                            r.DataStart <= DateTime.Today &&
                            r.DataFinal >= DateTime.Today)
                .Select(r => r.AutovehiculID!.Value)
                .ToHashSet();

            ViewData["RezervateIds"] = rezervateIds;

            Autovehicul = list;

            ViewData["CategoryID"] = new SelectList(_context.Categorie, "ID", "TipCategorie");

        }

        public async Task<IActionResult> OnGetExportPdfAsync(int? id, bool? download)
        {
            try
            {
                if (id == null) return NotFound();

                var auto = await _context.Autovehicul
                    .Include(a => a.Marca)
                    .Include(a => a.Combustibil)
                    .Include(a => a.Transmisie)
                    .Include(a => a.Utilizator)
                    .FirstOrDefaultAsync(m => m.ID == id);

                if (auto == null) return NotFound();

                auto.Poza = null;

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(1, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("FIȘĂ TEHNICĂ VEHICUL").FontSize(20).SemiBold().FontColor(Colors.Red.Medium);
                                col.Item().Text($"{auto.Marca?.NumeMarca} {auto.Model} - {auto.NrInmatriculare}");
                            });
                            row.ConstantItem(100).AlignRight().Text(DateTime.Now.ToString("dd.MM.yyyy"));
                        });

                        page.Content().PaddingVertical(10).Column(col =>
                        {
                            col.Item().LineHorizontal(1);

                            col.Item().PaddingTop(10).Text("IDENTIFICARE ȘI DETALII TEHNICE").FontSize(14).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Blue.Lighten3);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(140);
                                    columns.RelativeColumn();
                                });

                                AddRow(table, "Marcă:", auto.Marca?.NumeMarca ?? "-");
                                AddRow(table, "Model:", auto.Model ?? "-");
                                AddRow(table, "Serie Șasiu:", auto.SerieSasiu ?? "-");
                                AddRow(table, "Nr. Înmatriculare:", auto.NrInmatriculare ?? "-");
                                AddRow(table, "An fabricație:", $"{auto.AnFabricatie}");
                                AddRow(table, "Combustibil:", auto.Combustibil?.TipCombustibil ?? "-");
                                AddRow(table, "Transmisie:", auto.Transmisie?.TipTransmisie ?? "-");
                                AddRow(table, "Capacitate cilindrică:", $"{auto.CMC} CM³");
                                AddRow(table, "Cai Putere:", $"{auto.CP} CP");
                                AddRow(table, "Kilometraj:", $"{auto.Kilometraj} KM");
                                AddRow(table, "Consum mixt:", $"{auto.ConsumMixt} L/100 KM");
                                AddRow(table, "Număr locuri:", $"{auto.NrLocuri}");
                                AddRow(table, "Număr bagaje:", $"{auto.NrBagaje}");
                                AddRow(table, "Culoare:", auto.Culoare ?? "-");
                            });

                            col.Item().PaddingVertical(30);

                            col.Item().Text("TERMENE ȘI VALABILITĂȚI").FontSize(14).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Blue.Lighten3);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderStyle).Text("Document / Verificare");
                                    header.Cell().Element(HeaderStyle).Text("Data Expirării");

                                    IContainer HeaderStyle(IContainer container) =>
                                        container.Background(Colors.Grey.Lighten4).Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Medium);
                                });

                                AddDateRow(table, "Inspecție Tehnică Periodică (ITP)", auto.DataITP);
                                AddDateRow(table, "Asigurare RCA", auto.DataRCA);
                                AddDateRow(table, "Rovinietă", auto.DataRovinieta);
                                AddDateRow(table, "Revizie Tehnică", auto.DataRevizie);
                            });
                        });

                        page.Footer().AlignCenter().PaddingTop(10).Text(x =>
                        {
                            x.Span("Pagina ");
                            x.CurrentPageNumber();
                        });
                    });
                });

                using (var stream = new MemoryStream())
                {
                    await Task.Run(() => document.GeneratePdf(stream));
                    stream.Seek(0, SeekOrigin.Begin);

                    var content = stream.ToArray();
                    var fileName = $"Export_{auto.NrInmatriculare}.pdf";

                    if (download.HasValue && download.Value)
                    {
                        return File(content, "application/pdf", fileName);
                    }

                    Response.Headers["Content-Disposition"] = $"inline; filename*=UTF-8''{Uri.EscapeDataString(fileName)}";
                    return File(content, "application/pdf");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        private void AddRow(TableDescriptor table, string label, string value)
        {
            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(label).SemiBold();
            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(value);
        }

        private void AddDateRow(TableDescriptor table, string denumire, DateTime? data)
        {
            table.Cell().Element(CellStyle).Text(denumire);

            table.Cell().Element(CellStyle).Text(data.HasValue ? data.Value.ToString("dd.MM.yyyy") : "-");

            IContainer CellStyle(IContainer container) =>
                container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5);
        }

        public class Utilizator
        {
            public int ID { get; set; }

            [Column("PermisFataPath")]
            public string? PermisFata { get; set; }

            [Column("PermisVersoPath")]
            public string? PermisVerso { get; set; }

            [Column("NumarPermis")] 
            public string? SeriePermis { get; set; }

        }
    }
}
