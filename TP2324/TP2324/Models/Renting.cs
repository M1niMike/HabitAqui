using System.ComponentModel.DataAnnotations;

namespace TP2324.Models
{
    public class Renting
    {
        public int Id { get; set; }

        public decimal? Price { get; set; }

        [Display(Name = "Data de início do arrendamento")]
        public DateTime? BeginDate { get; set; }
        [Display(Name = "Data final do arrendamento")]
        public DateTime? EndDate { get; set; }
        
       


        //RELACIONAMENTOS

        //Relacionamento de 1:n entre habitação(home) e arrendamento(rentings)
        [Display(Name = "Habitação")]
        public int? HomeId { get; set; }
        public Home Homes { get; set; }


        //Relacionamento de 1:n entre Cliente(ApplicationUser) e Arrendamento(Rentings)
        public string? ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }


    }
}
