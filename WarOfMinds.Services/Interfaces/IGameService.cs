using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Common.DTO;

namespace WarOfMinds.Services.Interfaces
{
    public interface IGameService
    {
        Task<List<GameDTO>> GetAllAsync();
        Task<GameDTO> GetByIdAsync(int id);       
        Task<GameDTO> UpdateGameAsync(int id, GameDTO game);
        Task DeleteByIdAsync(int id);
        Task<GameDTO> FindOnHoldGameAsync(string subject, PlayerDTO player);        
        Task<GameDTO> AddGameAsync(GameDTO game);
        Task<GameDTO> GetWholeByIdAsync(int id);    
        Task<GameDTO> GetByIdInNewScopeAsync(int id);            
        Task<GameDTO> FindActiveGameAsync(string subject, PlayerDTO player);
    }
        
}
