using System.ComponentModel.DataAnnotations;
using TP2324.Models;

namespace TP2324.ViewModels
{
    public class ManagerAreaViewModel
    {
        public Manager? Manager { get; set; }

        public Employee? Employee { get; set; }

        //public List<Company>? companiesList { get; set; }


        [Display(Name = "Primeiro nome", Prompt = "O primeiro nome")]
        [Required(ErrorMessage = "Introduza um nome")]
        public string FirstName { get; set; }

        [Display(Name = "Ultimo nome", Prompt = "O ultimo nome")]
        [Required(ErrorMessage = "Introduza um nome")]
        public string LastName { get; set; }

        [Display(Name = "UserName", Prompt = "Introduza um email")]
        public string? UserName { get; set; }

        [Display(Name = "Senha", Prompt = "Introduza uma senha")]
        [Required(ErrorMessage = "Introduza uma senha")]
        public string Password { get; set; }
    }
}
