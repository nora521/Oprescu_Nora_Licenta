using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Licenta.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Licenta.Data
{
    public class LicentaContext : IdentityDbContext
    {
        public LicentaContext (DbContextOptions<LicentaContext> options)
            : base(options)
        {
        }

        public DbSet<Licenta.Models.Autovehicul> Autovehicul { get; set; } = default!;
        public DbSet<Licenta.Models.Marca> Marca { get; set; } = default!;
        public DbSet<Licenta.Models.Combustibil> Combustibil { get; set; } = default!;
        public DbSet<Licenta.Models.Utilizator> Utilizator { get; set; } = default!;
        public DbSet<Licenta.Models.Rezervare> Rezervare { get; set; } = default!;
        public DbSet<Licenta.Models.Transmisie> Transmisie { get; set; } = default!;
        public DbSet<Licenta.Models.Categorie> Categorie { get; set; } = default!;
        public DbSet<Licenta.Models.AutoCategorie> AutoCategorie { get; set; } = default!;
        public DbSet<Licenta.Models.Feedback> Feedback { get; set; } = default!;
    }
}
