using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.Dominio;
using ProAgil.Repositorio;

namespace ProAgil.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase
    {
        private readonly IProagilRepositorio _repo;
        public EventoController(IProagilRepositorio repositorio)
        {
            _repo = repositorio;            
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var results = await _repo.GetAllEventosAsync(true);
                return Ok(results);
            }
            catch (System.Exception)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }            
        }

        [HttpGet("{EventoId}")]
        public async Task<IActionResult> Get(int EventoId)
        {
            try
            {
                var results = await _repo.GetEventosAsyncByID(EventoId, true);
                return Ok(results);
            }
            catch (System.Exception)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }            
        }

        [HttpGet("getByTema/{tema}")]
        public async Task<IActionResult> Get(string tema)
        {
            try
            {
                var results = await _repo.GetAllEventosAsyncByTema(tema, true);
                return Ok(results);
            }
            catch (System.Exception)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }            
        }


        [HttpPost]
        public async Task<IActionResult> Post(Evento model)
        {
            try
            {
                _repo.Add(model);            
                
                if(await _repo.SaveChangeAsync())
                {
                    return Created($"/api/evento/{model.EventoID}", model);  
                }
            }
            catch (System.Exception)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }            

            return BadRequest();
        }

        
        [HttpPut("{EventoID}")]
        public async Task<IActionResult> Put(int EventoID, Evento model)
        {
            try
            {
                var evento = await _repo.GetEventosAsyncByID(EventoID, false);
                if(evento == null) return NotFound();

                _repo.Update(model);            
                
                if(await _repo.SaveChangeAsync())
                {
                    return Created($"/api/evento/{model.EventoID}", model);  
                }
            }
            catch (System.Exception)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }            

            return BadRequest();
        }


        [HttpDelete("{EventoID}")]
        public async Task<IActionResult> Delete(int EventoID)
        {
            try
            {
                var evento = await _repo.GetEventosAsyncByID(EventoID, false);
                if(evento == null) return NotFound();

                _repo.Delete(evento);
                
                if(await _repo.SaveChangeAsync())
                {
                    return Ok();
                }
            }
            catch (System.Exception)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }            

            return BadRequest();
        }
    }
}