using System.Collections.Generic;

namespace ProAgil.API.Dto
{
    public class PalestranteDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string ImagemURL { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public List<RedeSocialDTO> RedesSociais { get; set; }
        public List<EventoDTO> Eventos { get; set; }
    }
}