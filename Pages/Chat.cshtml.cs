using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenAI.Chat;
using Licenta.Services;
using Licenta.Data;
using Microsoft.EntityFrameworkCore;
using Licenta.Models;

namespace Licenta.Pages
{
    public class Chat : PageModel
    {
        private readonly ChatbotService _chatbot;
        private readonly LicentaContext _db;

        private const string SessionKey = "ChatHistory";
        public Chat(ChatbotService chatbot, LicentaContext db)
        {
            _chatbot = chatbot;
            _db = db;
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
                Messages.Add(new ChatMessageModel { Sender = "Bot", Text = "Bună! Cu ce te pot ajuta astăzi?" });
                SaveHistory(Messages);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Messages = LoadHistory();
            var cars = await _db.Autovehicul
                .Select(c => new {
                    c.ID,
                    Marca = c.Marca.NumeMarca,
                    c.Model,
                    Clasa = string.Join(", ", c.AutoCategorii.Select(ac => ac.Categorie.TipCategorie)),
                    PretPeZi = c.PretZi,
                    Transmisie = c.Transmisie.TipTransmisie,
                    Combustibil = c.Combustibil.TipCombustibil
                })
                .ToListAsync();

            var carsText = string.Join("\n\n", cars.Select(c =>
              "- " + c.Marca + " " + c.Model + "\n"
                ));

            var rezervari = await _db.Rezervare
                .Where(r => r.AutovehiculID != null)
                .Select(r => new {
                    r.AutovehiculID,
                    Start = r.DataStart,
                    End = r.DataFinal
                })
                .ToListAsync();

            var rezervariText = rezervari.Count == 0
                ? "Nu există rezervări în sistem."
                : string.Join("\n", rezervari.Select(r =>
                    $"Mașina {r.AutovehiculID} este ocupată între {r.Start:dd/MM/yyyy} și {r.End:dd/MM/yyyy}"
                ));

            var chatHistory = new List<ChatMessage>();

            chatHistory.Add(new SystemChatMessage($@"
Ești un chatbot inteligent pentru o aplicație de rent-a-car.
Răspunzi în română.
Folosești doar autovehiculele și rezervările din baza de date.

Ai acces la:
- lista completă de autovehicule
- toate rezervările existente
- perioadele ocupate pentru fiecare autovehicul

Reguli generale:
- Dacă utilizatorul cere o perioadă, verifici disponibilitatea.
- Un autovehicul este ocupat dacă există o rezervare care se suprapune:
   (start1 <= end2) și (end1 >= start2)
- Dacă un autovehicul este ocupat, nu îl recomanzi.
- Dacă toate sunt ocupate, spui clar.

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

Autovehicule disponibile:
{carsText}

Rezervări existente:
{rezervariText}
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

            Messages.Add(new ChatMessageModel { Sender = "Tu", Text = UserMessage });
            Messages.Add(new ChatMessageModel { Sender = "Bot", Text = reply });

            SaveHistory(Messages);

            UserMessage = "";

            return RedirectToPage();
        }
    }
}
