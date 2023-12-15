using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TP2324.Models
{
    public class HomeStatus
    {
        public int Id { get; set; }

        [Display(Name = "Equipamentos", Prompt = "Que equipamentos contém esta habitação?")]
        public string? Equipments { get; set; }

        [Display(Name = "Danos", Prompt = "A habitação possui algum tipo de danos?")]
        public bool Damage { get; set; }

        [Display(Name = "Observações", Prompt = "Adicione mais algumas observações a ter em conta...")]
        public string? Observation { get; set; }

        [Display(Name = "Utilizador", Prompt = "Utilizador que realizou a reserva")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        [Display(Name = "Arrendamento", Prompt = "O arrendamento")]
        public string RentingId { get; set; }
        public Renting? Renting { get; set; }

        [NotMapped]
        [Display(Name = "Fotografias", Prompt = "Anexe fotografias do dano da habitação")]
        public IFormFile[]? Files { get; set; }

    }
}
