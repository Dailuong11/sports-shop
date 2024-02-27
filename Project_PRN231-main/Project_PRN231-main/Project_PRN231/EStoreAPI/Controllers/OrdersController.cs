using AutoMapper;
using BusinessObject.Models;
using BusinessObject.Res;
using DataAccess.RepoImpl;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using EStoreAPI.Config;
using Aspose.Pdf;
using System.Reflection;
using Microsoft.Extensions.Options;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private IOrderRepo repository = new OrderRepo();
        private IProductRepo repo = new ProductRepo();
        private IMapper mapper;
        public OrdersController(IMapper _mapper, IConfiguration configuration)
        {
            this.configuration = configuration;
            mapper = _mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationParams @params, DateTime? from, DateTime? to)
        {
            var data = await repository.Orders(@params, from, to);
            var paginationMetadata = new PaginationMetadata(repository.Orders(from, to, null).Result.Count, @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(data.Select(mapper.Map<Order, OrderRes>).ToList());
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            if (id is null) return BadRequest();
            var order = await repository.Order(id);
            return order is null ? NotFound() : Ok(mapper.Map<OrderRes>(order));
        }

        [Authorize]
        [HttpPost]
        [Route("save/{email}")]
        public async Task<IActionResult> Post(Order? order, string? email)
        {
            if (order is null || email is null) return BadRequest();
            var isSave = await repository.Save(order);
            if (!isSave) return Conflict();
            var orderToSend = await repository.Order(order.OrderId);
            var isSend = MailConfig.SendOrderMail(mapper.Map<OrderRes>(orderToSend), email, configuration);
            return Ok(isSend);
        }

        [Authorize]
        [HttpPost]
        [Route("cancelorder/{id}")]
        public async Task<IActionResult> CancelOrder(int? id)
        {
            if (id is null) return BadRequest();
            var order = await repository.Order(id);
            if (order is null) return NotFound();
            order.RequiredDate = null;
            var isSave = await repository.Update(order);
            if (!isSave) return Conflict();
            return Ok(isSave);
        }

        [HttpGet]
        [Route("OrderMonth")]
        public async Task<IActionResult> GetTotalEmployees()
        {
            var data = await repository.OrderMonth();
            return Ok(data.Count());
        }
    }
}
