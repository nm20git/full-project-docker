using project.Models;
using project.Models.DTO;

namespace project.BLL.Interface
{
    public interface ISponsorBLL
    {
        Task<List<Sponsor>> Get();
        Task<Sponsor> Get(int Id);
        Task Add(Sponsor sponsor);
        Task Update(Sponsor sponsor);
        Task Delete(int Id);
        Task<List<Gift>> GetGifts(int sponsorId);
        Task<List<Sponsor>> FilterSponsors(SponsorFilterDTO filter);

    }
}
