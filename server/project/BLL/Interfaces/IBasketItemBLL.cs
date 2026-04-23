using project.Models;

namespace project.BLL.Interfaces
{
    public interface IBasketItemBLL
    {
        Task<List<BasketItem>> Get(int userId);
        Task AddToBasket(int userId, int giftId);
        Task UpdateQuantity(int userId, int giftId, string action);
        Task DeleteItem(int userId, int giftId);
        Task ClearBasket(int userId);
    }
}
