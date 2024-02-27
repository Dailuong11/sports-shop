using AutoMapper;
using BusinessObject.Models;
using BusinessObject.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Mapper
{
    public class OrderMapper : Profile
    {
        public OrderMapper()
        {
            CreateMap<Order, OrderRes>()
                .ForPath(dest => dest.emp!.FullName,
                opt => opt.MapFrom(src => src.Employee!.FullName))
                 .ForPath(dest => dest.emp!.Department,
                opt => opt.MapFrom(src => src.Employee!.Department!.DepartmentName))
                .ForPath(dest => dest.emp!.Title,
                opt => opt.MapFrom(src => src.Employee!.Title))
                .ForPath(dest => dest.cus!.ContactName,
                opt => opt.MapFrom(src => src.Customer!.ContactName))
                .ForPath(dest => dest.cus!.CompanyName,
                opt => opt.MapFrom(src => src.Customer!.CompanyName))
                .ForPath(dest => dest.cus!.ContactTitle,
                opt => opt.MapFrom(src => src.Customer!.ContactTitle))
                .ForPath(dest => dest.cus!.Address,
                opt => opt.MapFrom(src => src.Customer!.Address))
                .ForPath(dest => dest.cus!.Address,
                opt => opt.MapFrom(src => src.OrderDetails));
            CreateMap<OrderDetail, OrderDetailRes>()
                .ForMember(dest => dest.ProductName, 
                opt => opt.MapFrom(src => src.Product!.ProductName))
                .ForPath(dest => dest.Category,
                opt => opt.MapFrom(src => src.Product!.Category!.CategoryName));

        }
    }
}
