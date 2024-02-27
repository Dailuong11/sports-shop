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
    public class CategoryMapper : Profile
    {
        public CategoryMapper() {
            CreateMap<Category, CateRes>();
            CreateMap<Category, CateSelectRes>();
            CreateMap<CateReq, Category>();
        }
    }
}
