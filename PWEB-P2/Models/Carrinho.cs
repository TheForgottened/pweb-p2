namespace PWEB_P2.Models
{
    public class Carrinho
    {
        public List<CarrinhoItem> items { get; set; } = new List<CarrinhoItem>();

        public void AddItem(Curso curso, int qtd)
        {
            var item = items.FirstOrDefault(i => i.CursoId == curso.Id);

            if (item == null)
            {
                items.Add(new CarrinhoItem
                {
                    CursoId = curso.Id,
                    CursoNome = curso.Nome,
                    PrecoUnit = curso.Preco.GetValueOrDefault(),
                    Quantidade = qtd
                });
            }
            else
            {
                item.Quantidade += qtd;
            }
        }

        public void RemoveItem(int cursoId) => items.RemoveAll(i => i.CursoId == cursoId);
        public decimal Total() => items.Sum(i => i.PrecoUnit * i.Quantidade);
        public void Clear() => items.Clear();
    }
}
