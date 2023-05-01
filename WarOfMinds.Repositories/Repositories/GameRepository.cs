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
        public GameRepository(IContext context,IPlayerRepository playerRepository)
        {
            _context = context;
            _playerRepository = playerRepository;
        }

        public async Task<Game> AddAsync(Game game)
        {
            var addedGame = await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
            return addedGame.Entity;
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
            return await _context.Games.Include(g => g.Subject).FirstOrDefaultAsync(g => g.GameID == id);
        }

        public async Task<Game> UpdateAsync(Game game)
        {
            var updatedGame = _context.Games.Update(game);
            await _context.SaveChangesAsync();
            return updatedGame.Entity;
        }

        public async Task<Game> AddGameAsync(Game game)
        {
            if (game.Subject != null)
            {
                // attach the subject to the context
                _context.Subjects.Attach(game.Subject);
            }
            //attach each player to the context
            //foreach (var player in game.Players)
            //{
            //    if (player != null)
            //    {
            //        _context.Players.Attach(player);
            //    }
            //}

            // add the game to the context
            var addedGame = await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
            return addedGame.Entity;
        }
        public async Task<Game> UpdateGameAsync(Game game)
        {
            try
            {
                
                // Get the game to update
                Game existingGame = _context.Games.Include(g => g.Players).FirstOrDefault(g => g.GameID == game.GameID);

                if (existingGame != null)
                {
                    

                    // Get the subject
                    Subject subject = _context.Subjects.FirstOrDefault(s => s.SubjectID == game.SubjectID);

                    if (subject != null)
                    {
                        // Attach the subject to the context
                        _context.Subjects.Attach(subject);

                        // Update the game's subject
                        game.Subject = subject;
                    }

                    foreach (GamePlayer player in existingGame.Players.ToList())
                    {
                        if (!game.Players.Any(gp => gp.PlayerId == player.PlayerId))
                        {
                            existingGame.Players.Remove(player);
                        }
                    }

                    // Add players that are in the updated game but not in the existing game
                    foreach (GamePlayer player in game.Players)
                    {
                        if (!existingGame.Players.Any(gp => gp.PlayerId == player.PlayerId))
                        {
                            var existingPlayer = await _context.Players.FindAsync(player.PlayerId);   
                            player.GPlayer=await _playerRepository.GetByIdAsync(player.PlayerId);
                            player.PGame=await GetByIdAsync(player.GameId);
                            existingGame.Players.Add(player);
                        }
                    }

                    // Add new players to the game
                    //foreach (GamePlayer p1 in game.Players)
                    //{
                    //    // Check if the player exists in the database
                    //    Player player = _context.Players.FirstOrDefault(p => p.PlayerID == p1.PlayerID);

                    //    if (player != null)
                    //    {
                    //        // Player exists in the database, attach it to the context
                    //        _context.Players.Attach(player);
                    //    }
                    //    else
                    //    {
                    //        // Player doesn't exist in the database, add player to the context
                    //        _context.Players.Add(p1);
                    //    }

                    //    // Add the player to the game if it's not already added
                    //    if (gameToUpdate.Players.FirstOrDefault(p => p.PlayerID == player.PlayerID) == null)
                    //    {
                    //        gameToUpdate.Players.Add(player);
                    //    }
                    //}

                    // Update the game in the context
                    var updatedGame = _context.Games.Update(game);
                    // Save changes to the database
                    var t = await _context.SaveChangesAsync();

                    // Return the updated game
                    return updatedGame.Entity;
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
            return await _context.Games.Include(g => g.Subject).Include(g => g.Players).FirstOrDefaultAsync(g => g.GameID == id);
        }
    }
}
