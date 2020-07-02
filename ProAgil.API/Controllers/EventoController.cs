using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
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

        [HttpPost("upload")]
        public async Task<IActionResult> upload()
        {
            try
            {
                //arquivo
                var file = Request.Form.Files[0];
                //diretorio para armazenar o arquivo
                var folderName = Path.Combine("Resources", "Imagens");
                //combina o diretorio que quero salvar com o diretorio da aplicação para salvar dentro do diretorio da api
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if(file.Length > 0)
                {
                    //analiza o nome do arquivo e pega o nome dele
                    var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
                    //remove aspas duplas e espaço para montar o fullPath
                    var fullPath = Path.Combine(pathToSave, filename.Replace("\"", "").Trim());

                    //cria o arquivo
                    using(var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        //pega o arquivo recebido e salva
                        //file.CopyTo(stream);
                        await file.CopyToAsync(stream);
                    }
                }

                return Ok();
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou {ex.Message}");
            }

            //return BadRequest("Erro ao tentar realizar upload!");
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