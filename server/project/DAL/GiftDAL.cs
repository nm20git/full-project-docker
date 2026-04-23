using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using project.DAL.Intefaces;
using project.Models;
using project.Models.DTO;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace project.DAL
{
    public class GiftDAL : IGiftDAL
    {
        private readonly ProjectDbContext _context;
        public GiftDAL(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<List<Gift>> Get()
        {
            try
            {
                return await _context.Gifts
                     .Include(g => g.Cards)
                     .AsNoTracking()
                     .ToListAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while fetching gifts");
            }
        }

        public async Task<Gift?> Get(int id)
        {
            try
            {
                var gift = await _context.Gifts
                    .Include(g => g.Cards)
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (gift == null)
                    throw new KeyNotFoundException("Gift not found");

                return gift;
            }
            catch (Exception)
            {
                throw new DataException("Database error while fetching gift");
            }
        }
      
        public async Task Add(Gift gift)
        {
            bool sponsorExists = await _context.Sponsors
              .AnyAsync(s => s.Id == gift.SponsorId);

            if (!sponsorExists)
                throw new KeyNotFoundException("Sponsor does not exist");

            await _context.Gifts.AddAsync(gift);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while saving gift");
            }
        }
        
        public async Task Delete(int Id)
        {

            var gift = await _context.Gifts.FindAsync(Id);
            if (gift == null)
                throw new KeyNotFoundException("Gift not found");

            //בדיקה שאין למתנה כרטיסים
            bool cards = await _context.Cards
                .AnyAsync(c => c.GiftId == Id);
            if (cards)
            {
                throw new InvalidOperationException("This gift already has tickets assigned and cannot be deleted");
            }
            _context.Gifts.Remove(gift);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while deleting gift");
            }
        }

        public async Task Update(Gift gift)
        {    
            //בדיקה שקיימת כזו מתנה
            var existingGift = await _context.Gifts.FindAsync(gift.Id);

            if (existingGift == null)
                throw new KeyNotFoundException("Gift not found");

            if (existingGift.IsDrawn)
                throw new InvalidOperationException("Cannot update a drawn gift");

            bool sponsorExists = await _context.Sponsors
             .AnyAsync(s => s.Id == gift.SponsorId);

            if (!sponsorExists)
                throw new KeyNotFoundException("Sponsor does not exist");

            existingGift.Name = gift.Name;
            existingGift.Amount = gift.Amount;
            existingGift.SponsorId = gift.SponsorId;
            existingGift.ImageUrl = gift.ImageUrl;
            existingGift.Price = gift.Price;
            existingGift.Category = gift.Category;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while updating gift");
            }

        }

        public async Task<List<User>> GetBuyers(int giftId)
        {
            // בדיקה שהמתנה קיימת
            var giftExists = await _context.Gifts
                .AnyAsync(g => g.Id == giftId);

            if (!giftExists)
                throw new KeyNotFoundException("Gift not found");

            try
            {
                return await _context.Cards
                    .Where(c => c.GiftId == giftId)
                    .Include(c => c.User)
                    .Select(c => c.User)
                    .Distinct()
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while fetching buyers");
            }
        }

        public async Task<List<object>> GetAllWinners()
        {
            try
            {
                var winners = await _context.Raffles
                    .Include(r => r.Winner)
                    .Include(r => r.Gift)
                    .Select(r => new
                    {
                        UserId = r.Winner.Id,
                        FirstName = r.Winner.FirstName,
                        LastName = r.Winner.LastName,
                        Email = r.Winner.Email,

                        GiftId = r.Gift.Id,
                        GiftName = r.Gift.Name
                    })
                    .ToListAsync();

                return winners.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                throw new DataException(ex.ToString());
            }
        }



        public async Task<List<Gift>> FilterGifts(GiftFilterDTO filter)
        {
             try
             {
                var query = _context.Gifts
                    .Include(g => g.sponsor)
                    .Include(g => g.Cards)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.GiftName))
                    query = query.Where(g => g.Name.Contains(filter.GiftName));

                if (!string.IsNullOrWhiteSpace(filter.SponsorName))
                    query = query.Where(g =>
                        g.sponsor.FirstName.Contains(filter.SponsorName) ||
                        g.sponsor.LastName.Contains(filter.SponsorName));

                if (filter.BuyersCount != null)
                    query = query.Where(g => g.Cards.Count == filter.BuyersCount);

                var result = await query.AsNoTracking().ToListAsync();

                if (!result.Any())
                    throw new KeyNotFoundException("No gifts found matching search criteria");

                return result;
             }
             catch (Exception)
             {
                throw new DataException("Database error while filtering gifts");
             }
        }
        public async Task ResetAllDrawStatus()
        {
            var gifts = await _context.Gifts.ToListAsync();
            foreach (var gift in gifts)
            {
                gift.IsDrawn = false;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while resetting system");
            }
        }
        public async Task<List<Gift>> GetSortedByBuyers()
        {
            try
            {
                return await _context.Gifts
                    .Include(g => g.Cards)
                    .OrderByDescending(g => g.Cards.Count)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while sorting gifts by buyers");
            }
        }
        public async Task<List<Gift>> GetSortedByPrice()
        {
            try
            {
                return await _context.Gifts
                    .Include(g => g.Cards)
                    .OrderByDescending(g => g.Price)
                    .AsNoTracking()
                    .ToListAsync();

            }
            catch (Exception)
            {
                throw new DataException("Database error while sorting gifts by price");
            }
        }

        public async Task<List<User>> GetWinners(int giftId)
        {
            try 
            { 
                var gift = await _context.Gifts 
                    .Include(g => g.Winners) 
                    .ThenInclude(r => r.Winner) 
                    .FirstOrDefaultAsync(g => g.Id == giftId); 
                if (gift == null) 
                    throw new KeyNotFoundException("Gift not found"); 
                if (!gift.IsDrawn) 
                    throw new InvalidOperationException("Gift has not been drawn yet"); 
                return gift.Winners .Select(r => r.Winner) 
                    .Distinct() 
                    .ToList(); 
            } 
            catch (Exception ex) 
            { 
                throw new DataException(ex.ToString()); 
                // throw new DataException("Database error while fetching winners");
            }
        }
    }
}
