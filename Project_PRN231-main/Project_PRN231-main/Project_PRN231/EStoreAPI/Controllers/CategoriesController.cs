using AutoMapper;
using DataAccess.RepoImpl;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Models;
using BusinessObject.Res;
using BusinessObject.Req;
using DocumentFormat.OpenXml.Drawing;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using EStoreAPI.Config;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        private ICateRepo repository = new CateRepo();
        private IMapper mapper;
        public CategoriesController(IMapper _mapper) => mapper = _mapper;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationParams @params, string? name)
        {
            var data = await repository.Categories(@params, name);
            var paginationMetadata = new PaginationMetadata(repository.Categories(name).Result.Count, @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(data.Select(mapper.Map<Category, CateRes>).ToList());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            if (id is null) return BadRequest();
            var cate = await repository.Category(id);
            return cate is null ? NotFound() : Ok(mapper.Map<CateRes>(cate));
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpPost]
        public async Task<IActionResult> Post(CateReq cate)
        {
            if (cate is null) return BadRequest();
            var isSave = await repository.Save(mapper.Map<Category>(cate));
            if (isSave) return Ok(isSave);
            return Conflict();
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpPost]
        [Route("export")]
        public async Task<IActionResult> Post()
        {
            var cates = await repository.Categories(null);
            using (var workbook = new XLWorkbook())
            {
                ExcelConfig.export(cates, workbook);
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(
                       content,
                       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       "categories.xlsx");
                }
            }
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int? id, CateReq req)
        {
            if (id is null) return BadRequest();
            var category = await repository.Category(id);
            if (category is not null) return Ok(await repository.Update(mapper.Map<Category>(req)));
            return Conflict();
        }

        [Authorize(Policy = "EmpOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            var category = await repository.Category(id);
            if (category is not null) return Ok(await repository.Delete(category));
            return Conflict();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("selectlist")]
        public async Task<IActionResult> GetSelectList()
        {
            var data = await repository.Categories(null);
            return Ok(data.Select(mapper.Map<Category, CateSelectRes>).ToList());
        }
    }
}
