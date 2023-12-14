using System.ComponentModel.DataAnnotations;

namespace TP2324.Models
{
    public class Renting
    {
        public int Id { get; set; }

        [Display(Name = "Preço final")]
        public decimal? Price { get; set; }

        [Display(Name = "Data de início do arrendamento")]
        public DateTime? BeginDate { get; set; }

        [Display(Name = "Data final do arrendamento")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Status de Aprovação")]
        public bool? IsApproved { get; set; }

        // Relacionamento de 1:n entre habitação (Home) e arrendamento (Renting)
        [Display(Name = "Habitação")]
        public int? HomeId { get; set; }
        public Home Homes { get; set; }

        // Relacionamento de 1:n entre Cliente (ApplicationUser) e arrendamento (Renting)
        [Display(Name = "Cliente")]
        public string? ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }

}
