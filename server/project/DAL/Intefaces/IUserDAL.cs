using project.Models;

namespace project.DAL.Intefaces
{
    public interface IUserDAL
    {
        Task Add(User user);
        Task<User?> GetByEmail(string email);
        Task<List<Gift>> GetUserGifts(int userId);

    }
}
