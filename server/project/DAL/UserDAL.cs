using Microsoft.EntityFrameworkCore;
using project.DAL.Intefaces;
using project.Models;
using System.Data;

namespace project.DAL
{
    public class UserDAL : IUserDAL
    {
        private readonly ProjectDbContext _context;
        public UserDAL(ProjectDbContext context)
        {
            _context = context;
        }
        
        public async Task Add(User user)
        {

            await _context.Users.AddAsync(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while saving user");
            }
        }

       public async Task<User?> GetByEmail(string email)
    {
        try
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        catch (Exception ex)
        {
            throw new DataException($"Database error while fetching user by email: {ex.Message}");
        }
    }
        public async Task<List<Gift>> GetUserGifts(int userId)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == userId);

            if (!userExists)
                throw new KeyNotFoundException("User not found");
            try
            {
                return await _context.Cards
                    .Where(c => c.UserId == userId)
                    .Include(c => c.Gift)
                    .Select(c => c.Gift)
                    .Distinct()
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while fetching user gifts");
            }
        }
    }
}