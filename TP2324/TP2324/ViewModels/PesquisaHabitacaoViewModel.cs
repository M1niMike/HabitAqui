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
        public string TipoResidenciaSelecionado { get; set; }
        public string CategoriaSelecinada { get; set; }
        public string PeriodoMinimoSelecionado { get; set; }
        public string LocalizacaoSelecionada { get; set; }

        public decimal? PrecoMinimo { get; set; }
        public decimal? PrecoMaximo { get; set; }

        public string Ordenacao { get; set; }

    }
}
