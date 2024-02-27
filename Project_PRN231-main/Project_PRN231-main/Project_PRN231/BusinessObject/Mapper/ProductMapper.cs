using AutoMapper;
using BusinessObject.Models;
using BusinessObject.Req;
using BusinessObject.Res;

namespace BusinessObject.Mapper
{
    public class ProductMapper : Profile
    {
        public ProductMapper() { 
            CreateMap<Product, ProductRes>()
                .ForMember(
                    dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category!.CategoryName));
            CreateMap<ProductReq, Product>();
        }
    }
}
