using project.Models;

namespace project.DAL.Intefaces
{
    public interface ICardDAL
    {
        Task Add(Card card);
        Task DeleteAll();
        Task<int> GetTotal();
    }
}
