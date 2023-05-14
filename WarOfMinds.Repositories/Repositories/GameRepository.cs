using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WarOfMinds.Repositories.Entities;
using WarOfMinds.Repositories.Interfaces;
using System.Data.SqlClient;


namespace WarOfMinds.Repositories.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly IContext _context;
        private readonly IPlayerRepository _playerRepository;
        private readonly IGamePlayerRepository _gamePlayerRepository;
        public GameRepository(IContext context, IPlayerRepository playerRepository, IGamePlayerRepository gamePlayerRepository)
        {
            _context = context;
            _playerRepository = playerRepository;
            _gamePlayerRepository = gamePlayerRepository;
        }


        public async Task DeleteByIdAsync(int id)
        {
            var game = await GetByIdAsync(id);
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Game>> GetAllAsync()
        {
            return await _context.Games.Include(g => g.Subject).ToListAsync();
        }

        public async Task<Game> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Games.Include(g => g.Subject).FirstOrDefaultAsync(g => g.GameID == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }



        public async Task<Game> AddGameAsync(Game game)
        {
            if (game.Subject != null)
            {
                // attach the subject to the context
                _context.Subjects.Attach(game.Subject);
            }
            var players = game.Players;
            game.Players.Clear();
            // add the game to the context
            Game addedGame = (await _context.Games.AddAsync(game)).Entity;
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            foreach (GamePlayer player in players)
            {
                player.PGame = addedGame;
                player.GameId = addedGame.GameID;
                await _gamePlayerRepository.AddAsync(player);
            }

            return addedGame;
        }
        public async Task<Game> UpdateGameAsync(Game game)
        {
            try
            {
                // Get the game to update
                Game existingGame = await _context.Games.Include(g => g.Subject).Include(g => g.Players).ThenInclude(g => g.GPlayer).FirstOrDefaultAsync(g => g.GameID == game.GameID);

                if (existingGame != null)
                {

                    foreach (GamePlayer player in game.Players.ToList())
                    {
                        await _gamePlayerRepository.UpdateAsync(player);
                    }

                    existingGame.Players.Clear();
                    game.Players.Clear();

                    _context.Games.Update(game);

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.Clear();
                    Console.WriteLine("GameRepository update:\n" + _context.ChangeTracker.DebugView.LongView);
                    // Return the updated game
                    return game;
                }
                else
                {
                    return await AddAsync(game);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }



        public async Task<Game> GetWholeByIdAsync(int id)
        {
            Game ans = await _context.Games.Include(g => g.Subject).Include(g => g.Players).ThenInclude(g => g.GPlayer).FirstOrDefaultAsync(g => g.GameID == id);
            _context.ChangeTracker.Clear();
            Console.WriteLine("GameRepository GetWholeByIdAsync:\n" + _context.ChangeTracker.DebugView.LongView);
            return ans;
        }
    }
}
