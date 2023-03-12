using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WarOfMinds.Repositories.Entities;
using WarOfMinds.Repositories.Interfaces;

namespace WarOfMinds.Repositories.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly IContext _context;
        private readonly IPlayerRepository _playerRepository;
        public GameRepository(IContext context)
        {
            _context = context;
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
            return await _context.Games.Include(g=>g.Subject).ToListAsync();
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
            // attach each player to the context
            foreach (var player in game.Players)
            {
                if (player != null)
                {
                    _context.Players.Attach(player);
                }
            }
            
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
                Game gameToUpdate = _context.Games.Include(g => g.Players).FirstOrDefault(g => g.GameID == game.GameID);

                if (gameToUpdate != null)
                {
                    // Update the game properties
                    gameToUpdate.GameDate = game.GameDate;
                    gameToUpdate.GameLength = game.GameLength;
                    gameToUpdate.Rating = game.Rating;
                    gameToUpdate.IsActive = game.IsActive;

                    // Get the subject
                    Subject subject = _context.Subjects.FirstOrDefault(s => s.SubjectID == game.SubjectID);

                    if (subject != null)
                    {
                        // Attach the subject to the context
                        _context.Subjects.Attach(subject);

                        // Update the game's subject
                        gameToUpdate.Subject = subject;
                    }

                    // Add new players to the game
                    foreach (Player p1 in game.Players)
                    {
                        // Check if the player exists in the database
                        Player player = _context.Players.FirstOrDefault(p => p.PlayerID == p1.PlayerID);

                        if (player != null)
                        {
                            // Player exists in the database, attach it to the context
                            _context.Players.Attach(player);
                        }
                        else
                        {
                            // Player doesn't exist in the database, add player to the context
                            _context.Players.Add(p1);
                        }

                        // Add the player to the game if it's not already added
                        if (!gameToUpdate.Players.Contains(player))
                        {
                            gameToUpdate.Players.Add(player);
                        }
                    }

                    // Update the game in the context
                    var updatedGame = _context.Games.Update(gameToUpdate);

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Return the updated game
                    return updatedGame.Entity;
                }
                else
                {
                    return await AddAsync(gameToUpdate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }


        public async Task<Game> GetCurrentGame(Subject subject)
        {

            Game game= _context.Games.
                Where(g => g.Subject.SubjectID == subject.SubjectID && g.IsActive)
                .FirstOrDefault();
            
            return game;

            
        }
        
    }
}
