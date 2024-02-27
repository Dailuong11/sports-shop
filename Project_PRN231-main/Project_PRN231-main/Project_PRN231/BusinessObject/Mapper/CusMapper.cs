using AutoMapper;
using BusinessObject.Models;
using BusinessObject.Req;
using BusinessObject.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Mapper
{
    public class CusMapper : Profile
    {
        public CusMapper() {
            CreateMap<CusReq, Customer>();
            CreateMap<Customer, CusRes>()
                .ForMember(dest => dest.Email, 
                opt => opt.MapFrom(src => src.Accounts.FirstOrDefault(x => x.CustomerId == src.CustomerId)!.Email));
            CreateMap<Customer, CusSelectRes>();
        }
    }
}
