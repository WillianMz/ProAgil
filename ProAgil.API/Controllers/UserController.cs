using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProAgil.API.Dto;
using ProAgil.Dominio.Identity;

namespace ProAgil.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public UserController(IConfiguration config, UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            return Ok(new UserDTO());
        }

        [HttpPost("Register")]
        [AllowAnonymous]//nao solicita a autenticacao pois este metodo e para criacao de usuario -> permite ser anonimo
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            try
            {
                //faz o mapeamento do JSON recebido e mapeia para user e preenche alguns campos
                var user = _mapper.Map<User>(userDTO);
                //regista o user no banco de dados
                var result = await _userManager.CreateAsync(user, userDTO.password);
                //retorna o user
                var userToReturn = _mapper.Map<UserDTO>(user);

                //se teve sucesso ao inserir no banco de dados
                if(result.Succeeded)
                {
                    return Created("GetUser", userToReturn);                        
                }
                
                //qualquer tipo de erro
                return BadRequest(result.Errors);

            }   
            catch(Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou: {ex.Message}");
            }  
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO userLogin)
        {
            try
            {
                //verifica no banco de dados se tem usuario com o nome informado
                var user = await _userManager.FindByNameAsync(userLogin.userName);
                //se tem usuario com o nome informado ele verifica o password informado com o que esta no banco de dados
                //e nao trava o banco para fazer a consulta (parametro false)
                var result = await _signInManager.CheckPasswordSignInAsync(user, userLogin.password, false);

                if(result.Succeeded)
                {
                    //retorna o FirstOrDefaultAsync caso o NormalizedUserName seja igual ao userLogin UserName que foi encontrado
                    //retorna o usuario que foi encontrado na base de dados
                    var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == userLogin.userName.ToUpper());
                    var userToReturn = _mapper.Map<UserLoginDTO>(appUser);

                    return Ok(new 
                        {
                            //gera o token com base no usuario que foi encontrado pelo _userManager
                            token = GenerateJWtToken(appUser).Result,
                            user = userToReturn
                        }
                    );
                }

                //caso o login de errado retorna nao autorizado
                return Unauthorized();
            }
            catch( Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou: {ex.Message}");
            }
        }

        private async Task<string> GenerateJWtToken(User user)
        {
            //
            var claims = new List<Claim>
            {
                //adiciona as claims detalhes para a autorizacao
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            //retorna os papeis que o usuario possui
            //exemplo: Admin, Gerente, Operador, etc
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {//adiona as claims m
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //chave
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //cria o algoritmo para assinar
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //monta o token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),//data de expiracao
                SigningCredentials = creds //credecial
            };
            //manipulador de token
            var tokenHandler = new JwtSecurityTokenHandler();
            //ao criar o token usa a descricao criada anteriormente
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //retorna o token
            return tokenHandler.WriteToken(token);

        }
    }
}