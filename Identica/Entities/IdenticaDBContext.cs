using Identica.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identica.Entities
{
    public class IdenticaDBContext : IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public IdenticaDBContext(DbContextOptions options) : base(options)
        {
            Database.Migrate();
        }
    }
}
