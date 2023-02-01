using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Common.DTO;
using WarOfMinds.Repositories.Repositories;

namespace WarOfMinds.Services.Interfaces
{
    public interface IPlayerRatingBySubjectService
    {
        Task<List<PlayerRatingBySubjectDTO>> GetAllAsync();
        Task<PlayerRatingBySubjectDTO> GetByIdAsync(int id);
        Task<PlayerRatingBySubjectDTO> AddAsync(PlayerRatingBySubjectDTO playerRatingBySubject);
        Task<PlayerRatingBySubjectDTO> UpdateAsync(PlayerRatingBySubjectDTO playerRatingBySubject);
        Task DeleteByIdAsync(int id);
    }
}
