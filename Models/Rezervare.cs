using System.ComponentModel.DataAnnotations;

namespace Licenta.Models
{
    public class Rezervare
    {
        public int ID { get; set; }

        [Display(Name = "Nume")]
        public int? UtilizatorID { get; set; }
        public Utilizator? Utilizator { get; set; }

        public int? AutovehiculID { get; set; }
        public Autovehicul? Autovehicul { get; set; }

        [Display(Name = "Dată Start Rezervare")]
        public DateTime DataStart { get; set; }

        [Display(Name = "Dată Final Rezervare")]
        public DateTime DataFinal { get; set; }

        [Display(Name = "Preț/Zi (€)")]
        public decimal PretZi { get; set; }

        [Display(Name = "Garanție (€)")]
        public decimal Garantie { get; set; }
        [Display(Name = "Preț Total (€)")]
        public decimal PretTotal { get; set; }


    }
}
