using BusinessObject.Models;
using BusinessObject.Req;
using BusinessObject.Res;
using eStore.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http.Headers;
using System.Text;

namespace eStore.Controllers
{
    public class ProductController : Controller
    {

        [Authorize(Roles = "1")]
        public async Task<IActionResult> Products([FromQuery] PaginationParams @params, string? search, int? categoryId, int item)
        {
            if (@params.ItemsPerPage == 0) @params.ItemsPerPage = 10;
            if (item > 10) @params.ItemsPerPage = item;

            var conn = string.IsNullOrEmpty(search)
                ? $"api/Products?Page={@params.Page}&ItemsPerPage={@params.ItemsPerPage}"
                : $"api/Products?Page={@params.Page}&ItemsPerPage={@params.ItemsPerPage}&productName={search.Trim()}";
            conn = categoryId is null
               ? $"{conn}"
               : $"{conn}&categoryId={categoryId}";
            var _conn = $"api/Categories/selectlist";
            var Res = await ResponseConfig.GetData(conn, GetCookie());
            var _Res = await ResponseConfig.GetData(_conn, GetCookie());
            var products = JsonConvert.DeserializeObject<List<ProductRes>>(Res.Content.ReadAsStringAsync().Result);
            var pagination = JsonConvert.DeserializeObject<PaginationMetadata>(Res.Headers.GetValues("X-Pagination").FirstOrDefault()!);
            List<CateSelectRes>? category = JsonConvert.DeserializeObject<List<CateSelectRes>>(_Res.Content.ReadAsStringAsync().Result);


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
            ViewBag.categories = category;
            ViewBag.categoryId = categoryId;
            ViewBag.Item = item;
            return View(products);
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        [Route("/Product/delete/{id}")]
        public async Task<IActionResult> productdelete(string id)
        {
            var conn = $"api/Products/delete/{id}";


            var Res = await ResponseConfig.GetData(conn, GetCookie());

            if (!Res.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);
            ViewData["deleteProduct"] = "Can't delete because this product are in " + Res + " Order";
            return Redirect("/Product/Products");
        }

        [Authorize(Roles = "1")]
        public async Task<IActionResult> EditProduct(int id)
        {
            var conn = $"api/products/{id}";
            var _conn = $"api/Categories/selectlist";
            var Res = await ResponseConfig.GetData(conn, GetCookie());
            var _Res = await ResponseConfig.GetData(_conn, GetCookie());
            var product = JsonConvert.DeserializeObject<ProductRes>(Res.Content.ReadAsStringAsync().Result);
            List<CateSelectRes>? category = JsonConvert.DeserializeObject<List<CateSelectRes>>(_Res.Content.ReadAsStringAsync().Result);
            ViewBag.categories = category;
            return View(product);
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductReq pmp, [FromForm] IFormFile fileImage)
        {
            //var bytes = new byte[fileImage.OpenReadStream().Length + 1];
            //fileImage.OpenReadStream().Read(bytes, 0, bytes.Length);
            ProductReq req = new ProductReq
            {
                ProductId = pmp.ProductId,
                ProductName = pmp.ProductName,
                UnitPrice = pmp.UnitPrice,
                UnitsOnOrder = pmp.UnitsOnOrder,
                QuantityPerUnit = pmp.QuantityPerUnit,
                ReorderLevel = pmp.ReorderLevel,
                UnitsInStock = pmp.UnitsInStock,
                Discontinued = pmp.Discontinued,
                CategoryId = pmp.CategoryId,
                Picture = pmp.Picture,
                //Picture = bytes
            };

            var _conn = $"api/products/{req.ProductId}";
            var Res = await ResponseConfig.PutData(_conn, JsonConvert.SerializeObject(req), GetCookie());
            return RedirectToAction("Products");
        }

        [Authorize(Roles = "1")]
        public async Task<IActionResult> AddProduct()
        {

            var _conn = $"api/Categories/selectlist";
            var _Res = await ResponseConfig.GetData(_conn, GetCookie());
            List<CateSelectRes>? category = JsonConvert.DeserializeObject<List<CateSelectRes>>(_Res.Content.ReadAsStringAsync().Result);
            ViewBag.categories = category;
            return View();
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductReq pmp, [FromForm] IFormFile fileImage)
        {

            //var bytes = new byte[fileImage.OpenReadStream().Length + 1];
            //fileImage.OpenReadStream().Read(bytes, 0, bytes.Length);
            ProductReq req = new ProductReq
            {
                ProductName = pmp.ProductName,
                UnitPrice = pmp.UnitPrice,
                UnitsOnOrder = pmp.UnitsOnOrder,
                QuantityPerUnit = pmp.QuantityPerUnit,
                ReorderLevel = pmp.ReorderLevel,
                UnitsInStock = pmp.UnitsInStock,
                Discontinued = pmp.Discontinued,
                CategoryId = pmp.CategoryId,
                Picture = pmp.Picture
            };

            var _conn = $"api/products";
            var Res = await ResponseConfig.PostData(_conn, JsonConvert.SerializeObject(req), GetCookie());
            return RedirectToAction("Products");
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
