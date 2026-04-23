using project.Models;
using project.Models.DTO;
using System;
namespace project.BLL.Interfaces
{
    public interface IGiftBLL
    {
        Task<List<Gift>> Get();
        Task<Gift> Get(int Id);
        Task Add(Gift gift);
        Task Update(Gift gift);
        Task Delete(int Id);
        Task<List<User>> GetBuyers(int giftId);
        Task<List<object>> GetAllWinners();
        Task<List<User>> GetWinners(int giftId);
        Task<List<Gift>> FilterGifts(GiftFilterDTO filter);
        Task ResetSystem();
        Task<List<Gift>> GetSortedByBuyers();
        Task<List<Gift>> GetSortedByPrice();
    }
}
