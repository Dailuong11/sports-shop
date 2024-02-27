using BusinessObject.Models;
using BusinessObject.Res;
using eStore.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eStore.Controllers
{
    public class OrderController : Controller
    {
       
        public async Task<IActionResult> OrderManager(
           [FromQuery] PaginationParams @params,
          
           DateTime? from,
           DateTime? to)
        {

            if (@params.ItemsPerPage == 0) @params.ItemsPerPage = 10;
            var conn = $"api/Orders?Page={@params.Page}&ItemsPerPage={@params.ItemsPerPage}";
            conn = from is null
                ? $"{conn}"
                : $"{conn}&from={DateTime.Parse(from.ToString()!).ToString("MM/dd/yyyy")}";
            conn = to is null
                ? $"{conn}"
                : $"{conn}&to={DateTime.Parse(to.ToString()!).ToString("MM/dd/yyyy")}";

            var Res = await ResponseConfig.GetData(conn, GetCookie());

            var orders = JsonConvert.DeserializeObject<List<OrderRes>>(Res.Content.ReadAsStringAsync().Result);
            var pagination = JsonConvert.DeserializeObject<PaginationMetadata>(Res.Headers.GetValues("X-Pagination").FirstOrDefault()!);
         
            if (from is not null) ViewData["from"] = DateTime.Parse(from.ToString()!).ToString("yyyy-MM-dd");
            if (to is not null) ViewData["to"] = DateTime.Parse(to.ToString()!).ToString("yyyy-MM-dd");


            var con3 = $"api/Accounts/totalCustomersAccounts";
            var Res3 = await ResponseConfig.GetData(con3, GetCookie());

            var a = JsonConvert.DeserializeObject(Res3.Content.ReadAsStringAsync().Result);

            ViewBag.TotalCustomer = a;

            var con4 = $"api/Accounts/Page";
            var Res4 = await ResponseConfig.GetData(con4, GetCookie());

            var viewPage = JsonConvert.DeserializeObject(Res4.Content.ReadAsStringAsync().Result);

            ViewBag.ViewPage = viewPage;

            var con5 = $"api/Orders/OrderMonth";
            var Res5 = await ResponseConfig.GetData(con5, GetCookie());

            var renuve = JsonConvert.DeserializeObject(Res5.Content.ReadAsStringAsync().Result);

            ViewBag.renuve = renuve;

            ViewData["pagination"] = pagination;
			return View(orders);
		}

        private string? GetCookie()
        {
            var cookies = "";
            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["accessToken"]))
            {
                cookies = HttpContext.Request.Cookies["accessToken"];
            }
            return cookies;
        }
    }
}
