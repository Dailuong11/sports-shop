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
    public class EmpMapper : Profile
    {
        public EmpMapper() {
            #pragma warning disable CS8602
            CreateMap<Employee, EmpRes>()
                .ForMember(
                    dest => dest.Department,
                    opt => opt.MapFrom(src => src.Department.DepartmentName))
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.Accounts.FirstOrDefault(x => x.EmployeeId == src.EmployeeId).Email));
            CreateMap<EmpReq, Employee>();
        }
    }
}
