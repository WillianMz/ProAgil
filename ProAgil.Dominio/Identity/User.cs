using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProAgil.Dominio.Identity
{
    public class User : IdentityUser<int>
    {
        [Column(TypeName = "nvarchar(150)")]
        public string fullName { get; set; }

        public List<UserRole> userRoles { get; set; }
    }
}