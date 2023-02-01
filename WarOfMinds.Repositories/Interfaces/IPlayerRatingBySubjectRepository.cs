using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Repositories.Entities;

namespace WarOfMinds.Repositories.Interfaces
{
    public interface IPlayerRatingBySubjectRepository
    {
        Task<List<PlayerRatingBySubject>> GetAllAsync();
        Task<PlayerRatingBySubject> GetByIdAsync(int id);
        Task<PlayerRatingBySubject> AddAsync(PlayerRatingBySubject playerRatingBySubject);
        Task<PlayerRatingBySubject> UpdateAsync(PlayerRatingBySubject playerRatingBySubject);
        Task DeleteByIdAsync(int id);
    }
}
