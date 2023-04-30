using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Repositories.Entities;

namespace WarOfMinds.Repositories.Interfaces
{
    public interface IGamePlayerRepository
    {
        Task<List<GamePlayer>> GetAllAsync();
        Task<GamePlayer> GetByIdAsync(int id);
        Task<GamePlayer> AddAsync(GamePlayer GamePlayer);
        Task<GamePlayer> UpdateAsync(GamePlayer GamePlayer);
        Task DeleteByIdAsync(int id);
    }
}
