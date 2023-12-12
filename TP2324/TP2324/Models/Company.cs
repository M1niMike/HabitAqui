using System.ComponentModel.DataAnnotations;

namespace TP2324.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Display(Name = "Nome da empresa/locador", Prompt = "Introduza um nome para o locador")]
        [Required(ErrorMessage = "Introduza um nome")]
        public string Name { get; set; }

        [Display(Name = "Descrição", Prompt = "Introduza uma descrição")]
        [Required(ErrorMessage = "Introduza uma descrição!")]
        public string Description { get; set; }

        public int? Rating { get; set; }

        [Display(Name = "Estado", Prompt = "Check para estado 'Ativo'")]
        public bool State { get; set; }

        [Display(Name = "Crie um domínio de correio eletrônico", Prompt = "ex: @habitaqui")]
        [RegularExpression(@"^[a-zA-Z@]+$", ErrorMessage = "O domínio de e-mail deve conter apenas letras (maiúsculas ou minúsculas) e o caractere '@'.")]
        public string? EmailDomain { get; set; }



        // Relacionamento de 1:n entre Locador(Company) e Habitação(Homes)
        public List<Home> Homes { get; set; } = new List<Home>();

        // Relacionamento de 1:N entre Locador(Company) e Gestor()
        public List<Employee> Employees { get; set; } = new List<Employee>();

        // Relacionamento de 1:n entre Locador(Company) e Funcionario(Manager)
        public List<Manager> Managers { get; set; } = new List<Manager>();
    }

}
