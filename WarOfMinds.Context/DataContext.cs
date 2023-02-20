using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Repositories;
using WarOfMinds.Repositories.Entities;

namespace WarOfMinds.Context
{
    public class DataContext : DbContext, IContext
    {
        
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerRatingBySubject> PlayerRatingsBySubject { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }

       

    }
}

