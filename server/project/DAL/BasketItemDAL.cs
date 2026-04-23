using Microsoft.EntityFrameworkCore;
using project.DAL.Intefaces;
using project.Models;
using System.Data;

namespace project.DAL
{
    public class BasketItemDAL : IBasketItemDAL
    {
        private readonly ProjectDbContext _context;
        public BasketItemDAL(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<List<BasketItem>> Get(int userId)
        {
            try
            {
                return await _context.BasketItems
                    .Include(b => b.Gift)
                    .Where(b => b.UserId == userId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while retrieving basket items");
            }
        }

        public async Task AddItem(int userId, int giftId)
        {
            bool userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                throw new KeyNotFoundException("User not found");

            bool giftExists = await _context.Gifts.AnyAsync(g => g.Id == giftId);
            if (!giftExists)
                throw new KeyNotFoundException("Gift not found");

            var checkItem = await _context.BasketItems.FirstOrDefaultAsync(i => i.UserId == userId && i.GiftId == giftId);
            if (checkItem == null)
            {
                var newItem = new BasketItem
                {
                    UserId=userId,
                    GiftId=giftId,
                    Quantity=1
                };
                await _context.BasketItems.AddAsync(newItem);
            }
            else
            {
                checkItem.Quantity = checkItem.Quantity+1;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while updating basket");
            }
        }

        public async Task DeleteItem(int userId, int giftId)
        {
            var item = await _context.BasketItems.FirstOrDefaultAsync(i => i.UserId==userId && i.GiftId==giftId);
            if (item == null)
                throw new KeyNotFoundException("Basket item not found");

            _context.BasketItems.Remove(item);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while deleting basket item");
            }
        }
        public async Task ClearBasket(int userId)
        {
            var items = await _context.BasketItems
                .Where(i => i.UserId == userId)
                .ToListAsync();
           
            if(items != null)
                _context.BasketItems.RemoveRange(items);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while clearing basket");
            }
        }

        public async Task UpdateQuantity(int userId, int giftId, string action)
        {
            var item = await _context.BasketItems.FirstOrDefaultAsync(i => i.UserId == userId && i.GiftId == giftId);
            if (item == null)
                throw new KeyNotFoundException("Item not found");

            if (action == "+")
                item.Quantity += 1 ;
            else
            {
                if (item.Quantity == 1)
                    _context.BasketItems.Remove(item);
                else
                    item.Quantity = item.Quantity - 1;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while updating basket quantity");
            }
        }
    }
}
