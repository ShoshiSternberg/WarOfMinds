using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Repositories.Entities;

namespace WarOfMinds.Repositories
{
    public interface IContext 
    {
        DbSet<Game> Games { get; set; }
        
        DbSet<Player> Players { get; set; }        

        DbSet<Subject> Subjects { get; set; }
        DbSet<GamePlayer> GamePlayer { get; set; }
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        
        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
