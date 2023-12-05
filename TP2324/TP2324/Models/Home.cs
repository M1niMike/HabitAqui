using System.ComponentModel.DataAnnotations;

namespace TP2324.Models
{
    public class Home
    {
        public int Id { get; set; }

        //Apagar
        [Display(Name = "PriceToRent", Prompt = "Introduza o valor caso for arrendar.")]
        [Required(ErrorMessage = "Indique o valor para arrendamento!")]
        public int PriceToRent { get; set; }

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

        //Apagar
        [Display(Name = "Data de Início", Prompt = "Introduza a Data do início do contrato.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BeginDate { get; set; }

        //Apagar
        [Display(Name = "Data de Fim", Prompt = "Introduza a Data do final do contrato.")]
        [Required(ErrorMessage = "Indique a data de Início!")]
        public DateTime EndDate { get; set; }

        //Apagar
        [Display(Name = "Périodo minímo de contrato", Prompt = "Introduza o périodo minímo de contrato.")]
        [Required(ErrorMessage = "Indique o périodo minímo de contrato!")]
        public int MinimumPeriod { get; set; }

        //Se a habitação está disponivel -> possivelmente apagar e adicionar ou no renting ou contract
        public bool Available { get; set; }

        public string? ImgUrl { get; set; }

        [Display(Name = "Categoria")]
        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        [Display(Name = "Tipo")]
        public int? TypeResidenceId { get; set; }
        public TypeResidence typeResidence { get; set; }

    }
}
