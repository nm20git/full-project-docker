using project.BLL.Interface;
using project.DAL;
using project.DAL.Intefaces;
using project.DAL.Interfaces;
using project.Models;
using project.Models.DTO;
using Microsoft.Extensions.Logging;

namespace project.BLL
{
    public class SponsorBLL : ISponsorBLL
    {
        private readonly ISponsorDAL _sponsorDAL;
        private readonly ILogger<SponsorBLL> _logger;

        public SponsorBLL(ISponsorDAL sponsorDAL, ILogger<SponsorBLL> logger)
        {
            _sponsorDAL = sponsorDAL;
            _logger = logger;
        }

        public async Task Add(Sponsor sponsor)
        {
            _logger.LogInformation("BLL: Adding sponsor. Name: {Name}", sponsor?.FirstName);

            if (sponsor == null)
            {
                _logger.LogWarning("BLL: Sponsor is null during add");
                throw new ArgumentException("Sponsor is null");
            }

            await _sponsorDAL.Add(sponsor);

            _logger.LogInformation("BLL: Sponsor added successfully. Name: {Name}", sponsor.FirstName);
        }

        public async Task Delete(int Id)
        {
            _logger.LogInformation("BLL: Deleting sponsor. SponsorId: {SponsorId}", Id);

            await _sponsorDAL.Delete(Id);

            _logger.LogInformation("BLL: Sponsor deleted successfully. SponsorId: {SponsorId}", Id);
        }

        public async Task<List<Sponsor>> Get()
        {
            _logger.LogInformation("BLL: Getting all sponsors");

            var sponsors = await _sponsorDAL.Get();

            _logger.LogInformation("BLL: Retrieved {Count} sponsors", sponsors.Count);

            return sponsors;
        }

        public async Task<Sponsor?> Get(int Id)
        {
            _logger.LogInformation("BLL: Getting sponsor by id. SponsorId: {SponsorId}", Id);

            var sponsor = await _sponsorDAL.Get(Id);

            return sponsor;
        }

        public async Task Update(Sponsor sponsor)
        {
            _logger.LogInformation("BLL: Updating sponsor. SponsorId: {SponsorId}", sponsor?.Id);

            if (sponsor == null)
            {
                _logger.LogWarning("BLL: Sponsor is null during update");
                throw new ArgumentException("Sponsor is null");
            }

            await _sponsorDAL.Update(sponsor);

            _logger.LogInformation("BLL: Sponsor updated successfully. SponsorId: {SponsorId}", sponsor.Id);
        }

        public async Task<List<Gift>> GetGifts(int sponsorId)
        {
            _logger.LogInformation("BLL: Getting gifts for SponsorId: {SponsorId}", sponsorId);

            var gifts = await _sponsorDAL.GetGifts(sponsorId);

            _logger.LogInformation("BLL: Retrieved {Count} gifts for SponsorId: {SponsorId}", gifts.Count, sponsorId);

            return gifts;
        }

        public async Task<List<Sponsor>> FilterSponsors(SponsorFilterDTO filter)
        {
            _logger.LogInformation("BLL: Filtering sponsors");

            if (filter == null)
            {
                _logger.LogWarning("BLL: Filter is null");
                throw new ArgumentException("Filter is null");
            }

            if (string.IsNullOrWhiteSpace(filter.Name) &&
                string.IsNullOrWhiteSpace(filter.Email) &&
                string.IsNullOrWhiteSpace(filter.GiftName))
            {
                _logger.LogWarning("BLL: No filter fields provided");
                throw new ArgumentException("At least one filter field must be provided");
            }

            var sponsors = await _sponsorDAL.FilterSponsors(filter);

            _logger.LogInformation("BLL: Filter returned {Count} sponsors", sponsors.Count);

            return sponsors;
        }
    }
}
