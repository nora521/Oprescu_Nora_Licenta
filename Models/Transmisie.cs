using System.ComponentModel.DataAnnotations;

namespace Licenta.Models
{
    public class Transmisie
    {
        public int ID { get; set; }
        [Display(Name = "Transmisie")]
        public String TipTransmisie { get; set; }  
    }
}
