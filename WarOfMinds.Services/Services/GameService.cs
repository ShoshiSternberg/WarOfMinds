using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        private readonly ISubjectService _subjectService;
        private readonly IMapper _mapper;
        public GameService(IGameRepository gameRepository, ISubjectService subjectService, IMapper mapper)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
            //זה מה שצריך לעשות?
            _subjectService = subjectService;
        }

        public async Task<GameDTO> AddAsync(GameDTO game)
        {
            return _mapper.Map<GameDTO>(await _gameRepository.AddAsync(_mapper.Map<Game>(game)));
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

        private bool ChackRating(int gameRating, int playerRating)
        {
            int x = 500;//צריך להחליט מה הטווח שבו מאפשרים לשחקן חדש להצטרף למשחק
            if (gameRating + x > playerRating && gameRating - x < playerRating)
                return true;
            return false;
        }



        public async Task<GameDTO> GetActiveGameBySubjectAndRatingAsync(int subjectID, int rating)
        {
            return GetAllAsync().Result
                .Where(g => g.Subject.SubjectID == subjectID && g.IsActive && ChackRating(g.Rating, rating))
                .FirstOrDefault();

        }

        //מציאת משחק ועדכונו
        public async Task<GameDTO> FindGameAsync(int subjectID, int rating)
        {
            GameDTO game = GetActiveGameBySubjectAndRatingAsync(subjectID, rating).Result;
            if (game == null)
            {
                GameDTO newGame= new GameDTO();
                newGame.GameDate = DateTime.Now;
                newGame.GameLength = 0;
                newGame.Subject = _subjectService.GetByIdAsync(subjectID).Result;
                newGame.Rating = rating;
                newGame.IsActive=true;
                game= await AddAsync(newGame);
            }
            else
            {
                
                UpdateRating(game,rating);
                
            }
            return game;

        }

        public async Task UpdateRating(GameDTO game, int newPlayerRating)
        {
            try
            {
                //ממוצע של דירוגי השחקנים
                int NumOfPlayers = game.Players.Count;
                game.Rating = (game.Rating + newPlayerRating) / NumOfPlayers + 1;
                await UpdateAsync(game);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
