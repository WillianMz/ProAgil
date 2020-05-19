namespace ProAgil.Dominio
{
    public class PalestranteEvento
    {
        public int PalestranteID { get; set; }
        public Palestrante Palestrante { get; set; }
        public int EventoID { get; set; }
        public Evento Evento { get; set; }
    }
}