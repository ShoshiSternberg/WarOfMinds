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
        Task DeleteByIdAsync(int id);
        Task<GameDTO> FindGameAsync(SubjectDTO subject, PlayerDTO player);
        Task<GameDTO> GetActiveGameBySubjectAndRatingAsync(int subjectID, int rating);
        Task<GameDTO> AddGameAsync(GameDTO game);
        string Difficulty(int rating);
    }
        
}
