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
        private readonly IPlayerRepository _playerRepository;

        public GamePlayerRepository(IContext context, IPlayerRepository playerRepository)
        {
            _context = context;
            _playerRepository = playerRepository;

        }
        public async Task<GamePlayer> AddAsync(GamePlayer GamePlayer)
        {
            try
            {
                Player p = _context.Players.FirstOrDefault(s => s.PlayerID == GamePlayer.PlayerId);

                if (p != null)
                {
                    // Attach the subject to the context
                    _context.Players.Attach(p);

                    // Update the game's subject
                    GamePlayer.GPlayer = p;
                }
                Game g = _context.Games.FirstOrDefault(s => s.GameID == GamePlayer.GameId);

                if (g != null)
                {
                    // Attach the subject to the context
                    _context.Games.Attach(g);

                    // Update the game's subject
                    GamePlayer.PGame = g;
                }
                var addedGamePlayer = await _context.GamePlayer.AddAsync(GamePlayer);
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
                Console.WriteLine("gamePlayerRepository add: \n"+_context.ChangeTracker.DebugView.LongView);
                //_context.Entry(g).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Detached;
                return addedGamePlayer.Entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
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

        public async Task<GamePlayer> UpdateAsync(GamePlayer gamePlayer)
        {
            try
            {
                GamePlayer gamep = _context.GamePlayer.FirstOrDefault(p => p.GameId == gamePlayer.GameId && p.PlayerId == gamePlayer.PlayerId);
                if (gamep == null)
                {
                    return await AddAsync(gamePlayer);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }
    }
}
