using Microsoft.EntityFrameworkCore;
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

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
