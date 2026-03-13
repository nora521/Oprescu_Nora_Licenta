using Microsoft.EntityFrameworkCore;
using Licenta.Data;
using Licenta.Services;

namespace Licenta.Services
{
    public class NotificariBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public NotificariBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndSend(stoppingToken);
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private async Task CheckAndSend(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LicentaContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                var prag = DateTime.Now.Date.AddDays(7);
                var azi = DateTime.Now.Date;

                var masini = await context.Autovehicul
                    .Include(a => a.Utilizator)
                    .Include(a => a.Marca)
                    .AsNoTracking()
                    .Where(a => !a.Confirmare && (
                        (a.DataITP.Date >= azi && a.DataITP.Date <= prag) ||
                        (a.DataRCA.Date >= azi && a.DataRCA.Date <= prag) ||
                        (a.DataRevizie.Date >= azi && a.DataRevizie.Date <= prag) ||
                        (a.DataRovinieta.Date >= azi && a.DataRovinieta.Date <= prag))
                    )
                    .ToListAsync(stoppingToken);

                foreach (var auto in masini)
                {
                    if (auto.Utilizator != null && !string.IsNullOrEmpty(auto.Utilizator.Email))
                    {
                        try
                        {

                            string detaliiExpirare = "";
                            if (auto.DataITP.Date >= azi && auto.DataITP.Date <= prag)
                            {
                                int zile = (auto.DataITP.Date - azi).Days;
                                string textZile = zile == 0 ? "expiră ASTĂZI!" : (zile == 1 ? "mai aveți o zi" : $"mai aveți {zile} zile");
                                detaliiExpirare += $"<li><b>ITP:</b> {auto.DataITP:dd.MM.yyyy} - {textZile}</li>";
                            }

                            if (auto.DataRCA.Date >= azi && auto.DataRCA.Date <= prag)
                            {
                                int zile = (auto.DataRCA.Date - azi).Days;
                                string textZile = zile == 0 ? "expiră ASTĂZI!" : (zile == 1 ? "mai aveți o zi" : $"mai aveți {zile} zile");
                                detaliiExpirare += $"<li><b>RCA:</b> {auto.DataRCA:dd.MM.yyyy} - {textZile}</li>";
                            }

                            if(auto.DataRovinieta.Date >= azi && auto.DataRovinieta.Date <= prag)
    {
                                int zile = (auto.DataRovinieta.Date - azi).Days;
                                string textZile = zile == 0 ? "expiră ASTĂZI!" : (zile == 1 ? "mai aveți o zi" : $"mai aveți {zile} zile");
                                detaliiExpirare += $"<li><b>Rovinietă:</b> {auto.DataRovinieta:dd.MM.yyyy} - {textZile}</li>";
                            }

                            if (auto.DataRevizie.Date >= azi && auto.DataRevizie.Date <= prag)
                            {
                                int zile = (auto.DataRevizie.Date - azi).Days;
                                string textZile = zile == 0 ? "expiră ASTĂZI!" : (zile == 1 ? "mai aveți o zi" : $"mai aveți {zile} zile");
                                detaliiExpirare += $"<li><b>Revizie:</b> {auto.DataRevizie:dd.MM.yyyy} - {textZile}</li>";
                            }

                            string htmlContent = $@"
                        <div style='font-family: Arial; padding: 20px; border: 1px solid #eee;'>
                            <h2 style='color: #d9534f;'>Alertă Expirare Valabilitate</h2>
                            <p><b>{auto.Utilizator.FullName}</b>,</p>
                            <p>Următoarele termene pentru vehiculul <b>{auto.Marca.NumeMarca}  {auto.Model}</b>, cu numărul de înmatriculare <b>{auto.NrInmatriculare}</b> urmează să expire:</p>
                            <ul style='list-style: none; padding-left: 0;'>
                                {detaliiExpirare}
                             </ul>
                        </div>";

                            await emailService.SendNotificationEmailAsync(
                                auto.Utilizator.Email,
                                auto.Utilizator.FullName,
                                "Notificare Expirare Valabilitate",
                                htmlContent);
                            Console.WriteLine($"[ROBOT] Email trimis cu succes către: {auto.Utilizator.Email} pentru {auto.NrInmatriculare}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ROBOT] EROARE la trimiterea mailului către {auto.Utilizator.Email}: {ex.Message}");
                        }
                    }
                }
            }
        }

    }
}
