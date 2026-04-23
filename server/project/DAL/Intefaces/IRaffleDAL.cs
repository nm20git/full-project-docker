using project.Models;

namespace project.DAL.Intefaces
{
    public interface IRaffleDAL
    {
        Task<Gift> GetGiftWithBuyers(int giftId);
        Task AddRaffle(Raffle raffle);
        Task Save();
        Task DeleteAll();

    }
}
