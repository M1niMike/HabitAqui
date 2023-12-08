using System.ComponentModel.DataAnnotations;

namespace TP2324.Models
{
    public class Renting
    {
        public int Id { get; set; }
        public decimal? Price { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int MinimumPeriod { get; set; }
        public int MaximumPeriod { get; set; }


        //Relacionamento de 1:n entre habitação(home) e arrendamento(rentings)
        [Display(Name = "Habitação")]
        public int? HomeId { get; set; }
        public Home Homes { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }


    }
}
