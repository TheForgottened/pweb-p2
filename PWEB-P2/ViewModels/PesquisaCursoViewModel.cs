using PWEB_P2.Models;

namespace PWEB_P2.ViewModels
{
    public class PesquisaCursoViewModel
    {
        public List<Curso> ListaDeCursos { get; set; }
        public int NumResultados { get; set; }
        public string TextoAPesquisar { get; set; }
    }
}
