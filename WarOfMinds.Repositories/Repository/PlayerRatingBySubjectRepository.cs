using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Repositories.Entities;
using WarOfMinds.Repositories.Interfaces;

namespace WarOfMinds.Repositories.Repository
{
    public class PlayerRatingBySubjectRepository : IPlayerRatingBySubjectRepository
    {
        private readonly IContext _context;

        public PlayerRatingBySubjectRepository(IContext context)
        {
            _context = context;
        }

        public async Task<PlayerRatingBySubject> AddAsync(PlayerRatingBySubject playerRatingBySubject)
        {
            var addedPlayerRatingBySubject = await _context.PlayerRatingsBySubject.AddAsync(playerRatingBySubject) ;
            await _context.SaveChangesAsync();
            return addedPlayerRatingBySubject.Entity;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var PlayerRatingBySubject = await GetByIdAsync(id);
            _context.PlayerRatingsBySubject.Remove(PlayerRatingBySubject);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PlayerRatingBySubject>> GetAllAsync()
        {
            return await _context.PlayerRatingsBySubject.ToListAsync();
        }

        public async Task<PlayerRatingBySubject> GetByIdAsync(int id)
        {
            return await _context.PlayerRatingsBySubject.FindAsync(id);
        }

        public async Task<PlayerRatingBySubject> UpdateAsync(PlayerRatingBySubject playerRatingBySubject)
        {
            var updatedRating = _context.PlayerRatingsBySubject.Update(playerRatingBySubject);
            await _context.SaveChangesAsync();
            return updatedRating.Entity;
        }
    }
}
