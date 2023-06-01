using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Common.DTO;
using WarOfMinds.Repositories.Repositories;

namespace WarOfMinds.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<List<PlayerDTO>> GetAllAsync();
        Task<PlayerDTO> GetByIdAsync(int id);
        Task<PlayerDTO> AddAsync(PlayerDTO player);
        Task<PlayerDTO> UpdateAsync(PlayerDTO player);
        Task<PlayerDTO> GetByEmailAndPassword(string Email, string password);
        Task DeleteByIdAsync(int id);
    }
}
