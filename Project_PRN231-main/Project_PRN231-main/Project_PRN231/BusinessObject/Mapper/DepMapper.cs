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
    public class DepMapper : Profile
    {
        public DepMapper()
        {
            CreateMap<Department, DepRes>();
            CreateMap<DepReq, Department>();
            CreateMap<Department, DepSelectRes>();
        }
    }
}
