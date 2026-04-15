namespace Licenta.Models
{
    public class AutoCategorie
    {
        public int ID { get; set; }
        public int CategorieID { get; set; }
        public Categorie Categorie { get; set; }

        public int AutovehiculID { get; set; }
        public Autovehicul Autovehicul { get; set; }
    }
}
