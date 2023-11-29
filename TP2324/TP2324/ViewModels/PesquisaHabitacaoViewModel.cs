using System.ComponentModel.DataAnnotations;
using TP2324.Models;

namespace TP2324.ViewModels
{
    public class PesquisaHabitacaoViewModel
    {
        public List<Home> Homeslist { get; set; }

        public int NumResultados { get; set; }

        [Display(Name = "Pesquisa", Prompt ="Pesquise uma habitação")]
        public string TextoAPesquisar { get; set; }

    }
}
