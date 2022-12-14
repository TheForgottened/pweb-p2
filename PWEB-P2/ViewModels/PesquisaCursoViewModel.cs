using PWEB_P2.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PWEB_P2.ViewModels
{
    public class PesquisaCursoViewModel
    {
        public List<Curso> ListaDeCursos { get; set; }
        public int NumResultados { get; set; }

        [Display(Name = "Texto", Prompt = "Introduza o texto a pesquisar")]
        public string TextoAPesquisar { get; set; }
    }
}
