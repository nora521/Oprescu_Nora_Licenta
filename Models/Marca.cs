using System.ComponentModel.DataAnnotations;

namespace Licenta.Models
{
    public class Marca
    {
        public int ID { get; set; }
        [Display(Name = "Marcă")]
        public string NumeMarca { get; set; } 

        public ICollection<Autovehicul>? Autovehicule { get; set; }
    }
}
