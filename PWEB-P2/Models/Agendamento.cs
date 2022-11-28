namespace PWEB_P2.Models
{
    public class Agendamento
    {
        public int Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int DuracaoHoras { get; set; }
        public int DuracaoMinutos { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataHoraDoPedido { get; set; }

        public int TipoDeAulaId { get; set; }
        public TipoDeAula tipoDeAula { get; set; }

        // Relação ApplicationUser
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
