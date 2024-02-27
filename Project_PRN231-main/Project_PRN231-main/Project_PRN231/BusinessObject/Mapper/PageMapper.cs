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
    public class PageMapper : Profile
    {
        public PageMapper()
        {
            CreateMap<PageReq, PageRes>();
            CreateMap<PageRes, Page>();
            CreateMap<Page, PageRes>();
            CreateMap<PageRes, PageReq>();
        }
    }
}
