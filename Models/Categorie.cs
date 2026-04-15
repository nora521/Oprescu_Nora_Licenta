using System.ComponentModel.DataAnnotations;

namespace Licenta.Models
{
    public class Categorie
    {
        public int ID { get; set; }
        [Display(Name = "Categorie")]
        public string TipCategorie { get; set; }   
        
        public ICollection <AutoCategorie>? AutoCategorii { get; set; }
    }
}
