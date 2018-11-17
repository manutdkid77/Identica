using Microsoft.EntityFrameworkCore;

namespace Identica.Entities
{
    public class IdenticaDBContext : DbContext
    {
        public IdenticaDBContext(DbContextOptions options) : base(options)
        {
            Database.Migrate();
        }
    }
}
