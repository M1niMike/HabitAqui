using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP2324.Models
{
    public class Home
    {
        public int Id { get; set; }

        [Display(Name = "Preço por dia", Prompt = "Introduza o valor para arrendar.")]
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

        [Display(Name = "Possui Wifi?", Prompt = "Selecione se a habitação possui WiFi.")]
        [Required(ErrorMessage = "Indique se tem Wifi!")]
        public bool Wifi { get; set; }

        [Display(Name = "Descrição", Prompt = "Insira uma descrição sobre a habitação.")]
        [Required(ErrorMessage = "Insira a descrição!")]
        public string Description { get; set; }

        [Display(Name = "Período Minimo", Prompt = "Insira o período mínimo para arrendar.")]
        [Required(ErrorMessage = "Insira o período")]
        public int MinimumPeriod { get; set; }

        [Display(Name = "Avaliação", Prompt = "Avalie essa habitação")]
        //[Required(ErrorMessage = "")]
        public int? Ratings { get; set; }

        [Display(Name = "Está Disponível?")]
        public bool Available { get; set; }

        [Display(Name = "Imagem")]
        public string? ImgUrl { get; set; }

        [NotMapped] // Esta propriedade não será mapeada para o banco de dados
        [Display(Name = "Carregar Imagem")]
        public IFormFile? ImageFile { get; set; }



        //RELACIONAMENTOS


        //Relacionamento de 1:n entre Categoria(Category) e Habitação(Home) 
        [Display(Name = "Categoria")]
        public int? CategoryId { get; set; }
        [Display(Name = "Categoria da habitação")]
        public Category Category { get; set; }


        //Relacionamento de 1:n entre Type(Category) e Habitação(Home) 
        [Display(Name = "Tipo")]
        public int? TypeResidenceId { get; set; }
        [Display(Name = "Tipo da habitação")]
        public TypeResidence typeResidence { get; set; }


        [Display(Name = "Distrito")]
        public int? DistrictId { get; set; }
        [Display(Name = "Distrito da habitação")]
        public District District { get; set; }

        [Display(Name = "Locador/Empresa")]
        public int? CompanyId { get; set; }
        [Display(Name = "Locador/Empresa da habitação")]
        public Company Company { get; set; }

        //Relacionamento de 1:n entre Habitação(Home) e Arrendamentos(Rentings)
        public List<Renting> Rentings { get; set; }

    }
}
