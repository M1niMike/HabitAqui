using System.ComponentModel.DataAnnotations;

namespace TP2324.Models
{
    public class Home
    {
        public int Id { get; set; }

        [Display(Name = "Tipo de habitação", Prompt ="Selecione o tipo da habitação, ex: Apartamento, Moradia...")]
        [Required(ErrorMessage = "Indique o tipo da habitação!")]
        public string Type { get; set; }

        [Display(Name = "toRent", Prompt = "Selecione se a habitação é para arrendar.")]
        [Required(ErrorMessage = "Indique se a habitação é para arrendar!")]
        public bool toRent { get; set; }

        [Display(Name = "toPurchase", Prompt = "Selecione se a habitação é para vender.")]
        [Required(ErrorMessage = "Indique se a habitação é para vender!")]
        public bool toPurchase { get; set; }

        [Display(Name = "PriceToRent", Prompt = "Introduza o valor caso for arrendar.")]
        [Required(ErrorMessage = "Indique o valor para arrendamento!")]
        public int PriceToRent { get; set; }

        [Display(Name = "PriceToPurchase", Prompt = "Introduza o valor caso for vender.")]
        [Required(ErrorMessage = "Indique o valor para venda!")]
        public int PriceToPurchase { get; set; }

        // public int NumRooms { get; set; }

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

        [Display(Name = "Data de Início", Prompt = "Introduza a Data do início do contrato.")]
        public DateTime BeginDate { get; set; }

        [Display(Name = "Data de Fim", Prompt = "Introduza a Data do final do contrato.")]
        [Required(ErrorMessage = "Indique a data de Início!")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Périodo minímo de contrato", Prompt = "Introduza o périodo minímo de contrato.")]
        [Required(ErrorMessage = "Indique o périodo minímo de contrato!")]
        public int MinimumPeriod { get; set; }

        //Se a habitação está disponivel
        public bool Available { get; set; }

        public string ImgUrl { get; set; }

        [Display(Name = "Categoria")]
        public int? CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
