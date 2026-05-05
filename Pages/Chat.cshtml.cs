using Licenta.Data;
using Licenta.Migrations;
using Licenta.Models;
using Licenta.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;

namespace Licenta.Pages
{
    public class Chat : PageModel
    {
        private readonly ChatbotService _chatbot;
        private readonly LicentaContext _db;

        private const string SessionKey = "ChatHistory";

        private readonly EmailService _email;

        public Chat(ChatbotService chatbot, LicentaContext db, EmailService email)
        {
            _chatbot = chatbot;
            _db = db;
            _email = email;
        }


        private List<ChatMessageModel> LoadHistory()
        {
            var data = HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(data))
                return new List<ChatMessageModel>();

            return System.Text.Json.JsonSerializer.Deserialize<List<ChatMessageModel>>(data);
        }

        private void SaveHistory(List<ChatMessageModel> history)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(history);
            HttpContext.Session.SetString(SessionKey, json);
        }

        [BindProperty]
        public string UserMessage { get; set; }
        public List<ChatMessageModel> Messages { get; set; } = new();

        public void OnGet()
        {
            Messages = LoadHistory();
            if (Messages.Count == 0)
            {
                Messages.Add(new ChatMessageModel
                {
                    Sender = "Bot",
                    Text = "Bună! Cu ce te pot ajuta astăzi?"
                });
                SaveHistory(Messages);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Messages = LoadHistory();

            var cars = await _db.Autovehicul
                .Select(c => new
                {
                    c.ID,
                    Marca = c.Marca.NumeMarca,
                    c.Model,
                    Clasa = string.Join(", ", c.AutoCategorii.Select(ac => ac.Categorie.TipCategorie)),
                    PretPeZi = c.PretZi,
                    Transmisie = c.Transmisie.TipTransmisie,
                    Combustibil = c.Combustibil.TipCombustibil
                })
                .ToListAsync();

            var carsText = System.Text.Json.JsonSerializer.Serialize(cars);

            var rezervari = await _db.Rezervare
                .Where(r => r.AutovehiculID != null)
                .Select(r => new
                {
                    r.AutovehiculID,
                    Start = r.DataStart,
                    End = r.DataFinal
                })
                .ToListAsync();

            var chatHistory = new List<ChatMessage>();

            chatHistory.Add(new SystemChatMessage($@"
Ești un chatbot inteligent pentru o aplicație de rent-a-car. 
Răspunzi în română. 
Folosești doar autovehiculele și rezervările din baza de date. 
Ai acces la: 
- lista completă de autovehicule 
- toate rezervările existente 
- perioadele ocupate pentru fiecare autovehicul

Autovehicule disponibile: 
{carsText} 

Rolul tău este:
- să înțelegi ce autovehicul vrea utilizatorul (după marcă și model)
- să înțelegi perioada dorită (data de început și data de sfârșit)
- să întorci un JSON VALID pentru backend atunci când utilizatorul vrea să facă o rezervare.

Reguli pentru recomandări:
- Dacă utilizatorul cere autovehicule pentru munte, recomanzi autovehicule din clasa SUV.
- Dacă utilizatorul cere autovehicule pentru oraș, recomanzi autovehicule din clasa Economy/Compact sau cu combustibil Hybrid.
- Dacă utilizatorul cere autovehicule pentru autostradă, recomanzi autovehicule din clasa Sedan, Break sau Hatchback.
- Dacă utilizatorul cere autovehicule pentru familie, recomanzi autovehicule din clasa Break sau SUV.
- Nu recomanzi autovehicule nepotrivite pentru context.
- Dacă utilizatorul cere detalii despre un anumit model, îi oferi detalii despre transmisie, combustibil și preț.
- Dacă utilizatorul cere detalii despre preț, îi spui prețul pe zi, precum și garanția pentru clasa din care face parte autovehiculul.

Reguli de afișare: 
- Nu afișezi toate autovehiculele. 
- Afișezi maximum 3–5 autovehicule relevante pentru cererea utilizatorului. 
- Fiecare autovehicul trebuie afișat pe linii separate, exact în formatul primit. 
- Nu comprima lista într-un singur paragraf. 
- Nu afișezi și clasa autovehiculelor atunci când le enumeri/ oferi recomandări.

NU verifici disponibilitatea autovehiculelor.
NU decizi dacă un autovehicul este ocupat sau liber.
NU verifici suprapuneri de rezervări.
Doar extragi intenția și construiești JSON-ul.

Când utilizatorul confirmă că vrea să facă o rezervare,
întoarce STRICT un JSON VALID, fără text înainte sau după, în formatul:

{{
  ""action"": ""create_reservation"",
  ""autovehiculId"": 123,
  ""start"": ""2026-05-10"",
  ""end"": ""2026-05-15""
}}

- ""autovehiculId"" trebuie să fie un ID din lista de autovehicule.
- ""start"" și ""end"" trebuie să fie în format ""yyyy-MM-dd"".

Dacă utilizatorul NU vrea să facă o rezervare, întoarce:

{{
  ""action"": ""none""
}}

NU adăuga explicații.
NU adăuga text în afara JSON-ului.
NU adăuga alte câmpuri.
"));

            foreach (var m in Messages)
            {
                if (string.IsNullOrWhiteSpace(m.Text))
                    continue;

                if (m.Sender == "Tu")
                    chatHistory.Add(new UserChatMessage(m.Text));
                else
                    chatHistory.Add(new AssistantChatMessage(m.Text));
            }

            chatHistory.Add(new UserChatMessage(UserMessage));

            var reply = await _chatbot.AskAsync(chatHistory);

            if (reply.TrimStart().StartsWith("{"))
            {
                try
                {
                    var json = System.Text.Json.JsonDocument.Parse(reply);
                    var action = json.RootElement.GetProperty("action").GetString();

                    if (action == "create_reservation")
                    {
                        int carId = json.RootElement.GetProperty("autovehiculId").GetInt32();
                        DateTime start = DateTime.Parse(json.RootElement.GetProperty("start").GetString());
                        DateTime end = DateTime.Parse(json.RootElement.GetProperty("end").GetString());

                        bool overlap = await _db.Rezervare.AnyAsync(r =>
                            r.AutovehiculID == carId &&
                            start <= r.DataFinal &&
                            end >= r.DataStart
                        );

                        if (overlap)
                        {
                            Messages.Add(new ChatMessageModel
                            {
                                Sender = "Tu",
                                Text = UserMessage
                            });

                            Messages.Add(new ChatMessageModel
                            {
                                Sender = "Bot",
                                Text = "Mașina aleasă este ocupată în perioada selectată. Te rog să alegi altă perioadă sau alt autovehicul."
                            });

                            SaveHistory(Messages);
                            return RedirectToPage();
                        }

                        var pretZi = await _db.Autovehicul
                            .Where(a => a.ID == carId)
                            .Select(a => a.PretZi)
                            .FirstAsync();

                        var userEmail = User.Identity.Name;
                        var user = await _db.Utilizator.FirstOrDefaultAsync(u => u.Email == userEmail);
                 
                        var masina = await _db.Autovehicul
                        .Include(a => a.AutoCategorii)
                        .ThenInclude(ac => ac.Categorie)
                        .FirstOrDefaultAsync(a => a.ID == carId);

                        var categorii = masina.AutoCategorii
                            .Select(ac => ac.Categorie.TipCategorie)
                            .ToList();

                        decimal garantieMax = 0;

                        foreach (var cat in categorii)
                        {
          
                            decimal garantie = cat switch
                            {
                                "Mică/Economy" => 100,
                                "Hatchback/Compact" => 200,
                                "Sedan" => 300,
                                "SUV" => 500,
                                "Break/Wagon" => 300,
                                "Premium/Luxury" => 600,
                                _ => 0
                            };

                            if (garantie > garantieMax)
                                garantieMax = garantie;
                        }

                        var rezervareNoua = new Rezervare
                        {
                            AutovehiculID = carId,
                            DataStart = start,
                            DataFinal = end,
                            PretZi = pretZi,
                            PretTotal = pretZi * (decimal)(end - start).TotalDays + garantieMax,
                            UtilizatorID = user.ID,
                            Garantie = garantieMax
                        };

                        _db.Rezervare.Add(rezervareNoua);
                        await _db.SaveChangesAsync();

                        await _db.Entry(rezervareNoua)
                            .Reference(r => r.Autovehicul)
                            .LoadAsync();

                        await _db.Entry(rezervareNoua.Autovehicul)
                            .Reference(a => a.Marca)
                            .LoadAsync();

                        await _email.TrimiteEmailRezervareAsync(
                            user.Email,
                            user.FullName, 
                            rezervareNoua
                        );


                        Messages.Add(new ChatMessageModel
                        {
                            Sender = "Tu",
                            Text = UserMessage
                        });

                        Messages.Add(new ChatMessageModel
                        {
                            Sender = "Bot",
                            Text = $"Rezervarea a fost creată cu succes! ID rezervare: {rezervareNoua.ID}"
                        });

                        SaveHistory(Messages);
                        return RedirectToPage();
                    }
                }
                catch
                {

                }
            }

            Messages.Add(new ChatMessageModel { Sender = "Tu", Text = UserMessage });
            Messages.Add(new ChatMessageModel { Sender = "Bot", Text = reply });

            SaveHistory(Messages);
            UserMessage = "";

            return RedirectToPage();
        }
    }
}
