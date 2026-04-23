using project.BLL.Interfaces;
using project.DAL.Intefaces;
using project.Models.DTO;
using project.Models;
using Microsoft.Extensions.Logging;

namespace project.BLL
{
    public class RaffleBLL : IRaffleBLL
    {
        private readonly IRaffleDAL _raffleDAL;
        private readonly ILogger<RaffleBLL> _logger;

        public RaffleBLL(IRaffleDAL raffleDAL, ILogger<RaffleBLL> logger)
        {
            _raffleDAL = raffleDAL;
            _logger = logger;
        }

        public async Task AddRaffle(Raffle raffle)
        {
            _logger.LogWarning("BLL: Starting raffle for GiftId: {GiftId}", raffle.GiftId);

            var gift = await _raffleDAL.GetGiftWithBuyers(raffle.GiftId);

            if (gift.IsDrawn)
            {
                _logger.LogWarning("BLL: Raffle failed - gift already drawn. GiftId: {GiftId}", raffle.GiftId);
                throw new InvalidOperationException("Gift already drawn");
            }

            if (gift.Cards == null || !gift.Cards.Any())
            {
                _logger.LogWarning("BLL: Raffle failed - no buyers. GiftId: {GiftId}", raffle.GiftId);
                throw new InvalidOperationException("No buyers for this gift");
            }

            if (gift.Amount > gift.Cards.Count)
            {
                _logger.LogWarning("BLL: Raffle failed - not enough buyers. GiftId: {GiftId}", raffle.GiftId);
                throw new InvalidOperationException("Not enough buyers for this gift");
            }

            var random = new Random();

            var Users = gift.Cards
                .Select(c => c.User)
                .ToList();

            _logger.LogInformation("BLL: Raffle started. GiftId: {GiftId}, WinnersToDraw: {Amount}, BuyersCount: {Count}",
                gift.Id, gift.Amount, Users.Count);

            for (int i = 0; i < gift.Amount; i++)
            {
                int index = random.Next(Users.Count);
                var winner = Users[index];

                var newRaffle = new Raffle
                {
                    GiftId = gift.Id,
                    WinnerId = winner.Id
                };

                await _raffleDAL.AddRaffle(newRaffle);

                _logger.LogInformation("BLL: Winner selected. GiftId: {GiftId}, WinnerId: {WinnerId}",
                    gift.Id, winner.Id);

                Users.RemoveAt(index);
            }

            gift.IsDrawn = true;
            await _raffleDAL.Save();

            _logger.LogWarning("BLL: Raffle completed successfully. GiftId: {GiftId}", gift.Id);
        }
    }
}