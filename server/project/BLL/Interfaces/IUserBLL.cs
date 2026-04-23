using project.Models;
using project.Models.DTO;

namespace project.BLL.Interfaces
{
    public interface IUserBLL
    {
        Task<(bool Success, string Message)> Register(User user, string password);
        Task<User?> Login(string email, string password);
        Task<List<Gift>> GetUserGifts(int userId);

    }
}
