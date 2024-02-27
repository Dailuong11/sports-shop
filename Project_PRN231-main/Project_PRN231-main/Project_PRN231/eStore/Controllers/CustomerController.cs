using BusinessObject.Models;
using BusinessObject.Req;
using BusinessObject.Res;
using DocumentFormat.OpenXml.Wordprocessing;
using eStore.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace eStore.Controllers
{
    public class CustomerController : Controller
    {

        [Authorize(Roles = "1")]
        public async Task<IActionResult> Customers([FromQuery] PaginationParams @params, string search, string title, int item)
        {
            if (@params.ItemsPerPage == 0) @params.ItemsPerPage = 10;
            if (item > 10) @params.ItemsPerPage = item;

            var conn = string.IsNullOrEmpty(search)
                ? $"api/Customers?Page={@params.Page}&ItemsPerPage={@params.ItemsPerPage}"
                : $"api/Customers?Page={@params.Page}&ItemsPerPage={@params.ItemsPerPage}&name={search}";
            conn = string.IsNullOrEmpty(title)
                ? $"{conn}"
                : $"{conn}&title={title}";
            var _conn = $"api/Customers/select";
            var Res = await ResponseConfig.GetData(conn, GetCookie());
            var _Res = await ResponseConfig.GetData(_conn, GetCookie());
            var customers = JsonConvert.DeserializeObject<List<CusRes>>(Res.Content.ReadAsStringAsync().Result);
            var pagination = JsonConvert.DeserializeObject<PaginationMetadata>(Res.Headers.GetValues("X-Pagination").FirstOrDefault()!);
            HashSet<CusSelectRes>? listTitle = JsonConvert.DeserializeObject<HashSet<CusSelectRes>>(_Res.Content.ReadAsStringAsync().Result);



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


            var con6 = $"api/Accounts/totalEmployeesAccounts";
            var Res6 = await ResponseConfig.GetData(con6, GetCookie());

            var employee = JsonConvert.DeserializeObject(Res6.Content.ReadAsStringAsync().Result);

            ViewBag.employee = employee;

            ViewData["search"] = search;
            ViewData["pagination"] = pagination;
            listTitle = listTitle!.DistinctBy(x => x.ContactTitle).ToHashSet();
            ViewBag.Title = listTitle;
            ViewBag.Contact = title;
            ViewBag.Item = item;
            return View(customers);
        }

        [Authorize(Roles = "1")]
        public async Task<IActionResult> Status(string id)
        {
            var conn = $"api/Customers/{id}";
            var Res = await ResponseConfig.GetData(conn, GetCookie());
            var cus = JsonConvert.DeserializeObject<CusRes>(Res.Content.ReadAsStringAsync().Result);
            CusReq req = new CusReq
            {
                CustomerId = cus!.CustomerId,
                CompanyName = cus!.CompanyName,
                ContactName = cus.ContactName!.Contains("(deactive)") 
                ? $"{cus!.ContactName.Split("(")[0]}" 
                : $"{cus!.ContactName}(deactive)",
                ContactTitle = cus.ContactTitle,
                Address = cus!.Address,

            };
            var _Res = await ResponseConfig.PutData(conn, JsonConvert.SerializeObject(req), GetCookie());

            return RedirectToAction("Customers");

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
