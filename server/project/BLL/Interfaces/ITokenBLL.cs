using project.Models;

namespace project.BLL.Interfaces
{
    public interface ITokenBLL
    {
        string CreateToken(User user);
    }
}
