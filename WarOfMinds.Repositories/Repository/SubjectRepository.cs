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
    public class SubjectRepository : ISubjectRepository
    {
        private readonly IContext _context;
        public SubjectRepository(IContext context)
        {
            _context = context;
        }

        public async Task<Subject> AddAsync(Subject subject)
        {
            var addedSubject = await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            return addedSubject.Entity;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var subject = await GetByIdAsync(id);
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Subject>> GetAllAsync()
        {
            return await _context.Subjects.ToListAsync();
        }

        public async Task<Subject> GetByIdAsync(int id)
        {
            return await _context.Subjects.FindAsync(id);
        }

        public async Task<Subject> UpdateAsync(Subject subject)
        {
            var updatedSubject = _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
            return updatedSubject.Entity;
        }
    }
}
