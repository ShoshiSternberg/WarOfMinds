using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Repositories.Entities;
using WarOfMinds.Repositories.Interfaces;

namespace WarOfMinds.Repositories.Repositories
{
    public class GamePlayerRepository : IGamePlayerRepository
    {
        private readonly IContext _context;
        public GamePlayerRepository(IContext context)
        {
            _context = context;
        }
        public async Task<GamePlayer> AddAsync(GamePlayer GamePlayer)
        {
            var addedGamePlayer = await _context.GamePlayer.AddAsync(GamePlayer);
            await _context.SaveChangesAsync();
            return addedGamePlayer.Entity;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var GamePlayer = await GetByIdAsync(id);
            _context.GamePlayer.Remove(GamePlayer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<GamePlayer>> GetAllAsync()
        {
            return await _context.GamePlayer.ToListAsync();
        }

        public async Task<GamePlayer> GetByIdAsync(int id)
        {
            return await _context.GamePlayer.FindAsync(id);
        }

        public async Task<GamePlayer> UpdateAsync(GamePlayer GamePlayer)
        {
            var updatedGamePlayer = _context.GamePlayer.Update(GamePlayer);
            await _context.SaveChangesAsync();
            return updatedGamePlayer.Entity;
        }
    }
}
