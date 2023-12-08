using System.ComponentModel.DataAnnotations;

namespace TP2324.Models
{
    public class Home
    {
        public int Id { get; set; }

        [Display(Name = "PriceToRent", Prompt = "Introduza o valor caso for arrendar.")]
        [Required(ErrorMessage = "Indique o valor para arrendamento!")]
        public decimal? PriceToRent { get; set; }

        [Display(Name = "Casa(s) de banho", Prompt = "Indique a quantidade de casa(s) de banho.")]
        [Required(ErrorMessage = "Indique a quantidade!")]
        public int NumWC { get; set; }

        [Display(Name = "Morada", Prompt = "Introduza a morada da habitação.")]
        [Required(ErrorMessage = "Indique a morada!")]
        public string Address { get; set; }

        [Display(Name = "Metros Quadrados", Prompt = "Introduza o valor dos m2 da habitação.")]
        [Required(ErrorMessage = "Indique os m2 da habitação!")]
        public float SquareFootage { get; set; }

        [Display(Name = "Estacionamentos", Prompt = "Indique o número de lugares de estacionamento.")]
        [Required(ErrorMessage = "Indique a quantidade!")]
        public int NumParks { get; set; }

        [Display(Name = "Wifi", Prompt = "Selecione se a habitação possui WiFi.")]
        [Required(ErrorMessage = "Indique se tem Wifi!")]
        public bool Wifi { get; set; }

        [Display(Name = "Descrição", Prompt = "Insira uma descrição sobre a habitação.")]
        [Required(ErrorMessage = "Insira a descrição!")]
        public string Description { get; set; }

        [Display(Name = "Avaliação", Prompt = "Avalie essa habitação")]
        //[Required(ErrorMessage = "")]
        public int? Ratings { get; set; }

        public bool Available { get; set; }

        public string? ImgUrl { get; set; }

        [Display(Name = "Categoria")]
        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        [Display(Name = "Tipo")]
        public int? TypeResidenceId { get; set; }
        public TypeResidence typeResidence { get; set; }


        public List<Renting> Rentings { get; set; }

    }
}
