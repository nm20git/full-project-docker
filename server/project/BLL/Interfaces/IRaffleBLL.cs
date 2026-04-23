using project.Models;
using project.Models.DTO;

namespace project.BLL.Interfaces
{
    public interface IRaffleBLL
    {
        Task AddRaffle(Raffle raffle);
    }
}
