using AutoMapper;
using DataAccess.RepoImpl;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Models;
using BusinessObject.Res;
using Newtonsoft.Json;
using BusinessObject.Req;
using EStoreAPI.Config;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private IEmpRepo repository = new EmpRepo();
        private IMapper mapper;
        public EmployeesController(IMapper _mapper) => mapper = _mapper;


        [Authorize(Policy = "EmpOnly")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationParams @params, string? name, int? dep, string? courtesy, string? title, DateTime? from, DateTime? to)
        {            
            var data = await repository.Employees(@params, name, dep, courtesy, title, from, to);
            var paginationMetadata = new PaginationMetadata(repository.Employees(name, dep, courtesy, title, from, to).Result.Count, @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(data.Select(mapper.Map<Employee, EmpRes>).ToList());
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            if (id is null) return BadRequest();
            var employee = await repository.Employee(id);
            return employee is null ? NotFound() : Ok(mapper.Map<EmpRes>(employee));
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpPost]
        public async Task<IActionResult> Post(EmpReq emp)
        {
            if (emp is null) return BadRequest();
            var id = await repository.Save(mapper.Map<Employee>(emp));
            if (id > 0) return Ok(id);
            return Conflict();
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int? id, EmpReq req)
        {
            if (id is null) return BadRequest();
            var employee = await repository.Employee(id);
            if (employee is not null) return Ok(await repository.Update(mapper.Map<Employee>(req)));
            return Conflict();
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            var employee = await repository.Employee(id);
            if (employee is not null) return Ok(await repository.Delete(employee));
            return Conflict();
        }
    }
}
