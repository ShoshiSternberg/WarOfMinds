using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WarOfMinds.Repositories;
using WarOfMinds.Repositories.Entities;

namespace WarOfMinds.Context
{
    public class DataContext : DbContext, IContext
    {

        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<GamePlayer> GamePlayer { get; set; }
        public DataContext(DbContextOptions<DataContext> options)
        : base(options)
        {

        }
        public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            return base.Entry(entity);
        }
        public ChangeTracker ChangeTracker => base.ChangeTracker;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                 .Property(x => x.GameID)
                 .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity<Player>()
                            .Property(x => x.PlayerID)
                            .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
            modelBuilder.Entity<Subject>()
                            .Property(x => x.SubjectID)
                            .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity<GamePlayer>()
                .HasKey(pg => new { pg.PlayerId, pg.GameId });

            modelBuilder.Entity<GamePlayer>()
                .HasOne(pg => pg.GPlayer)
                .WithMany(p => p.Games)
                .HasForeignKey(pg => pg.PlayerId);

            modelBuilder.Entity<GamePlayer>()
                .HasOne(pg => pg.PGame)
                .WithMany(g => g.Players)
                .HasForeignKey(pg => pg.GameId);



        }
    }

}



