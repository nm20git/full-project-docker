using project.Models;

namespace project.DAL.Intefaces
{
    public interface IBasketItemDAL
    {
        Task<List<BasketItem>> Get(int userId);
        Task AddItem(int userId, int giftId);
        Task UpdateQuantity(int userId, int giftId, string action);
        Task DeleteItem(int userId, int giftId);
        Task ClearBasket(int userId);


        
    }
}
