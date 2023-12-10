namespace TP2324.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Rating { get; set; }
        public bool State { get; set; }

        // Relacionamento de 1:n entre Locador(Company) e Habitação(Homes)
        public List<Home> Homes { get; set; } = new List<Home>();

        // Relacionamento de 1:N entre Locador(Company) e Gestor()
        public List<Employee> Employees { get; set; } = new List<Employee>();

        // Relacionamento de 1:n entre Locador(Company) e Funcionario()
        public List<Manager> Managers { get; set; } = new List<Manager>();
    }

}
