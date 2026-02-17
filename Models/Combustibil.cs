using System.ComponentModel.DataAnnotations;

namespace Licenta.Models
{
    public class Combustibil
    {
        public int ID { get; set; }
        [Display(Name = "Combustibil")]
        public string TipCombustibil { get; set; }
        public ICollection<Autovehicul>? Autovehicule { get; set; }
    }
}
