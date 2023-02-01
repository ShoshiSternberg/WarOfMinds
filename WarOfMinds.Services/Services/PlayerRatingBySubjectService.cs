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
    public class PlayerRatingBySubjectService : IPlayerRatingBySubjectService
    {
        private readonly IPlayerRatingBySubjectRepository _playerRatingBySubjectRepository;
        private readonly IMapper _mapper;

        public PlayerRatingBySubjectService(IPlayerRatingBySubjectRepository playerRatingBySubjectRepository, IMapper mapper)
        {
            _playerRatingBySubjectRepository = playerRatingBySubjectRepository;
            _mapper = mapper;
        }

        public async Task<PlayerRatingBySubjectDTO> AddAsync(PlayerRatingBySubjectDTO PlayerRatingBySubject)
        {
            return _mapper.Map<PlayerRatingBySubjectDTO>(await _playerRatingBySubjectRepository.AddAsync(_mapper.Map<PlayerRatingBySubject>(PlayerRatingBySubject)));
        }

        public async Task DeleteByIdAsync(int id)
        {
            await _playerRatingBySubjectRepository.DeleteByIdAsync(id);
        }

        public async Task<List<PlayerRatingBySubjectDTO>> GetAllAsync()
        {
            return _mapper.Map<List<PlayerRatingBySubjectDTO>>(await _playerRatingBySubjectRepository.GetAllAsync());
        }

        public async Task<PlayerRatingBySubjectDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<PlayerRatingBySubjectDTO>(await _playerRatingBySubjectRepository.GetByIdAsync(id));
        }

        public async Task<PlayerRatingBySubjectDTO> UpdateAsync(PlayerRatingBySubjectDTO PlayerRatingBySubject)
        {
            return _mapper.Map<PlayerRatingBySubjectDTO>(await _playerRatingBySubjectRepository.UpdateAsync(_mapper.Map<PlayerRatingBySubject>(PlayerRatingBySubject)));
        }
    }
}
