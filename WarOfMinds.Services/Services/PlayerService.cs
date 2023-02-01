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
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;

        public PlayerService(IPlayerRepository playerRepository, IMapper mapper)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;   
        }

        public async Task<PlayerDTO> AddAsync(PlayerDTO Player)
        {
            return _mapper.Map<PlayerDTO>(await _playerRepository.AddAsync(_mapper.Map<Player>(Player)));
        }

        public async Task DeleteByIdAsync(int id)
        {
            await _playerRepository.DeleteByIdAsync(id);
        }

        public async Task<List<PlayerDTO>> GetAllAsync()
        {
            return _mapper.Map<List<PlayerDTO>>(await _playerRepository.GetAllAsync());
        }

        public async Task<PlayerDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<PlayerDTO>(await _playerRepository.GetByIdAsync(id));

        }

        public async Task<PlayerDTO> UpdateAsync(PlayerDTO Player)
        {
            return _mapper.Map<PlayerDTO>(await _playerRepository.UpdateAsync(_mapper.Map<Player>(Player)));
        }
    }
}
