using System;
using System.ComponentModel.DataAnnotations;
using TP2324.Models;

namespace TP2324.ViewModels
{
    public class CreateCompanyViewModel
    {
        public Company Company { get; set; }


        [Display(Name = "Primeiro nome", Prompt = "O primeiro nome")]
        [Required(ErrorMessage = "Introduza um nome")]
        public string FirstName { get; set; }

        [Display(Name = "Ultimo nome", Prompt = "O ultimo nome")]
        [Required(ErrorMessage = "Introduza um nome")]
        public string LastName { get; set; }

        [Display(Name = "Email", Prompt = "Introduza um email")]
        [Required(ErrorMessage = "Introduza um email")]
        public string UserName { get; set; }

        [Display(Name = "Senha", Prompt = "Introduza uma senha")]
        [Required(ErrorMessage = "Introduza uma senha")]
        public string Password { get; set; }

        public string? Name { get; set; }
        public string? TextoAPesquisar { get; set; }
        public string? Ordenacao { get; set; }

    }
}
