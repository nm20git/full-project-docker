using project.Models;
using project.Models.DTO;
using System;

namespace project.DAL.Intefaces
{
    public interface IGiftDAL
    {
        Task<List<Gift>> Get();
        Task<Gift?> Get(int id);
        Task Add(Gift gift);
        Task Delete(int Id);
        Task Update(Gift gift);
        Task<List<User>> GetBuyers(int giftId);
        Task<List<object>> GetAllWinners();
        Task<List<User>> GetWinners(int giftId);
        Task<List<Gift>> FilterGifts(GiftFilterDTO filter);
        Task ResetAllDrawStatus();
        Task<List<Gift>> GetSortedByBuyers();
        Task<List<Gift>> GetSortedByPrice();
    }
}
