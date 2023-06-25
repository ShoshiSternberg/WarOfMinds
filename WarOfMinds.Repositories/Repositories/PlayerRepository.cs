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
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IContext _context;
        public PlayerRepository(IContext context)
        {
            _context = context;
        }

        public async Task<Player> AddAsync(Player Player)
        {
            if (await _context.Players.FirstOrDefaultAsync(p => p.PlayerEmail == Player.PlayerEmail && p.PlayerPassword == Player.PlayerPassword) != null)
                return null;
            var addedPlayer = await _context.Players.AddAsync(Player);
            await _context.SaveChangesAsync();
            return addedPlayer.Entity;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var Player = await GetByIdAsync(id);
            _context.Players.Remove(Player);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Player>> GetAllAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<Player> GetByIdAsync(int id)
        {
            return await _context.Players.FindAsync(id);
        }

        public async Task<Player> UpdateAsync(Player Player)
        {
            var updatedPlayer = _context.Players.Update(Player);
            await _context.SaveChangesAsync();
            return updatedPlayer.Entity;
        }
        
        public async Task<Player> GetByEmailAndPassword(string email,string password)
        {
            try
            {
                var InsertedPlayer =await _context.Players.FirstOrDefaultAsync(p => p.PlayerEmail == email && p.PlayerPassword == password);
                await _context.SaveChangesAsync();
                return InsertedPlayer;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<Player> GetWholeByIdAsync(int id)
        {
            try
            {
                Player ans = await _context.Players.Include(g => g.Games).ThenInclude(g => g.PGame).FirstOrDefaultAsync(g => g.PlayerID == id);
                _context.ChangeTracker.Clear();
                return ans;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
