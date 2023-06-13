using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Repositories.Entities;
using WarOfMinds.Repositories.Repositories;

namespace WarOfMinds.Repositories.Interfaces
{
    public interface IPlayerRepository
    {
        Task<List<Player>> GetAllAsync();
        Task<Player> GetByIdAsync(int id);
        Task<Player> AddAsync(Player player);
        Task<Player> UpdateAsync(Player player);
        Task<Player> GetByEmailAndPassword(string name, string password);
        Task DeleteByIdAsync(int id);
        Task<Player> GetWholeByIdAsync(int id);
    }
}
