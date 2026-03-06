using System.ComponentModel.DataAnnotations;

namespace Licenta.Models
{
    public class Utilizator
    {
        public int ID { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        [Display(Name = "Conducător auto")]
        public string? FullName
        {
            get
            {
                return Nume + " " + Prenume;
            }
        }

        public string CNP { get; set; }
        public string Email { get; set; }
        public string NrTelefon { get; set; }
        public string Parola { get; set; }

        public ICollection<Autovehicul>? Autovehicule { get; set; }
    }
}
