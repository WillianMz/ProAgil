using System.Threading.Tasks;
using ProAgil.Dominio;

namespace ProAgil.Repositorio
{
    public interface IProagilRepositorio
    {
        //GERAL
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangeAsync();

        //EVENTOS
        Task<Evento[]> GetAllEventosAsyncByTema(string tema, bool includePalestrante);
        Task<Evento[]> GetAllEventosAsync(bool includePalestrante);
        Task<Evento> GetEventosAsyncByID(int EventoID, bool includePalestrante);

        //PALESTRANTES
        Task<Palestrante[]> GetAllPalestrantesAsyncByName(string nome, bool includeEventos);
        Task<Palestrante> GetPalestranteAsync(int PalestranteID, bool includeEventos);

    }
}