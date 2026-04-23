using project.Models;

namespace project.BLL.Interfaces
{
    public interface ICardBLL
    {
        Task Add(Card card, int quantity);
        Task<int> GetTotal();
    }
}
