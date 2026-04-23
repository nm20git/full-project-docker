using project.BLL.Interfaces;
using project.DAL.Intefaces;
using project.Models;
using Microsoft.Extensions.Logging;

namespace project.BLL
{
    public class BasketItemBLL : IBasketItemBLL
    {
        private readonly IBasketItemDAL _basketItemDAL;
        private readonly IGiftDAL _giftDAL;
        private readonly ILogger<BasketItemBLL> _logger;

        public BasketItemBLL(IBasketItemDAL basketItemDAL, IGiftDAL giftDAL, ILogger<BasketItemBLL> logger)
        {
            _basketItemDAL = basketItemDAL;
            _giftDAL = giftDAL;
            _logger = logger;
        }

        public async Task<List<BasketItem>> Get(int userId)
        {
            _logger.LogInformation("BLL: Getting basket items for UserId: {UserId}", userId);

            var items = await _basketItemDAL.Get(userId);

            _logger.LogInformation("BLL: Retrieved {Count} basket items for UserId: {UserId}", items.Count, userId);

            return items;
        }

        public async Task AddToBasket(int userId, int giftId)
        {
            _logger.LogInformation("BLL: Adding gift to basket. UserId: {UserId}, GiftId: {GiftId}", userId, giftId);

            await _basketItemDAL.AddItem(userId, giftId);

            _logger.LogInformation("BLL: Gift added successfully. UserId: {UserId}, GiftId: {GiftId}", userId, giftId);
        }

        public async Task ClearBasket(int userId)
        {
            _logger.LogWarning("BLL: Clearing basket for UserId: {UserId}", userId);

            await _basketItemDAL.ClearBasket(userId);

            _logger.LogInformation("BLL: Basket cleared successfully for UserId: {UserId}", userId);
        }

        public async Task DeleteItem(int userId, int giftId)
        {
            _logger.LogInformation("BLL: Deleting basket item. UserId: {UserId}, GiftId: {GiftId}", userId, giftId);

            await _basketItemDAL.DeleteItem(userId, giftId);

            _logger.LogInformation("BLL: Basket item deleted successfully. UserId: {UserId}, GiftId: {GiftId}", userId, giftId);
        }

        public async Task UpdateQuantity(int userId, int giftId, string action)
        {
            _logger.LogInformation("BLL: Updating quantity. UserId: {UserId}, GiftId: {GiftId}, Action: {Action}",
                userId, giftId, action);

            await _basketItemDAL.UpdateQuantity(userId, giftId, action);

            _logger.LogInformation("BLL: Quantity updated successfully. UserId: {UserId}, GiftId: {GiftId}, Action: {Action}",
                userId, giftId, action);
        }
    }
}
