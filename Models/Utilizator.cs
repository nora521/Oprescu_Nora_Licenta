using System.ComponentModel.DataAnnotations;

namespace Licenta.Models
{
    public class Utilizator
    {
        public int ID { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        [Display(Name = "Nume")]
        public string? FullName
        {
            get
            {
                return Nume + " " + Prenume;
            }
        }

        public string? CNP { get; set; }  
        public string Email { get; set; }
        public string NrTelefon { get; set; }
        public string Parola { get; set; }

        public string? PermisFataPath { get; set; }

        public string? PermisVersoPath { get; set; }

        public string? SeriePermis { get; set; }

        public DateTime? DataNasterii { get; set; }

        public DateTime? DataEmiterePermis { get; set; }

        public DateTime? DataExpirarePermis { get; set; }

        public string? CategoriiPermis { get; set; }

        public bool PermisVerificat { get; set; }


        public ICollection<Autovehicul>? Autovehicule { get; set; }

        public ICollection<Rezervare>? Rezervari { get; set; }
    }
}
