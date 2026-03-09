using Licenta.Data;
using Licenta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
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

        public async Task<IActionResult> OnGetExportPdfAsync(int? id)
        {
            if (id == null) return NotFound();

            var auto = await _context.Autovehicul
                .Include(a => a.Marca)
                .Include(a => a.Combustibil)
                .Include(a => a.Utilizator)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (auto == null) return NotFound();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial));

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
                            AddRow(table, "Model:", auto.Model);
                            AddRow(table, "Serie Șasiu:", auto.SerieSasiu);
                            AddRow(table, "Nr. Înmatriculare:", auto.NrInmatriculare);
                            AddRow(table, "Combustibil:", auto.Combustibil?.TipCombustibil ?? "-");
                            AddRow(table, "Kilometraj:", $"{auto.Kilometraj} KM");
                            AddRow(table, "Consum mixt:", $"{auto.ConsumMixt} L/100 KM");
                            AddRow(table, "Conducător auto:", auto.Utilizator?.FullName ?? "Nespecificat");
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
                document.GeneratePdf(stream);
                return File(stream.ToArray(), "application/pdf", $"Export_{auto.NrInmatriculare}.pdf");
            }
        }

        private void AddRow(TableDescriptor table, string label, string value)
        {
            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(label).SemiBold();
            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(value);
        }

        private void AddDateRow(TableDescriptor table, string denumire, DateTime data)
        {
            table.Cell().Element(CellStyle).Text(denumire);

            table.Cell().Element(CellStyle).Text(data.ToString("dd.MM.yyyy"));

            IContainer CellStyle(IContainer container) =>
                container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5);
        }

        public async Task<IActionResult> OnPostConfirmareAsync(int id)
        {
            var autovehicul = await _context.Autovehicul.FindAsync(id);

            if (autovehicul != null)
            {
                autovehicul.Confirmare = true; 
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
