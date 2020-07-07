using System.Linq;
using AutoMapper;
using ProAgil.API.Dto;
using ProAgil.Dominio;
using ProAgil.Dominio.Identity;

namespace ProAgil.API.Helpers
{
    public class AutoMapperProfiles : Profile
    { 
        public AutoMapperProfiles()
        {
            CreateMap<Evento, EventoDTO>()
                .ForMember(dest => dest.Palestrantes, opt => 
                {
                    opt.MapFrom(src => src.PalestrantesEventos.Select(x => x.Palestrante).ToList());
                })
                .ReverseMap();
            
            //Mapemento reverso
            //CreateMap<EventoDTO, Evento>();

            //relacionamentos muitos pra muitos
            CreateMap<Palestrante, PalestranteDTO>()
                .ForMember(dest => dest.Eventos, opt =>
                {
                    opt.MapFrom(src => src.PalestrantesEventos.Select(x => x.Evento).ToList());
                })
                .ReverseMap();

            CreateMap<Lote, LoteDTO>().ReverseMap();
            CreateMap<RedeSocial, RedeSocialDTO>().ReverseMap();
            //CreateMap<LoteDTO, Lote>();            
            //CreateMap<RedeSocialDTO, RedeSocial>();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserLoginDTO>().ReverseMap();
        }        
    }
}