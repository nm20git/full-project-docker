using project.BLL.Interfaces;
using project.DAL.Intefaces;
using project.DAL.Interfaces;
using project.Models;
using Microsoft.Extensions.Logging;

namespace project.BLL
{
    public class CardBLL : ICardBLL
    {
        private readonly ICardDAL _cardDAL;
        private readonly ILogger<CardBLL> _logger;

        public CardBLL(ICardDAL cardDAL, ILogger<CardBLL> logger)
        {
            _cardDAL = cardDAL;
            _logger = logger;
        }

        public async Task Add(Card card, int quantity)
        {
            _logger.LogInformation("BLL: Starting card purchase. UserId: {UserId}, GiftId: {GiftId}, Quantity: {Quantity}",
                card.UserId, card.GiftId, quantity);

            if (quantity <= 0)
            {
                _logger.LogWarning("BLL: Invalid quantity for card purchase. UserId: {UserId}, Quantity: {Quantity}",
                    card.UserId, quantity);

                throw new ArgumentException("quantity must be greater than zero");
            }

            for (int i = 0; i < quantity; i++)
            {
                var newCard = new Card
                {
                    UserId = card.UserId,
                    GiftId = card.GiftId
                };

                await _cardDAL.Add(newCard);
            }

            _logger.LogInformation("BLL: Card purchase completed successfully. UserId: {UserId}, GiftId: {GiftId}, Quantity: {Quantity}",
                card.UserId, card.GiftId, quantity);
        }

        public async Task<int> GetTotal()
        {
            _logger.LogInformation("BLL: Getting total income");

            var total = await _cardDAL.GetTotal();

            if (total < 0)
            {
                _logger.LogError("BLL: Total income is negative! Value: {Total}", total);
                throw new InvalidOperationException("Total income cannot be negative");
            }

            _logger.LogInformation("BLL: Total income calculated successfully. Total: {Total}", total);

            return total;
        }
    }
}
