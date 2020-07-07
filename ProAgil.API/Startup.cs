using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProAgil.Dominio.Identity;
using ProAgil.Repositorio;

namespace ProAgil.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ProAgilContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            
            
            IdentityBuilder builder = services.AddIdentityCore<User>(options =>
                {
                    //altera a configuracao padrao para a cricao de usuarios
                    /* se nao for modificado aqui, toda vez que for criado usuarios
                    sera feita as validacoes abaixo*/
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 4;

                }
            );

            //builder.Services = ao builder criado acima
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            //fala pro entityFrameworkstores que continua sendo utilizado o meu contexto para trabalhar com os outros detalhes
            builder.AddEntityFrameworkStores<ProAgilContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            //gerenciador dos papeis
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            //JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => 
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            //validar emissor da chave que no caso é a propria API
                            ValidateIssuerSigningKey = true,
                            //chave a ser validada
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                            //
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    }
                );

            //trabalhar com MVC
            //exemplo https://minhaapi.com/api/evento -> chama a controler evento
            //esta linha determina a configuracao de qual controler sera chamada
            services.AddMvc(options => 
                {
                    //toda vez que chama alguma controle deve ser respeitada a configuracao definida abaixo
                    //requer que o usuario esteja autenticado para usar a controler
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                }
            )
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                //controla qualquer redundancia no retorno dos itens (loop infinito)
                .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            
            services.AddScoped<IProagilRepositorio, ProagilRepositorio>();            
            //referencia do automapper
            services.AddAutoMapper();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                    RequestPath = new PathString("/Resources")
                });
            app.UseMvc();
        }
    }
}
