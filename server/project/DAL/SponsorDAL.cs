using Microsoft.EntityFrameworkCore;
using project.DAL.Interfaces;
using project.Models;
using project.Models.DTO;
using System.Data;

namespace project.DAL
{
    public class SponsorDAL : ISponsorDAL
    {
        private readonly ProjectDbContext _context;
        public SponsorDAL(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sponsor>> Get()
        {
            try
            {
                return await _context.Sponsors
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while fetching sponsors");
            }
        }

        public async Task<Sponsor?> Get(int id)
        {
            try
            {
                var sponsor = await _context.Sponsors
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (sponsor == null)
                    throw new KeyNotFoundException("Sponsor not found");

                return sponsor;
            }
            catch (Exception)
            {
                throw new DataException("Database error while fetching sponsor");
            }
        }

        public async Task Add(Sponsor sponsor)
        {
            await _context.Sponsors.AddAsync(sponsor);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while saving sponsor");
            }
        }

        public async Task Delete(int Id)
        {
            var sponsor = await _context.Sponsors.FindAsync(Id);
            if (sponsor == null)
                throw new KeyNotFoundException("Sponsor not found");

            bool cardsExist = await _context.Cards
                .AnyAsync(c => c.Gift.SponsorId == Id);

            //בדיקה האם נרכשו כבר כרטיסים למתנה שתרם - א"א למחוק  
            if (cardsExist) throw new InvalidOperationException( "This sponsor cannot be deleted because at least one of their gifts already has tickets assigned.");

            _context.Sponsors.Remove(sponsor);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while deleting sponsor");
            }
        }
        public async Task Update(Sponsor sponsor)
        {
            var existingSponsor = await _context.Sponsors.FindAsync(sponsor.Id);

            if (existingSponsor == null)
                throw new KeyNotFoundException("Sponsor not found");
            existingSponsor.FirstName = sponsor.FirstName;
            existingSponsor.LastName = sponsor.LastName;
            existingSponsor.Phone = sponsor.Phone;
            existingSponsor.Email = sponsor.Email;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while updating sponsor");
            }
        }

        public async Task<List<Gift>> GetGifts(int sponsorId)
        {
            var sponsorExists = await _context.Sponsors
                    .AnyAsync(s => s.Id == sponsorId);

            if (!sponsorExists)
                throw new KeyNotFoundException("Sponsor not found");

            try
            {
                return await _context.Gifts
                    .Where(g => g.SponsorId == sponsorId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw new DataException("Database error while fetching sponsor gifts");
            }
        }
        public async Task<List<Sponsor>> FilterSponsors(SponsorFilterDTO filter)
        {
            try
            {
                var query = _context.Sponsors
                    .Include(s => s.Gifts)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                    query = query.Where(s =>
                        s.FirstName.Contains(filter.Name) ||
                        s.LastName.Contains(filter.Name));
                }

                if (!string.IsNullOrWhiteSpace(filter.Email))
                {
                    query = query.Where(s => s.Email.Contains(filter.Email));
                }

                if (!string.IsNullOrWhiteSpace(filter.GiftName))
                {
                    query = query.Where(s =>
                        s.Gifts.Any(g => g.Name.Contains(filter.GiftName)));
                }

                var result = await query.AsNoTracking().ToListAsync();

                if (!result.Any())
                    throw new KeyNotFoundException("No sponsors found matching the filter");

                return result;
            }
            catch (Exception)
            {
                throw new DataException("Database error while filtering sponsors");
            }
        }
    }
}