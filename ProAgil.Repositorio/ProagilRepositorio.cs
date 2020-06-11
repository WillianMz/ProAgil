using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProAgil.Dominio;

namespace ProAgil.Repositorio
{
    public class ProagilRepositorio : IProagilRepositorio
    {
        private readonly ProAgilContext _context;

        public ProagilRepositorio(ProAgilContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        //GERAL
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);            
        }
         public void Update<T>(T entity) where T : class
        {
             _context.Update(entity); 
        }
        public void Delete<T>(T entity) where T : class
        {
             _context.Remove(entity); 
        }
        
        public async Task<bool> SaveChangeAsync()
        {
           return(await _context.SaveChangesAsync()) > 0;
        }        


        //EVENTO
        public async Task<Evento[]> GetAllEventosAsync(bool includePalestrante = false)
        {
            IQueryable<Evento> qry = _context.Eventos.Include(c => c.Lotes).Include(c => c.RedesSociais);

            if(includePalestrante)
            {
                qry = qry.Include(pe => pe.PalestrantesEventos).ThenInclude(p => p.Palestrante);
            }

            qry = qry.AsNoTracking().OrderBy(c => c.EventoID);

            return await qry.ToArrayAsync();
        }

        public async Task<Evento[]> GetAllEventosAsyncByTema(string tema, bool includePalestrante)
        {
            IQueryable<Evento> qry = _context.Eventos.Include(c => c.Lotes).Include(c => c.RedesSociais);

            if(includePalestrante)
            {
                qry = qry.Include(pe => pe.PalestrantesEventos).ThenInclude(p => p.Palestrante);
            }

            qry = qry.AsNoTracking().OrderByDescending(c => c.DataEvento).Where(c => c.Tema.ToLower().Contains(tema.ToLower()));

            return await qry.ToArrayAsync();
        }

        public async Task<Evento> GetEventosAsyncByID(int EventoID, bool includePalestrante)
        {
            IQueryable<Evento> qry = _context.Eventos.Include(c => c.Lotes).Include(c => c.RedesSociais);

            if(includePalestrante)
            {
                qry = qry.Include(pe => pe.PalestrantesEventos).ThenInclude(p => p.Palestrante);
            }

            qry = qry.AsNoTracking().OrderBy(c => c.EventoID).Where(c => c.EventoID == EventoID);

            return await qry.FirstOrDefaultAsync();
        }


        //PALESTRANTE
        public async Task<Palestrante> GetPalestranteAsync(int PalestranteID, bool includeEventos = false)
        {
            IQueryable<Palestrante> qry = _context.Palestrantes.Include(c => c.RedesSociais);

            if(includeEventos)
            {
                qry = qry.Include(pe => pe.PalestrantesEventos).ThenInclude(e => e.Evento);
            }

            //ordenar por nome e consulta pelo paramentro ID
            qry = qry.AsNoTracking().OrderBy(p => p.Nome).Where(p => p.Id == PalestranteID);

            return await qry.FirstOrDefaultAsync();
        }
        public async Task<Palestrante[]> GetAllPalestrantesAsyncByName(string nome, bool includeEventos)
        {
            IQueryable<Palestrante> qry = _context.Palestrantes.Include(c => c.RedesSociais);

            if(includeEventos)
            {
                qry = qry.Include(pe => pe.PalestrantesEventos).ThenInclude(e => e.Evento);
            }

            //ordenar por nome e consulta pelo paramentro ID
            qry = qry.AsNoTracking().Where(p => p.Nome.ToLower().Contains(nome.ToLower()));

            return await qry.ToArrayAsync();
        }

    }
}