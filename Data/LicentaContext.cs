using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Licenta.Models;

namespace Licenta.Data
{
    public class LicentaContext : DbContext
    {
        public LicentaContext (DbContextOptions<LicentaContext> options)
            : base(options)
        {
        }

        public DbSet<Licenta.Models.Autovehicul> Autovehicul { get; set; } = default!;
        public DbSet<Licenta.Models.Marca> Marca { get; set; } = default!;
        public DbSet<Licenta.Models.Combustibil> Combustibil { get; set; } = default!;
    }
}
