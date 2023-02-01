using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w
namespace WarOfMind.Services.Interfaces
{
    public interface IGameService
    {
        Task<List<GameDTO>> GetAllAsync();
        Task<Game> GetByIdAsync(int id);
        Task<Game> AddAsync(Game game);
        Task<Game> UpdateAsync(Game game);
        Task DeleteByIdAsync(int id);
    }
}
