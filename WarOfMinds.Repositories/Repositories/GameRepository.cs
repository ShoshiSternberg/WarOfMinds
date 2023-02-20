using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return await _context.Games.ToListAsync();
        }

        public async Task<Game> GetByIdAsync(int id)
        {
            return await _context.Games.FindAsync(id);
        }

        public async Task<Game> UpdateAsync(Game game)
        {            
            var updatedGame = _context.Games.Update(game);
            await _context.SaveChangesAsync();
            return updatedGame.Entity;
        }


        public async bool AddPlayerToGame(Player player)
        {
             await _playerRepository.GetByIdAsync(player.PlayerID);
            Player p = await GetByIdAsync(player.PlayerID).Result;
            if ( p!=null)
            {

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
