using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Licenta.Models
{
    public class Autovehicul
    {
        public int ID { get; set; }
        public string? Poza { get; set; }
        public int? MarcaID { get; set; }
        public Marca? Marca { get; set; }
        public string Model { get; set; }

        [Display(Name = "Serie Șasiu")]
        public string SerieSasiu { get; set; }

        [Display(Name = "Nr. Înmatriculare")]
        public string NrInmatriculare { get; set; }
        public int? CombustibilID { get; set; }
        public Combustibil? Combustibil { get; set; }
        [Display(Name = "Transmisie")]

        public int? TransmisieID { get; set; }

        public Transmisie? Transmisie { get; set; }
        [Display(Name = "Nr. Locuri")]
        public int? NrLocuri { get; set; }
        [Display(Name = "Nr. Bagaje")]
        public int? NrBagaje { get; set; }
        [Display(Name = "Preț/Zi (€)")]
        public decimal? PretZi { get; set; }
        public int Kilometraj { get; set; }
        [Display(Name = "Consum Mixt(l/100 km)")]
        public decimal ConsumMixt { get; set; }
        public ICollection<AutoCategorie>? AutoCategorii { get; set; }
        public int AnFabricatie { get; set; } 
        public string Culoare { get; set; }
        public int CP { get; set; }
        public int CMC { get; set; }
   
        [Display(Name = "Dată Exp. ITP")]
        [DataType(DataType.Date)]
        public DateTime DataITP { get; set; }
        [Display(Name = "Dată Exp. RCA")]
        [DataType(DataType.Date)]
        public DateTime DataRCA { get; set; }
        [Display(Name = "Dată Exp. Rovinietă")]
        [DataType(DataType.Date)]
        public DateTime DataRovinieta { get; set; }
        [Display(Name = "Dată Revizie")]
        [DataType(DataType.Date)]
        public DateTime DataRevizie { get; set; }

        public int? UtilizatorID { get; set; }
        public Utilizator? Utilizator { get; set; }

        public ICollection<Rezervare>? Rezervari { get; set; }
        public bool Confirmare { get; set; }

    }
}
