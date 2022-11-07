﻿namespace PWEB_P2.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public bool Disponivel { get; set; }

        public ICollection<Curso> Cursos { get; set; }
    }
}
