using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Common.DTO;
using WarOfMinds.Repositories.Repositories;

namespace WarOfMinds.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<List<SubjectDTO>> GetAllAsync();
        Task<SubjectDTO> GetByIdAsync(int id);
        Task<SubjectDTO> AddAsync(SubjectDTO subject);
        Task<SubjectDTO> UpdateAsync(SubjectDTO subject);
        Task DeleteByIdAsync(int id);
    }
}
