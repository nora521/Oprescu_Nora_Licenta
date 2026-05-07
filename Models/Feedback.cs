using System.ComponentModel.DataAnnotations;

namespace Licenta.Models
{
    public class Feedback
    {
        public int ID { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comentariu { get; set; }

        public DateTime DataFeedback { get; set; } = DateTime.Now;

        public int RezervareId { get; set; }
        public Rezervare ?Rezervare { get; set; }
    }
}
