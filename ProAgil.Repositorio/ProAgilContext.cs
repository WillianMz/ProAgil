using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProAgil.Dominio;
using ProAgil.Dominio.Identity;

namespace ProAgil.Repositorio
{
    public class ProAgilContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, 
                                                    UserRole, IdentityUserLogin<int>, 
                                                    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public ProAgilContext(DbContextOptions<ProAgilContext> options) : base(options){} 
         public DbSet<Evento> Eventos { get; set; }
        public DbSet<Palestrante> Palestrantes { get; set; }
        public DbSet<PalestranteEvento> PalestranteEventos { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<RedeSocial> RedesSociais { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //relacionamento de usuario com seu papel
            modelBuilder.Entity<UserRole>(userRole => 
                {
                    userRole.HasKey(ur => new {ur.UserId, ur.RoleId});
                    userRole.HasOne(ur => ur.role).WithMany( r => r.userRoles).HasForeignKey(ur => ur.RoleId).IsRequired();
                    userRole.HasOne(ur => ur.user).WithMany( r => r.userRoles).HasForeignKey(ur => ur.UserId).IsRequired();
                }
            );

            //relacionamento N pra N
            modelBuilder.Entity<PalestranteEvento>().HasKey(PE => new {PE.EventoID, PE.PalestranteID});
        }
    }
}