using System.ComponentModel.DataAnnotations;

namespace ProAgil.API.Dto
{
    public class RedeSocialDTO
    {        
        public int Id { get; set; }
        
        //{0} inseri automaticamento o nome da propriedade
        [Required(ErrorMessage="O campo {0} é obrigatório")]
        public string Nome { get; set; }
        
        [Required(ErrorMessage="O campo {0} é obrigatório")]
        public string URL { get; set; }
    }
}