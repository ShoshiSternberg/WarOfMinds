using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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

        public async Task<GameDTO> UpdateAsync(int id,GameDTO game)
        {
            game.GameID=id;
            return _mapper.Map<GameDTO>(await _gameRepository.UpdateAsync(_mapper.Map<Game>(game)));
        }

        //public async GameDTO AddPlayerToGame(Player player)
        //{

        //}

        public bool CheckRating(int gameRating, int playerRating)
        {
            int x = 500;//צריך להחליט מה הטווח שבו מאפשרים לשחקן חדש להצטרף למשחק
            if (gameRating + x > playerRating && gameRating - x < playerRating)
                return true;
            return false;
        }



        public async Task<GameDTO> GetActiveGameBySubjectAndRatingAsync(int subjectID, int rating)
        {

            return GetAllAsync().Result
                .Where(g => g.Subject.SubjectID == subjectID && g.IsActive && CheckRating(g.Rating, rating))
                .FirstOrDefault();



        }

        //מציאת משחק ועדכונו
        public async Task<GameDTO> FindGameAsync(SubjectDTO subject, PlayerDTO player)
        {

            var game = await GetActiveGameBySubjectAndRatingAsync(subject.SubjectID, player.ELORating);

            if (game == null)
            {
                // Create a new game with the player's rating
                game = new GameDTO
                {
                    Subject = subject,
                    //GameManager = player,
                    GameDate = DateTime.Now,
                    GameLength = 30,
                    IsActive = true,
                    Rating = player.ELORating,
                    Players = new List<PlayerDTO> { player }
                };
                await AddAsync(game);
            }
            else
            {
                //apdate game to active
                game.IsActive = true;
                // Add player to existing game
                game.Players.Add(player);

                await UpdateAsync(game.GameID,game);

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
                await UpdateAsync(game.GameID,game);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
