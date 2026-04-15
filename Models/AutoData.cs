namespace Licenta.Models
{
    public class AutoData
    {
        public IEnumerable<Autovehicul> Autovehicule { get; set; }
        public IEnumerable<Categorie> Categorii { get; set; }
        public IEnumerable <AutoCategorie> AutoCategorii { get; set; }
    }
}
