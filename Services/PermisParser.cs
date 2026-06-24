using Licenta.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Licenta.Services
{
    public class PermisParser
    {
        public PermisData Parse(string text)
        {
            var data = new PermisData();

            data.DataNasterii = ExtractDateAfter(text, @"3\.");
            data.DataEmitere = ExtractDateAfter(text, @"4a\.");
            data.DataExpirare = ExtractDateAfter(text, @"4b\.");

            var cnp = Regex.Match(text, @"\b\d{13}\b");
            if (cnp.Success)
                data.CNP = cnp.Value;

            var nrPermis = Regex.Match(text, @"\b[A-Z]\d{8}[A-Z]\b");
            if (nrPermis.Success)
                data.SeriePermis = nrPermis.Value;

            var matchCat = Regex.Match(text, @"9\.\s*([A-Z0-9\s]+)");

            if (matchCat.Success)
            {
                var zona = matchCat.Groups[1].Value;

                var toateCategoriile = new[]
                {
                    "AM", "A1", "A2", "A",
                    "B1", "B", "BE",
                    "C1", "C", "CE",
                    "D1", "D", "DE",
                    "Tr", "Tb", "Tv"
                };

                var categoriiGasite = new List<string>();

                foreach (var cat in toateCategoriile)
                {
                    if (Regex.IsMatch(zona, $@"\b{Regex.Escape(cat)}\b"))
                    {
                        categoriiGasite.Add(cat);
                    }
                }

                data.Categorii = string.Join(",", categoriiGasite);
            }

            return data;
        }

        private DateTime? ExtractDateAfter(string text, string label)
        {
            var match = Regex.Match(
                text,
                label + @"\s*(\d{2}\.\d{2}\.\d{4})");

            if (!match.Success)
                return null;

            if (DateTime.TryParseExact(
                match.Groups[1].Value,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
            {
                return date;
            }

            return null;
        }
    }
}