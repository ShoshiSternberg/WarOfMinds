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
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IMapper _mapper;
        public GameService(IGameRepository gameRepository, IMapper mapper)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
        }
    
        public async Task<GameDTO> AddAsync(GameDTO game)
        {
            return _mapper.Map<GameDTO>(await _gameRepository.AddAsync( _mapper.Map<Game>(game)));
        }

        public async Task DeleteByIdAsync(int id)
        {
            await _gameRepository.DeleteByIdAsync(id);
        }

        public async Task<List<GameDTO>> GetAllAsync()
        {
            return _mapper.Map<List<GameDTO>>(await _gameRepository.GetAllAsync());
        }

        public async Task<GameDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<GameDTO>(await _gameRepository.GetByIdAsync(id));
            
        }

        public async Task<GameDTO> UpdateAsync(GameDTO game)
        {
            return _mapper.Map<GameDTO>(await _gameRepository.UpdateAsync(_mapper.Map<Game>(game)));
        }

        //public async GameDTO AddPlayerToGame(Player player)
        //{
            
        //}

        //public Game GetCurrentGame(string subject)
        //{
        //    return _context.Games
        //        .Where(g => g.Subject == subject && g.IsActive)
        //        .FirstOrDefault();
        //}


    }
}
