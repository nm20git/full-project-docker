using Microsoft.EntityFrameworkCore;
using project.DAL.Intefaces;
using project.Models;
using System.Data;

namespace project.DAL
{
    public class RaffleDAL : IRaffleDAL
    {
        private readonly ProjectDbContext _context;

        public RaffleDAL(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<Gift> GetGiftWithBuyers(int giftId)
        {
            try
            {
                var gift = await _context.Gifts
                    .Include(g => g.Cards)
                        .ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(g => g.Id == giftId);

                if (gift == null)
                    throw new KeyNotFoundException("Gift not found");

                return gift;
            }
            catch (Exception)
            {
                throw new DataException("Database error while fetching gift with buyers");
            }
        }

        public async Task AddRaffle(Raffle raffle)
        {
            await _context.Raffles.AddAsync(raffle);
        }

        public async Task Save()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while saving raffle");
            }
        }
        public async Task DeleteAll()
        {
            _context.Raffles.RemoveRange(_context.Raffles);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while deleting raffles");
            }
        }

    }
}
