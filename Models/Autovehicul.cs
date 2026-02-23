using System.ComponentModel.DataAnnotations;

namespace Licenta.Models
{
    public class Autovehicul
    {
        public int ID { get; set; }
        [Display(Name = "Poză")]
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
        public int Kilometraj { get; set; }
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

    }
}
