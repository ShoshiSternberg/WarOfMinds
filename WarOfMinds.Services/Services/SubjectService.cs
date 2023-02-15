using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Common.DTO;
using WarOfMinds.Repositories.Entities;
using WarOfMinds.Repositories.Interfaces;
using WarOfMinds.Services.Interfaces;

namespace WarOfMinds.Services.Services
{
    public class SubjectService : ISubjectService
    {
        public readonly ISubjectRepository _subjectRepository;
        public readonly IMapper  _mapper;
        public SubjectService(ISubjectRepository subjectRepository, IMapper mapper)
        {
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task<SubjectDTO> AddAsync(SubjectDTO Subject)
        {
            return _mapper.Map<SubjectDTO>(await _subjectRepository.AddAsync(_mapper.Map<Subject>(Subject)));
        }

        public async Task DeleteByIdAsync(int id)
        {
            await _subjectRepository.DeleteByIdAsync(id);
        }

        public async Task<List<SubjectDTO>> GetAllAsync()
        {
            return _mapper.Map<List<SubjectDTO>>(await _subjectRepository.GetAllAsync());
        }

        public async Task<SubjectDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<SubjectDTO>(await _subjectRepository.GetByIdAsync(id));

        }
        
        public async Task<SubjectDTO> GetByNameAsync(string subject)
        {
            return GetAllAsync().Result.Where(s => s.Subjectname == subject).FirstOrDefault();
        }

        public async Task<SubjectDTO> UpdateAsync(SubjectDTO Subject)
        {
            return _mapper.Map<SubjectDTO>(await _subjectRepository.UpdateAsync(_mapper.Map<Subject>(Subject)));
        }
    }
}
