using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eStore.Models;
using BusinessObject.Req;
using Newtonsoft.Json;
using eStore.Config;
using BusinessObject.Res;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessObject.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using System.Text.Json;
using System;
using Microsoft.Extensions.Logging;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace eStore.Controllers;

public class HomeController : Controller

{
    private readonly HttpClient client = null;
    private string url = "";
    private string _url = "";
    private readonly IConfiguration configuration;
    public HomeController(IConfiguration configuration)
    {
        this.configuration = configuration;
        client = new HttpClient();
        var contentType = new MediaTypeWithQualityHeaderValue("application/json");
        client.DefaultRequestHeaders.Accept.Add(contentType);
        this.configuration = configuration;
    }

    public async Task<IActionResult> Index([FromQuery] PaginationParams @params, string? search, int? id)
    {
        if (@params.ItemsPerPage == 0) @params.ItemsPerPage = 9;
        var conn2 = string.IsNullOrEmpty(search)
               ? $"api/Products?Page={@params.Page}&ItemsPerPage={@params.ItemsPerPage}"
               : $"api/Products?Page={@params.Page}&ItemsPerPage={@params.ItemsPerPage}&productName={search}";
        conn2 = id is null
               ? $"{conn2}"
               : $"{conn2}&categoryId={id}";

        var conn1 = $"api/Products/sale";
        var conn = $"api/Products/top4";
        var _conn = $"api/Categories/selectlist";
        if (id is null) id = 1;
        var conn3 = $"api/Products/top4byCategory?category={id}";
        var _Res2 = await ResponseConfig.GetData(conn2, GetCookie());
        var Res = await ResponseConfig.GetData(conn, GetCookie());
        var _Res = await ResponseConfig.GetData(_conn, GetCookie());
        var _Res1 = await ResponseConfig.GetData(conn1, GetCookie());
        var _Res3 = await ResponseConfig.GetData(conn3, GetCookie());
        var products = JsonConvert.DeserializeObject<List<ProductRes>>(_Res2.Content.ReadAsStringAsync().Result);
        List<CateSelectRes>? category = JsonConvert.DeserializeObject<List<CateSelectRes>>(_Res.Content.ReadAsStringAsync().Result);
        List<ProductRes>? productlastest = JsonConvert.DeserializeObject<List<ProductRes>>(Res.Content.ReadAsStringAsync().Result);
        List<ProductRes>? productSales = JsonConvert.DeserializeObject<List<ProductRes>>(_Res1.Content.ReadAsStringAsync().Result);
        List<ProductRes>? productTopByCategories = JsonConvert.DeserializeObject<List<ProductRes>>(_Res3.Content.ReadAsStringAsync().Result);
        var pagination = JsonConvert.DeserializeObject<PaginationMetadata>(_Res2.Headers.GetValues("X-Pagination").FirstOrDefault()!);
        ViewBag.categories = category;
        ViewBag.categoryId = id;
        ViewData["search"] = search;
        ViewData["pagination"] = pagination;
        ViewBag.productlastest = productlastest;
        ViewBag.productSales = productSales;
        ViewBag.productTops = productTopByCategories;
        return View(products);
    }
    public async Task<IActionResult> Shop([FromQuery] PaginationParams @params,
            int item,
            int? id)
    {
        if (@params.ItemsPerPage == 0) @params.ItemsPerPage = 9;
        if (item > 10) @params.ItemsPerPage = item;
        url = id is null
            ? $"https://localhost:7177/api/Products?Page={@params.Page}&ItemsPerPage={@params.ItemsPerPage}"
            : $"https://localhost:7177/api/Products?Page={@params.Page}&ItemsPerPage={@params.ItemsPerPage}&categoryId={id}";
        _url = "https://localhost:7177/api/Categories/selectlist";
        var conn1 = $"api/Products/top4";
        var _conn = $"api/Categories/selectlist";
        var _Res1 = await ResponseConfig.GetData(_conn, GetCookie());
        List<CateSelectRes>? category = JsonConvert.DeserializeObject<List<CateSelectRes>>(_Res1.Content.ReadAsStringAsync().Result);
        var Res1 = ResponseConfig.GetData(conn1, GetCookie()).Result;
        List<ProductRes>? productlastest = JsonConvert.DeserializeObject<List<ProductRes>>(Res1.Content.ReadAsStringAsync().Result);

        HttpResponseMessage response = await client.GetAsync(url);
        HttpResponseMessage _response = await client.GetAsync(_url);
        string json = await response.Content.ReadAsStringAsync();
        string _json = await _response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var Res = ResponseConfig.GetData(url, GetCookie()).Result;
        var _Res = ResponseConfig.GetData(_url, GetCookie()).Result;
        var pagination = JsonConvert.DeserializeObject<PaginationMetadata>(Res.Headers.GetValues("X-Pagination").FirstOrDefault()!);
        List<ProductRes>? products = JsonSerializer.Deserialize<List<ProductRes>>(json, options);
        List<CateRes>? categories = JsonSerializer.Deserialize<List<CateRes>>(_json, options);
        ViewBag.categories = category;
        ViewData["pagination"] = pagination;
        ViewBag.productlastest = productlastest;
        ViewBag.categoryId = id;
        ViewBag.Item = item;
        return View(products);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var identity = (ClaimsIdentity)User.Identity!;
        var claims = identity.Claims.ToList();
        var email = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        email = email ?? string.Empty;
        var conn = $"api/Accounts/getEmail/{email}";
        var Res = await ResponseConfig.GetData(conn, GetCookie());
        var account = JsonConvert.DeserializeObject<AccRes>(Res.Content.ReadAsStringAsync().Result);
        var conn1 = $"api/Customers/{account!.CustomerId}";
        var Res2 = await ResponseConfig.GetData(conn1, GetCookie());
        var cus = JsonConvert.DeserializeObject<CusRes>(Res2.Content.ReadAsStringAsync().Result);
        ViewBag.Customer = cus;
        return View(account);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> EditProfile(string? id)
    {
        var identity = (ClaimsIdentity)User.Identity!;
        var claims = identity.Claims.ToList();
        var email = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        email = email ?? string.Empty;
        var conn = $"api/Accounts/getEmail/{email}";
        var Res = await ResponseConfig.GetData(conn, GetCookie());
        var account = JsonConvert.DeserializeObject<AccRes>(Res.Content.ReadAsStringAsync().Result);

        var conn1 = $"api/Customers/{account!.CustomerId}";
        var Res2 = await ResponseConfig.GetData(conn1, GetCookie());
        var cus = JsonConvert.DeserializeObject<CusRes>(Res2.Content.ReadAsStringAsync().Result);

        return View(cus);
    }

    [Authorize]
    [HttpGet]
    [Route("/ChangePass")]
    public async Task<IActionResult> ChangePass()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [Route("/ChangePass")]
    public async Task<IActionResult> ChangePass(AccRes acc)
    {
        var identity = (ClaimsIdentity)User.Identity!;
        var claims = identity.Claims.ToList();
        var email = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        email = email ?? string.Empty;

        var conn = $"api/Accounts/getEmail/{email}";
        var Res = await ResponseConfig.GetData(conn, GetCookie());
        var account = JsonConvert.DeserializeObject<AccRes>(Res.Content.ReadAsStringAsync().Result);

        AccRes req = new AccRes
        {
            AccountId = account!.AccountId,
            Email = account.Email,
            Password = acc.Password,
            CustomerId = account.CustomerId,
            EmployeeId = account.EmployeeId,
            Role = account.Role,

        };

        var _conn = $"api/Accounts/{email}";
        var Res2 = ResponseConfig.PutData(_conn, JsonConvert.SerializeObject(req), GetCookie());

        return RedirectToAction("Profile");
    }

    [HttpGet]
    public async Task<IActionResult> CheckOut([FromForm] OrderDto orderDto)
    {
        var identity = (ClaimsIdentity)User.Identity!;
        var claims = identity.Claims.ToList();
        var email = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        email = email ?? string.Empty;
        var conn = $"api/Accounts/getEmail/{email}";
        var Res = await ResponseConfig.GetData(conn, GetCookie());
        var _conn = $"api/Categories/selectlist";
        var _Res1 = await ResponseConfig.GetData(_conn, GetCookie());
        List<CateSelectRes>? category = JsonConvert.DeserializeObject<List<CateSelectRes>>(_Res1.Content.ReadAsStringAsync().Result);
        var account = JsonConvert.DeserializeObject<AccRes>(Res.Content.ReadAsStringAsync().Result);
        if(account is not null)
        {
            var conn1 = $"api/Customers/{account!.CustomerId}";
            var Res2 = await ResponseConfig.GetData(conn1, GetCookie());
            var cus = JsonConvert.DeserializeObject<CusRes>(Res2.Content.ReadAsStringAsync().Result);
            ViewBag.Customer = cus;
        }
        ViewBag.categories = category;
        return View(orderDto);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EditProfile(CusRes pmp)
    {
        CusRes req = new CusRes
        {
            CustomerId = pmp.CustomerId,
            CompanyName = pmp.CompanyName,
            ContactName = pmp.ContactName,
            ContactTitle = pmp.ContactTitle,
            Address = pmp.Address,

        };

        var _conn = $"api/Customers/{req.CustomerId}";
        var Res = await ResponseConfig.PutData(_conn, JsonConvert.SerializeObject(req), GetCookie());

        return RedirectToAction("Profile");
    }
    [HttpGet]
    [Route("/cart")]
    public async Task<IActionResult> cart()
    {
        var _conn = $"api/Categories/selectlist";
        var _Res1 = await ResponseConfig.GetData(_conn, GetCookie());
        List<CateSelectRes>? category = JsonConvert.DeserializeObject<List<CateSelectRes>>(_Res1.Content.ReadAsStringAsync().Result);
        ViewBag.categories = category;
        return View();
    }

    [HttpPost]
    [Route("/cart")]
    public async Task<IActionResult> cart([FromForm] OrderDto orderDto)
    {
        var _Res = await ResponseConfig.GetData("api/Products/allProductName", GetCookie());
        var allProductName = JsonConvert.DeserializeObject<List<string>>(_Res.Content.ReadAsStringAsync().Result);
        
        
        if (allProductName == null)
            return StatusCode(StatusCodes.Status500InternalServerError);

        if (!string.IsNullOrEmpty(orderDto.action))
        {
            switch (orderDto.action)
            {
                case "BUY NOW":
                    if (!string.IsNullOrEmpty(orderDto.name) && allProductName.Contains(orderDto.name))
                    {
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("cart")))
                        {
                            List<OrderDetailDTO> cart = JsonConvert.DeserializeObject<List<OrderDetailDTO>>(HttpContext.Session.GetString("cart")!)!;
                            foreach (var item in cart)
                            {
                                if (item.Product.ProductName == orderDto.name)
                                {
                                    item.Quantity++;
                                    item.Total = (decimal)item.Product.UnitPrice! * item.Quantity;
                                    break;
                                }
                                else
                                {
                                    await AddToCart(cart, orderDto.name);
                                    break;
                                }
                            }
                            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cart));
                        }
                        else
                        {
                            List<OrderDetailDTO> cart = new List<OrderDetailDTO>();
                            await AddToCart(cart, orderDto.name);
                            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cart));
                        }
                    }
                    return await cart();
                case "ADD TO CART":
                    if (!string.IsNullOrEmpty(orderDto.name) && allProductName.Contains(orderDto.name))
                    {
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("cart")))
                        {
                            List<OrderDetailDTO> cart = JsonConvert.DeserializeObject<List<OrderDetailDTO>>(HttpContext.Session.GetString("cart")!)!;
                            foreach (var item in cart)
                            {
                                if (item.Product.ProductName == orderDto.name)
                                {
                                    item.Quantity++;
                                    item.Total = (decimal)item.Product.UnitPrice! * item.Quantity;
                                    break;
                                }
                                else
                                {
                                    await AddToCart(cart, orderDto.name);
                                    break;
                                }
                            }
                            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cart));
                        }
                        else
                        {
                            List<OrderDetailDTO> cart = new List<OrderDetailDTO>();
                            await AddToCart(cart, orderDto.name);
                            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cart));
                        }
                    }
                    return Redirect("/cart" + orderDto.name);
                case "Remove":
                    if (!string.IsNullOrEmpty(orderDto.name) && allProductName.Contains(orderDto.name))
                    {
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("cart")))
                        {
                            List<OrderDetailDTO> cart = JsonConvert.DeserializeObject<List<OrderDetailDTO>>(HttpContext.Session.GetString("cart")!)!;
                            foreach (var item in cart)
                            {
                                if (item.Product.ProductName == orderDto.name)
                                {
                                    cart.Remove(item);
                                    break;
                                }
                            }
                            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cart));
                        }
                    }
                    return await cart();
                case "+":
                    if (!string.IsNullOrEmpty(orderDto.name) && allProductName.Contains(orderDto.name))
                    {
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("cart")))
                        {
                            List<OrderDetailDTO> cart = JsonConvert.DeserializeObject<List<OrderDetailDTO>>(HttpContext.Session.GetString("cart")!)!;
                            foreach (var item in cart)
                            {
                                if (item.Product.ProductName == orderDto.name)
                                {
                                    item.Quantity++;
                                    item.Total = (decimal)item.Product.UnitPrice! * item.Quantity;
                                    break;
                                }
                            }
                            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cart));
                        }
                    }
                    return await cart();
                case "-":
                    if (!string.IsNullOrEmpty(orderDto.name) && allProductName.Contains(orderDto.name))
                    {
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("cart")))
                        {
                            List<OrderDetailDTO> cart = JsonConvert.DeserializeObject<List<OrderDetailDTO>>(HttpContext.Session.GetString("cart")!)!;
                            foreach (var item in cart)
                            {
                                if (item.Product.ProductName == orderDto.name)
                                {
                                    item.Quantity--;
                                    item.Total = (decimal)item.Product.UnitPrice! * item.Quantity;
                                    if (item.Quantity <= 0)
                                        cart.Remove(item);

                                    break;
                                }
                            }
                            if (cart.Count <= 0)
                                return Redirect("cart");

                            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cart));
                        }
                    }
                    return await cart();
                case "ORDER":
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("cart")))
                    {
                        List<OrderDetailDTO> cart = JsonConvert.DeserializeObject<List<OrderDetailDTO>>(HttpContext.Session.GetString("cart")!)!;
                        var identity = (ClaimsIdentity)User.Identity!;
                        var claims = identity.Claims.ToList();
                        var email = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                        if (email is null)
                            return Redirect("/Home/Index");

                        var conn = $"api/Accounts/getEmail/{email}";
                        var AccRes = await ResponseConfig.GetData(conn, GetCookie());
                        var account = JsonConvert.DeserializeObject<AccRes>(AccRes.Content.ReadAsStringAsync().Result);

                        var conn1 = $"api/Customers/{account!.CustomerId}";
                        var Res2 = await ResponseConfig.GetData(conn1, GetCookie());
                        var cus = JsonConvert.DeserializeObject<CusRes>(Res2.Content.ReadAsStringAsync().Result);

                        List<OrderDetail> orderDetail = new List<OrderDetail>();
                        foreach (var item in cart)
                        {
                            OrderDetail product = new OrderDetail
                            {
                                ProductId = item.Product.ProductId,
                                UnitPrice = (decimal)item.Product.UnitPrice!,
                                Quantity = (short)item.Quantity,
                                Discount = 0
                            };
                            orderDetail.Add(product);
                        }

                        if (((ClaimsIdentity)User.Identity!).HasClaim(ClaimTypes.Role, "2"))
                        {
                            Order order = new Order
                            {
                                CustomerId = cus!.CustomerId,
                                OrderDate = DateTime.Now,
                                RequiredDate = DateTime.Now.AddDays(30),
                                ShipName = cus!.CompanyName,
                                ShipAddress = cus!.Address,
                                OrderDetails = orderDetail
                            };

                            var a = JsonConvert.SerializeObject(order);
                            var Res = await ResponseConfig.PostData($"api/Orders/save/{email}", JsonConvert.SerializeObject(order), GetCookie());
                            HttpContext.Session.Remove("cart");
                            return Redirect("/Home/Index");
                            //if (!Res.IsSuccessStatusCode)
                            //    return StatusCode(StatusCodes.Status500InternalServerError);
                        }
                    }
                    return Redirect("/Home/Index");
            }
        }
        return View();
    }
    public async Task AddToCart(List<OrderDetailDTO> cart, string name)
    {
        var Res = await ResponseConfig.GetData("api/Products/GetProductbyName/" + name, GetCookie());
        var P = JsonConvert.DeserializeObject<Product>(Res.Content.ReadAsStringAsync().Result);
        decimal Total = (decimal)P!.UnitPrice! * 1;
        cart.Add(new OrderDetailDTO { Product = P, Quantity = 1, Total = Total });
    }

    [HttpGet]
    [Route("/product/detail/{id}")]
    public async Task<IActionResult> Detail(string id)
    {
            var conn = $"api/Products/{id}";
            var conn1 = $"api/Products/sale";

            var _conn = $"api/Categories/selectlist";
            var _Res = await ResponseConfig.GetData(_conn, GetCookie());
            var Res = await ResponseConfig.GetData(conn, GetCookie());
            var Res1 = await ResponseConfig.GetData(conn1, GetCookie());

            ProductRes products = JsonConvert.DeserializeObject<ProductRes>(Res.Content.ReadAsStringAsync().Result!)!;
            List<ProductRes>? productSales = JsonConvert.DeserializeObject<List<ProductRes>>(Res1.Content.ReadAsStringAsync().Result);

            List<CateSelectRes>? category = JsonConvert.DeserializeObject<List<CateSelectRes>>(_Res.Content.ReadAsStringAsync().Result);
            ViewBag.categories = category;
            ViewBag.products = products;
            ViewBag.productSales = productSales;
            return View();
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["accessToken"]))
            {
                return Redirect("/Home/Index");
            }
            if (string.IsNullOrEmpty(HttpContext.Request.Cookies["accessToken"]) && !string.IsNullOrEmpty(HttpContext.Request.Cookies["refreshToken"]))
            {
                UserRes u = new UserRes();
                u.RefreshToken = HttpContext.Request.Cookies["refreshToken"];
                var conn = $"api/Accounts/refresh-token";
                var Res = await ResponseConfig.PostData(conn, JsonConvert.SerializeObject(u), GetCookie());
                if (!Res.IsSuccessStatusCode)
                {
                    Response.Cookies.Delete("refreshToken");
                    return RedirectToAction("Login");
                }

                var user = JsonConvert.DeserializeObject<UserRes>(Res.Content.ReadAsStringAsync().Result);

                HttpContext.Session.SetString("user", Res.Content.ReadAsStringAsync().Result);

                ValidateToken(user!.AccessToken!.Replace("\"", ""));

                Response.Cookies.Append("refreshToken", user.RefreshToken!, new CookieOptions { Expires = user.TokenExpires, HttpOnly = true, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict });

                if (user.Account!.Role == 1) return Redirect("https://localhost:7290/Employee/Employees");

                return Redirect("/Home/Index");
            }
            ViewBag.ErrMsg = TempData["ErrorMessage"] as string;
            return View();
    }



    [HttpPost]
    public async Task<IActionResult> Login(AuthReq req)
    {
        var conn = $"api/Accounts/signin";
        var Res = await ResponseConfig.PostData(conn, JsonConvert.SerializeObject(req), GetCookie());
        if (!Res.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Wrong email or password";
            return RedirectToAction("Login");
        }
        var user = JsonConvert.DeserializeObject<UserRes>(Res.Content.ReadAsStringAsync().Result);

        HttpContext.Session.SetString("user", Res.Content.ReadAsStringAsync().Result);

        ValidateToken(user!.AccessToken!.Replace("\"", ""));

        Response.Cookies.Append("refreshToken", user.RefreshToken!, new CookieOptions
        { Expires = user.TokenExpires, HttpOnly = true, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict });
        if (user.Account!.Role == 1) return Redirect("https://localhost:7290/Employee/Employees");
        return RedirectToAction("index");
    }
    [HttpGet]
    public IActionResult Signup()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Signup(SignUpReq req)
    {
        var conn = $"api/Accounts/signup";
        var Res = await ResponseConfig.PostData(conn, JsonConvert.SerializeObject(req), GetCookie());
        if (!Res.IsSuccessStatusCode) return StatusCode(StatusCodes.Status500InternalServerError);
        return RedirectToAction("Signup");
    }

    [HttpGet]
    public  IActionResult Forgot()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Forgot(string email)
    {
        var conn = $"api/Accounts/reset?email={email}";
        var Res = await ResponseConfig.GetData(conn, GetCookie());
        if (!Res.IsSuccessStatusCode) return StatusCode(StatusCodes.Status500InternalServerError);
        return RedirectToAction("Login");
    }


    [Authorize]
    [HttpGet]
    [Route("/logout")]
    public IActionResult signout()
    {
        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");
        HttpContext.Session.Remove("cart");
        return RedirectToAction("index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private void ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JWT:Issuer"],
                ValidAudience = configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            var expires = jwtToken.ValidTo;
            var role = jwtToken.Claims.First(x => x.Type == ClaimTypes.Role);
            var email = jwtToken.Claims.First(x => x.Type == ClaimTypes.Email);
            List<ClaimsIdentity> identities = new List<ClaimsIdentity>
                {
                    new ClaimsIdentity(new[] { role }),
                    new ClaimsIdentity(new[] { email })
                };

            User.AddIdentities(identities);

            Response.Cookies.Append("accessToken", token, new CookieOptions
            { Expires = expires, HttpOnly = true, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict });

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
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