using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.API.Dto;
using ProAgil.Dominio;
using ProAgil.Repositorio;

namespace ProAgil.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase
    {
        private readonly IProagilRepositorio _repo;
        private readonly IMapper _mapper;
        public EventoController(IProagilRepositorio repositorio, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repositorio;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var eventos = await _repo.GetAllEventosAsync(true);
                //var results = _mapper.Map<IEnumerable<EventoDTO>>(eventos);
                var results = _mapper.Map<EventoDTO[]>(eventos);

                return Ok(results);
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou {ex.Message}");
            }
        }

        [HttpGet("{EventoId}")]
        public async Task<IActionResult> Get(int EventoId)
        {
            try
            {
                var evento = await _repo.GetEventosAsyncByID(EventoId, true);
                var results = _mapper.Map<EventoDTO>(evento);

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
                var eventos = await _repo.GetAllEventosAsyncByTema(tema, true);
                var results = _mapper.Map<EventoDTO[]>(eventos);
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post(EventoDTO model)
        {
            try
            {
                //mapeamento inverso EventoDTO -> Evento
                var evento = _mapper.Map<Evento>(model);
                
                _repo.Add(evento);

                if (await _repo.SaveChangeAsync())
                {
                    return Created($"/api/evento/{model.EventoID}", _mapper.Map<EventoDTO>(evento));
                }
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou {ex.Message}");
            }

            return BadRequest();
        }


        [HttpPut("{EventoID}")]
        public async Task<IActionResult> Put(int EventoID, EventoDTO model)
        {
            try
            {
                var evento = await _repo.GetEventosAsyncByID(EventoID, false);
                if (evento == null) return NotFound();

                _mapper.Map(model, evento);
                _repo.Update(evento);

                if (await _repo.SaveChangeAsync())
                {
                    return Created($"/api/evento/{model.EventoID}", _mapper.Map<EventoDTO>(evento));
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
                if (evento == null) return NotFound();

                _repo.Delete(evento);

                if (await _repo.SaveChangeAsync())
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