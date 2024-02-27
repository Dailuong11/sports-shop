using BusinessObject.Models;
using BusinessObject.Req;
using BusinessObject.Res;
using eStore.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing;
using System.Net.Http.Headers;
using System.Text;

namespace eStore.Controllers
{
	public class EmployeeController : Controller
	{

		[Authorize(Roles = "1")]
		public async Task<IActionResult> Employees(
			[FromQuery] PaginationParams @params,
			int item,
			string? search,
			int? dep,
			string? courtesy,
			string? title,
			DateTime? from,
			DateTime? to)
		{

			if (@params.ItemsPerPage == 0) @params.ItemsPerPage = 10;
			if (item > 10) @params.ItemsPerPage = item;
			var conn = string.IsNullOrEmpty(search)
				? $"api/Employees?Page={@params.Page}&ItemsPerPage={@params.ItemsPerPage}"
				: $"api/Employees?Page={@params.Page}&ItemsPerPage={@params.ItemsPerPage}&name={search}";

			conn = dep is null
				? $"{conn}"
				: $"{conn}&dep={dep}";
			conn = courtesy is null
				? $"{conn}"
				: $"{conn}&courtesy={courtesy}";
			conn = title is null
				? $"{conn}"
				: $"{conn}&title={title}";
			conn = from is null
				? $"{conn}"
				: $"{conn}&from={DateTime.Parse(from.ToString()!).ToString("MM/dd/yyyy")}";
			conn = to is null
				? $"{conn}"
				: $"{conn}&to={DateTime.Parse(to.ToString()!).ToString("MM/dd/yyyy")}";

			var _conn = $"api/Departments/selectlist";
			var Res = await ResponseConfig.GetData(conn, GetCookie());
			var _Res = await ResponseConfig.GetData(_conn, GetCookie());

			var employees = JsonConvert.DeserializeObject<List<EmpRes>>(Res.Content.ReadAsStringAsync().Result);
			var pagination = JsonConvert.DeserializeObject<PaginationMetadata>(Res.Headers.GetValues("X-Pagination").FirstOrDefault()!);
			List<DepSelectRes>? depart = JsonConvert.DeserializeObject<List<DepSelectRes>>(_Res.Content.ReadAsStringAsync().Result);



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
			if (from is not null) ViewData["from"] = DateTime.Parse(from.ToString()!).ToString("yyyy-MM-dd");
			if (to is not null) ViewData["to"] = DateTime.Parse(to.ToString()!).ToString("yyyy-MM-dd");
			ViewData["courtesy"] = courtesy;
			ViewData["title"] = title;
			ViewBag.departments = depart;
			ViewBag.Dep = dep;
			ViewBag.Item = item;
			ViewData["pagination"] = pagination;
			return View(employees);
		}

		[Authorize(Roles = "1")]
		public async Task<IActionResult> Upload(IFormFile? file)
		{
			if (file == null) return RedirectToAction("Employees");
			var bytes = new byte[file.OpenReadStream().Length + 1];
			file.OpenReadStream().Read(bytes, 0, bytes.Length);
			var Res = await ResponseConfig.UploadData("api/Accounts/import-employees", bytes, file.FileName, GetCookie());
			return RedirectToAction("Employees");
		}

		[Authorize(Roles = "1")]
		public async Task<IActionResult> Edit(int id)
		{
			var conn = $"api/Employees/{id}";
			var _conn = $"api/Departments/selectlist";
			var Res = await ResponseConfig.GetData(conn, GetCookie());
			var _Res = await ResponseConfig.GetData(_conn, GetCookie());
			var employee = JsonConvert.DeserializeObject<EmpRes>(Res.Content.ReadAsStringAsync().Result);
			List<DepSelectRes>? depart = JsonConvert.DeserializeObject<List<DepSelectRes>>(_Res.Content.ReadAsStringAsync().Result);
			ViewBag.departments = depart;
			ViewData["BirthDate"] = DateTime.Parse(employee!.BirthDateString!).ToString("yyyy-MM-dd");
			ViewData["HireDate"] = DateTime.Parse(employee!.HireDateString!).ToString("yyyy-MM-dd");
			return View(employee);
		}

		[Authorize(Roles = "1")]
		[HttpPost]
		public async Task<IActionResult> Edit(EmpRes emp)
		{
			EmpReq req = new EmpReq
			{
				EmployeeId = emp.EmployeeId,
				LastName = emp.LastName,
				FirstName = emp.FirstName,
				DepartmentId = emp.DepartmentId,
				Title = emp.Title,
				TitleOfCourtesy = emp.TitleOfCourtesy,
				BirthDate = emp.BirthDate,
				HireDate = emp.HireDate,
				Address = emp.Address,
			};

			EmpSignUpReq empSignUpReq = new EmpSignUpReq
			{
				EmployeeId = emp.EmployeeId,
				Email = emp.Email,
				Password = emp.Password,
			};
			var conn = $"api/Employees/{emp.EmployeeId}";
			var _conn = $"api/Accounts/add-edit-account";
			var Res = await ResponseConfig.PutData(conn, JsonConvert.SerializeObject(req), GetCookie());
			if (emp.Password is not null)
			{
				var _Res = await ResponseConfig.PostData(_conn, JsonConvert.SerializeObject(empSignUpReq), GetCookie());
			}
			return RedirectToAction("Employees");
		}

		[Authorize(Roles = "1")]
		public async Task<IActionResult> Status(int id)
		{
			var conn = $"api/Employees/{id}";
			var Res = await ResponseConfig.GetData(conn, GetCookie());
			var emp = JsonConvert.DeserializeObject<EmpRes>(Res.Content.ReadAsStringAsync().Result);
			EmpReq req = new EmpReq
			{
				EmployeeId = emp!.EmployeeId,
				LastName = emp.LastName,
				FirstName = emp.FirstName,
				DepartmentId = emp.DepartmentId,

				Title = emp.Title!.Contains("(deactive)")
				? $"{emp.Title!.Split("(")[0]}"
				: $"{emp.Title!}(deactive)",
				TitleOfCourtesy = emp.TitleOfCourtesy,
				BirthDate = emp.BirthDate,
				HireDate = emp.HireDate,
				Address = emp.Address,
			};
			var _Res = await ResponseConfig.PutData(conn, JsonConvert.SerializeObject(req), GetCookie());
			return RedirectToAction("Employees");
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
