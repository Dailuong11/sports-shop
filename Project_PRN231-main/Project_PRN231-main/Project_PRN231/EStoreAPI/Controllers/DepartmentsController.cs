using AutoMapper;
using DataAccess.RepoImpl;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Models;
using BusinessObject.Res;
using Newtonsoft.Json;
using BusinessObject.Req;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private IDepRepo repository = new DepRepo();
        private IMapper mapper;
        public DepartmentsController(IMapper _mapper) => mapper = _mapper;

        [Authorize(Policy = "EmpOnly")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationParams @params, string? name)
        {
            var data = await repository.Departments(@params, name);
            var paginationMetadata = new PaginationMetadata(repository.Departments(name).Result.Count, @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(data.Select(mapper.Map<Department, DepRes>).ToList());
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            if (id is null) return BadRequest();
            var department = await repository.Department(id);
            return department is null ? NotFound() : Ok(mapper.Map<DepRes>(department));
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpPost]
        public async Task<IActionResult> Post(DepReq dep)
        {
            if (dep is null) return BadRequest();
            var isSave = await repository.Save(mapper.Map<Department>(dep));
            if (isSave) return Ok(isSave);
            return Conflict();
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int? id, DepReq dep)
        {
            if (id is null) return BadRequest();
            var department = await repository.Department(id);
            if (department is not null) return Ok(await repository.Update(mapper.Map<Department>(dep)));
            return Conflict();
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            var department = await repository.Department(id);
            if (department is not null) return Ok(await repository.Delete(department));
            return Conflict();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("selectlist")]
        public async Task<IActionResult> GetSelectList()
        {
            var data = await repository.Departments(null);
            return Ok(data.Select(mapper.Map<Department, DepSelectRes>).ToList());
        }
    }
}
