using AutoMapper;
using project.BLL.Interfaces;
using project.DAL;
using project.DAL.Intefaces;
using project.DAL.Interfaces;
using project.Models;
using project.Models.DTO;
using System;
using Microsoft.Extensions.Logging;

namespace project.BLL
{
    public class GiftBLL : IGiftBLL
    {
        private readonly IGiftDAL _giftDAL;
        private readonly ISponsorDAL _sponsorDAL;
        private readonly ICardDAL _cardDAL;
        private readonly IRaffleDAL _raffleDAL;
        private readonly ILogger<GiftBLL> _logger;

        public GiftBLL(IGiftDAL giftDAL, ISponsorDAL sponsorDAL, ICardDAL cardDAL, IRaffleDAL raffleDAL, ILogger<GiftBLL> logger)
        {
            _giftDAL = giftDAL;
            _sponsorDAL = sponsorDAL;
            _cardDAL = cardDAL;
            _raffleDAL = raffleDAL;
            _logger = logger;
        }

        public async Task Add(Gift gift)
        {
            _logger.LogInformation("BLL: Adding gift. Name: {Name}, Amount: {Amount}, Price: {Price}",
                gift?.Name, gift?.Amount, gift?.Price);

            if (gift == null)
            {
                _logger.LogWarning("BLL: Gift is null");
                throw new ArgumentException("Gift is null");
            }

            if (gift.Amount <= 0)
            {
                _logger.LogWarning("BLL: Invalid amount for gift {Name}", gift.Name);
                throw new ArgumentException("Amount must be greater than zero");
            }

            if (gift.Price <= 0)
            {
                _logger.LogWarning("BLL: Invalid price for gift {Name}", gift.Name);
                throw new ArgumentException("Price must be greater than zero");
            }

            if (!Enum.IsDefined(typeof(Gift.GiftCategory), gift.Category))
            {
                _logger.LogWarning("BLL: Invalid category for gift {Name}", gift.Name);
                throw new ArgumentException("Invalid gift category");
            }

            await _giftDAL.Add(gift);

            _logger.LogInformation("BLL: Gift added successfully. Name: {Name}", gift.Name);
        }

        public async Task Delete(int Id)
        {
            _logger.LogInformation("BLL: Deleting gift. GiftId: {GiftId}", Id);

            await _giftDAL.Delete(Id);

            _logger.LogInformation("BLL: Gift deleted successfully. GiftId: {GiftId}", Id);
        }

        public async Task<List<Gift>> Get()
        {
            _logger.LogInformation("BLL: Getting all gifts");

            var gifts = await _giftDAL.Get();

            _logger.LogInformation("BLL: Retrieved {Count} gifts", gifts.Count);

            return gifts;
        }

        public async Task<Gift?> Get(int Id)
        {
            _logger.LogInformation("BLL: Getting gift by id. GiftId: {GiftId}", Id);

            var gift = await _giftDAL.Get(Id);

            return gift;
        }

        public async Task Update(Gift gift)
        {
            _logger.LogInformation("BLL: Updating gift. GiftId: {GiftId}", gift?.Id);

            if (gift == null)
            {
                _logger.LogWarning("BLL: Gift is null during update");
                throw new ArgumentException("Gift is null");
            }

            await _giftDAL.Update(gift);

            _logger.LogInformation("BLL: Gift updated successfully. GiftId: {GiftId}", gift.Id);
        }

        public async Task<List<User>> GetBuyers(int giftId)
        {
            _logger.LogInformation("BLL: Getting buyers for GiftId: {GiftId}", giftId);

            var buyers = await _giftDAL.GetBuyers(giftId);

            _logger.LogInformation("BLL: Retrieved {Count} buyers for GiftId: {GiftId}", buyers.Count, giftId);

            return buyers;
        }

        public async Task<List<object>> GetAllWinners()
        {
            _logger.LogInformation("BLL: Getting winners for Gifts");

            var winners = await _giftDAL.GetAllWinners();

            _logger.LogInformation("BLL: Retrieved {Count} winners for Gifts", winners.Count);

            return winners;
        }

        public async Task<List<Gift>> FilterGifts(GiftFilterDTO filter)
        {
            _logger.LogInformation("BLL: Filtering gifts");

            if (filter == null)
            {
                _logger.LogWarning("BLL: Filter data is null");
                throw new ArgumentException("Search data is null");
            }

            bool hasGiftName = !string.IsNullOrWhiteSpace(filter.GiftName);
            bool hasSponsorName = !string.IsNullOrWhiteSpace(filter.SponsorName);
            bool hasBuyersCount = filter.BuyersCount.HasValue;

            if (!hasGiftName && !hasSponsorName && !hasBuyersCount)
            {
                _logger.LogWarning("BLL: No filter criteria provided");
                throw new ArgumentException("At least one search field must be provided");
            }

            if (hasBuyersCount && filter.BuyersCount < 0)
            {
                _logger.LogWarning("BLL: Buyers count cannot be negative");
                throw new ArgumentException("Buyers count cannot be negative");
            }

            var gifts = await _giftDAL.FilterGifts(filter);

            _logger.LogInformation("BLL: Filter returned {Count} gifts", gifts.Count);

            return gifts;
        }

        public async Task ResetSystem()
        {
            _logger.LogWarning("BLL: Reset system initiated");

            var gifts = await _giftDAL.Get();

            if (gifts == null || !gifts.Any())
            {
                _logger.LogWarning("BLL: Reset failed - no gifts in system");
                throw new InvalidOperationException("System contains no gifts to reset");
            }

            await _raffleDAL.DeleteAll();
            await _cardDAL.DeleteAll();
            await _giftDAL.ResetAllDrawStatus();

            _logger.LogInformation("BLL: System reset completed successfully");
        }

        public async Task<List<Gift>> GetSortedByBuyers()
        {
            _logger.LogInformation("BLL: Getting gifts sorted by buyers");

            var gifts = await _giftDAL.GetSortedByBuyers();

            if (gifts == null || !gifts.Any())
            {
                _logger.LogWarning("BLL: No gifts found while sorting by buyers");
                throw new KeyNotFoundException("No gifts found");
            }

            _logger.LogInformation("BLL: Retrieved {Count} gifts sorted by buyers", gifts.Count);

            return gifts;
        }

        public async Task<List<Gift>> GetSortedByPrice()
        {
            _logger.LogInformation("BLL: Getting gifts sorted by price");

            var gifts = await _giftDAL.GetSortedByPrice();

            if (gifts == null || !gifts.Any())
            {
                _logger.LogWarning("BLL: No gifts found while sorting by price");
                throw new KeyNotFoundException("No gifts found");
            }

            _logger.LogInformation("BLL: Retrieved {Count} gifts sorted by price", gifts.Count);

            return gifts;
        }

        public async Task<List<User>> GetWinners(int giftId)
        {
            _logger.LogInformation("BLL: Getting winners for Gifts");

            var winners = await _giftDAL.GetWinners(giftId);

            _logger.LogInformation("BLL: Retrieved {Count} winners for Gifts", winners.Count);

            return winners;
        }
    }
}
