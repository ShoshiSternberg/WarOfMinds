using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WarOfMinds.Common.DTO;
using WarOfMinds.Repositories.Entities;
using WarOfMinds.Repositories.Interfaces;
using WarOfMinds.Repositories.Repositories;
using WarOfMinds.Services.Interfaces;
using static System.Formats.Asn1.AsnWriter;

namespace WarOfMinds.Services.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly ISubjectService _subjectService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;
        private static readonly object _lock = new object();
        public GameService(IGameRepository gameRepository, ISubjectService subjectService, IMapper mapper, IServiceScopeFactory serviceScopeFactory)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
            _scopeFactory = serviceScopeFactory;
            _subjectService = subjectService;
            
        }

        public async Task<GameDTO> AddAsync(GameDTO game)
        {
            return _mapper.Map<GameDTO>(await _gameRepository.AddAsync(_mapper.Map<Game>(game)));
        }

        public async Task<GameDTO> AddGameAsync(GameDTO game)
        {
            if (game.Subject == null)
            {
                game.Subject = await _subjectService.GetByIdAsync(game.SubjectID);
            }
                      
            return _mapper.Map<GameDTO>(await _gameRepository.AddGameAsync(_mapper.Map<Game>(game)));
        }

        public async Task DeleteByIdAsync(int id)
        {
            await _gameRepository.DeleteByIdAsync(id);
        }

        public async Task<List<GameDTO>> GetAllAsync()
        {
            var ans = _mapper.Map<List<GameDTO>>(await _gameRepository.GetAllAsync());
            return ans;
        }

        public async Task<GameDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<GameDTO>(await _gameRepository.GetByIdAsync(id));

        }

        public async Task<GameDTO> GetByIdInNewScopeAsync(int id)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var gameRepository = scope.ServiceProvider.GetRequiredService<IGameRepository>();
                    return _mapper.Map<GameDTO>(await gameRepository.GetWholeByIdAsync(id));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }
        public async Task<GameDTO> GetWholeByIdAsync(int id)
        {
            return _mapper.Map<GameDTO>(await _gameRepository.GetWholeByIdAsync(id));

        }

        public async Task<GameDTO> UpdateAsync(int id, GameDTO game)
        {
            game.GameID = id;
            return _mapper.Map<GameDTO>(await _gameRepository.UpdateAsync(_mapper.Map<Game>(game)));
        }

        public async Task<GameDTO> UpdateGameAsync(int id, GameDTO game)
        {
            game.GameID = id;
            return _mapper.Map<GameDTO>(await _gameRepository.UpdateGameAsync(_mapper.Map<Game>(game)));
        }


        public bool CheckRating(int gameRating, int playerRating)
        {
            int x = 500;//צריך להחליט מה הטווח שבו מאפשרים לשחקן חדש להצטרף למשחק
            if (gameRating + x > playerRating && gameRating - x < playerRating)
                return true;
            return false;
        }


        public async Task<GameDTO> GetActiveGameBySubjectAndRatingAsync(int subjectID, int rating)
        {
            var games =await GetAllAsync();
            return games
            .Where(g => g.SubjectID == subjectID && g.IsActive && CheckRating(g.Rating, rating))
            .FirstOrDefault();
        }

        //מציאת משחק ועדכונו
        public async Task<GameDTO> FindGameAsync(SubjectDTO subject, PlayerDTO player)
        {
            GameDTO game;
            lock (_lock)
            {
                game = GetActiveGameBySubjectAndRatingAsync(subject.SubjectID, player.ELORating).Result;

                if (game == null)
                {
                    // Create a new game with the player's rating
                    game = new GameDTO
                    {
                        SubjectID = subject.SubjectID,
                        Subject = subject,                     
                        GameDate = DateTime.Now,
                        GameLength = 30,
                        IsActive = true,
                        Rating = player.ELORating,
                        Players = new List<PlayerDTO>()
                    };
                    game = AddGameAsync(game).Result;
                }
            }
            
            // Add player to existing game
            game = await GetWholeByIdAsync(game.GameID);
            if (!game.Players.Any(p => p.PlayerID == player.PlayerID))
                game.Players.Add(player);

            game = await UpdateGameAsync(game.GameID, game);
            return await GetWholeByIdAsync(game.GameID);
        }

        public async Task UpdateRating(GameDTO game, int newPlayerRating)
        {
            try
            {
                //ממוצע של דירוגי השחקנים
                int NumOfPlayers = game.Players.Count;
                game.Rating = (game.Rating + newPlayerRating) / NumOfPlayers + 1;
                await UpdateAsync(game.GameID, game);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        //כמובן צריך להחליט מהי הרמה המתאימה לאיזה טווח של דירוג
        public string Difficulty(int rating)
        {
            if (rating < 1000)
                return "easy";
            if (rating > 2000)
                return "medium";
            return "hard";

        }
    }
}
