using AutoMapper;
using MagicVille_CouponAPI.Models;
using MagicVille_CouponAPI.Models.DTO;

namespace MagicVille_CouponAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            //this allows us to map our DTO's to our Coupon in both directions
            CreateMap<Coupon, CouponCreateDTO>().ReverseMap();
            CreateMap<Coupon, CouponDTO>().ReverseMap();
            CreateMap<Coupon, CouponUpdateDTO>().ReverseMap();
        }
    }
}
