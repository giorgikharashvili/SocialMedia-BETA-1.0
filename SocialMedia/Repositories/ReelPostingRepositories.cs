using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models;

namespace SocialMedia.Repositories
{
    public class ReelPostingRepositories : IRepository<ReelPostings>
    {
        private readonly ApplicationDbContext _context;

        public ReelPostingRepositories(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReelPostings entity)
        {
            await _context.ReelPostings.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        

        public async Task DeleteAsync(int id)
        {
            var reelPosting = await _context.ReelPostings.FindAsync(id);

            if (reelPosting == null)
            {
                throw new KeyNotFoundException();
            }

            _context.ReelPostings.Remove(reelPosting);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReelPostings>> GetAllAsync()
        {
            return await _context.ReelPostings.ToListAsync();
        }

        public async Task<ReelPostings> GetByIdAsync(int id)
        {
            var reelPosting = await _context.ReelPostings.FindAsync(id);

            if(reelPosting == null)
            {
                throw new KeyNotFoundException();
            }

            return reelPosting;
        }

        public async Task UpdateAsync(ReelPostings entity)
        {
            _context.ReelPostings.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
