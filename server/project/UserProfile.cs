using AutoMapper;
using project.Models;
using project.Models.DTO;

namespace project
{
    public class ProjectProfile : Profile
    {
        //מיפוי של הUser 
        //מתעלם מהשדות של הסיסמה
        public ProjectProfile() 
        {
            CreateMap<UserDTO, User>()
                .ForMember(x => x.PasswordHash, y => y.Ignore())
                .ForMember(x => x.PasswordSalt, y => y.Ignore())
                .ForMember(x => x.Cards, y => y.Ignore());

            CreateMap<CardDTO, Card>()
               .ForMember(x => x.User, y => y.Ignore())
               .ForMember(x => x.Gift, y => y.Ignore());

            CreateMap<SponsorDTO, Sponsor>()
              .ForMember(x => x.Gifts, y => y.Ignore());

            CreateMap<Sponsor, SponsorDTO>();

            CreateMap<GiftDTO, Gift>()
                .ForMember(x => x.sponsor, y => y.Ignore())
                .ForMember(x => x.Cards, y => y.Ignore())
                .ForMember(x => x.Winners, y => y.Ignore())
                .ForMember(x => x.IsDrawn, y => y.Ignore());

            CreateMap<GiftCreateDTO, Gift>()
                .ForMember(x => x.sponsor, y => y.Ignore())
                .ForMember(x => x.Cards, y => y.Ignore())
                .ForMember(x => x.Winners, y => y.Ignore())
                .ForMember(x => x.IsDrawn, y => y.Ignore());

            CreateMap<Gift, GiftDTO>()
                .ForMember(dest => dest.PurchasesCount,
                 opt => opt.MapFrom(src => src.Cards.Count));


            CreateMap<RaffleDTO, Raffle>();

        }
    }
}
