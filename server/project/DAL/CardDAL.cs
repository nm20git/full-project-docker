using project.DAL.Intefaces;
using project.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace project.DAL
{
    public class CardDAL : ICardDAL
    {
        private readonly ProjectDbContext _context;
        public CardDAL(ProjectDbContext context)
        {
            _context = context;
        }
        public async Task Add(Card card)
        {
            bool userExists = await _context.Users
                .AnyAsync(u => u.Id == card.UserId);

            bool giftExists = await _context.Gifts
                .AnyAsync(g => g.Id == card.GiftId);

            if (!userExists)
                throw new KeyNotFoundException("User does not exist");

            if (!giftExists)
                throw new KeyNotFoundException("Gift does not exist");

            await _context.Cards.AddAsync(card);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while saving card");
            }
        }

        public async Task DeleteAll()
        {
            _context.Cards.RemoveRange(_context.Cards);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while deleting cards");
            }
        }

        public async Task<int> GetTotal()
        {
            try
            {
                var total = await _context.Cards
                    .Include(c => c.Gift)
                    .SumAsync(c => c.Gift.Price);

                return total;
            }
            catch (Exception)
            {
                throw new DataException("Database error while calculating total");
            }
        }

    }
}
