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
        Task<GameDTO> AddAsync(GameDTO game);
        Task<GameDTO> UpdateAsync(int id,GameDTO game);
        Task<GameDTO> UpdateGameAsync(int id, GameDTO game);
        Task DeleteByIdAsync(int id);
        Task<GameDTO> FindNotActiveGameAsync(SubjectDTO subject, PlayerDTO player);        
        Task<GameDTO> AddGameAsync(GameDTO game);
        Task<GameDTO> GetWholeByIdAsync(int id);    
        Task<GameDTO> GetByIdInNewScopeAsync(int id);    
        string Difficulty(int rating);
        Task<GameDTO> FindActiveGameAsync(SubjectDTO subject, PlayerDTO player);
    }
        
}
