using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Repositories.Entities;


namespace WarOfMinds.Repositories.Interfaces
{
    public interface IGameRepository
    {
        Task<List<Game>> GetAllAsync();
        Task<Game> GetByIdAsync(int id);
        Task<Game> AddAsync(Game game);
        Task<Game> UpdateAsync(Game game);
        Task DeleteByIdAsync(int id);
    }
}
