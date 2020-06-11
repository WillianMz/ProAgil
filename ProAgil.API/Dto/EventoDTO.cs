using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProAgil.API.Dto
{
    public class EventoDTO
    {
        public int EventoID { get; set; }

        [Required(ErrorMessage="Campo Obrigatório!")]
        [StringLength(100, MinimumLength=3, ErrorMessage="Local é entre 3 e 100 caracteres")]
        public string Local { get; set; }
        public string DataEvento { get; set; }

        [Required(ErrorMessage= "O tema é obrigatório!")]
        public string Tema { get; set; }
        
        [Range(2, 120000, ErrorMessage="Qtd. de pessoas é entre 2 e 120000")]
        public int QtdPessoas { get; set; }
        public string ImagemURL { get; set; }

        [Phone]
        public string Telefone { get; set; }

        [EmailAddress]//data annotations
        public string Email { get; set; }
        public List<LoteDTO> Lotes { get; set; }
        public List<RedeSocialDTO> RedesSociais { get; set; }
        public List<PalestranteDTO> Palestrantes { get; set; }
    }
}