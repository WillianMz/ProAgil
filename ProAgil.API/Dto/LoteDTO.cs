namespace ProAgil.API.Dto
{
    public class LoteDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public string DataIncio { get; set; }
        public string DataFim { get; set; }
        public int Quantidade { get; set; }
    }
}