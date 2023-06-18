using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Common.DTO;
using WarOfMinds.Repositories.Entities;
using WarOfMinds.Repositories.Interfaces;
using WarOfMinds.Repositories.Repositories;
using WarOfMinds.Services.Interfaces;


namespace WarOfMinds.Services.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;        

        public PlayerService(IPlayerRepository playerRepository, IMapper mapper,IServiceScopeFactory serviceScopeFactory)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;   
            _scopeFactory = serviceScopeFactory;
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

        public async Task<PlayerDTO> UpdatePlayerInNewScopeAsync(PlayerDTO player)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var playerRepository = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();
                    return _mapper.Map<PlayerDTO>(await playerRepository.UpdateAsync(_mapper.Map<Player>(player)));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }
        
        public async Task<PlayerDTO> GetPlayerByIdInNewScopeAsync(int player)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var playerRepository = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();
                    return _mapper.Map<PlayerDTO>(await playerRepository.GetByIdAsync(player));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }

        public async Task<PlayerDTO> GetByEmailAndPassword(string email, string password)
        {
            return _mapper.Map<PlayerDTO>(await _playerRepository.GetByEmailAndPassword(email, password));
        }

        public async Task<List<GameDTO>> GetHistory(int id)
        {
            try
            {
                PlayerDTO p = _mapper.Map<PlayerDTO>(await _playerRepository.GetWholeByIdAsync(id));
                if (p.Games == null)
                    return new List<GameDTO>();
                return p.Games.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
