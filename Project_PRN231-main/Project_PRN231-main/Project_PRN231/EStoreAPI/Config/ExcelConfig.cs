using BusinessObject.Models;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;

namespace EStoreAPI.Config
{
    public class ExcelConfig
    {
        private static readonly String error = "Can't import Excel! Error in column ";

        public static String? excelError { get; set; }

        public static async Task<List<Product>> import(IFormFile file)
        {
            var listProductsFromExcel = new List<Product>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    for (int i = worksheet.Dimension.Start.Row + 1; i <= worksheet.Dimension.End.Row; i++)
                    {
                        try
                        {
                            int j = 0;
                            var productName = GetString(worksheet.Cells[i, ++j]);
                            var unitPrice = GetDecimal(worksheet.Cells[i, ++j]);
                            var quantityPerUnit = GetString(worksheet.Cells[i, ++j]);
                            var unitInStock = GetShort(worksheet.Cells[i, ++j]);
                            var categoryId = GetInt(worksheet.Cells[i, ++j]);
                            var discontinued = GetBool(worksheet.Cells[i, ++j]);
                            listProductsFromExcel.Add(new Product
                            {
                                ProductName = productName ?? "Blank",
                                UnitPrice = unitPrice,
                                QuantityPerUnit = quantityPerUnit,
                                UnitsInStock = unitInStock,
                                CategoryId = categoryId,
                                Discontinued = discontinued ?? false
                            });
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            return listProductsFromExcel;
        }

        public static async Task<List<EmployeeAccount>> import(IFormFile file, string role = "employee")
        {
            var listFromExcel = new List<EmployeeAccount>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    for (int i = worksheet.Dimension.Start.Row + 1; i <= worksheet.Dimension.End.Row; i++)
                    {
                        try
                        {
                            int j = 0;
                            var email = GetString(worksheet.Cells[i, ++j]);
                            var password = GetString(worksheet.Cells[i, ++j]);
                            var lastName = GetString(worksheet.Cells[i, ++j]);
                            var firstName = GetString(worksheet.Cells[i, ++j]);
                            var birthDate = GetDate(worksheet.Cells[i, ++j]);
                            var address = GetString(worksheet.Cells[i, ++j]);
                            var departmentId = GetInt(worksheet.Cells[i, ++j]);
                            var hireDate = GetDate(worksheet.Cells[i, ++j]);
                            var title = GetString(worksheet.Cells[i, ++j]);
                            var TitleOfCourtesy = GetString(worksheet.Cells[i, ++j]);

                            listFromExcel.Add(new EmployeeAccount
                            {
                                Email = email,
                                Password = password,
                                LastName = lastName ?? "Blank",
                                FirstName = firstName ?? " Blank",
                                DepartmentId = departmentId,
                                Title = title,
                                TitleOfCourtesy = TitleOfCourtesy,
                                BirthDate = birthDate,
                                HireDate =  hireDate,
                                Address = address,
                            });
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            return listFromExcel;
        }

        public static XLWorkbook export(List<Product> products, XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheets.Add("Products");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "ProductId";
            worksheet.Cell(currentRow, 2).Value = "ProductName";
            worksheet.Cell(currentRow, 3).Value = "CategoryId";
            worksheet.Cell(currentRow, 4).Value = "CategoryName";
            worksheet.Cell(currentRow, 5).Value = "QuantityPerUnit";
            worksheet.Cell(currentRow, 6).Value = "UnitPrice";
            worksheet.Cell(currentRow, 7).Value = "UnitsInStock";
            worksheet.Cell(currentRow, 8).Value = "UnitsOnOrder";
            worksheet.Cell(currentRow, 9).Value = "ReorderLevel";
            worksheet.Cell(currentRow, 10).Value = "Discontinued";
            foreach (var product in products)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = product.ProductId;
                worksheet.Cell(currentRow, 2).Value = product.ProductName;
                worksheet.Cell(currentRow, 3).Value = product.CategoryId;

                if (product.Category is not null)
                {
                    worksheet.Cell(currentRow, 4).Value = product.Category.CategoryName;
                }
                else
                {
                    worksheet.Cell(currentRow, 4).Value = "";
                }

                worksheet.Cell(currentRow, 5).Value = product.QuantityPerUnit;
                worksheet.Cell(currentRow, 6).Value = product.UnitPrice;
                worksheet.Cell(currentRow, 7).Value = product.UnitsInStock;
                worksheet.Cell(currentRow, 8).Value = product.UnitsOnOrder;
                worksheet.Cell(currentRow, 9).Value = product.ReorderLevel;
                worksheet.Cell(currentRow, 10).Value = product.Discontinued;
            }
            return workbook;
        }

        public static XLWorkbook export(List<Category> categories, XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheets.Add("Categories");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "CategoryId";
            worksheet.Cell(currentRow, 2).Value = "CategoryName";
            worksheet.Cell(currentRow, 3).Value = "Description";
            foreach (var category in categories)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = category.CategoryId;
                worksheet.Cell(currentRow, 2).Value = category.CategoryName;
                worksheet.Cell(currentRow, 3).Value = category.Description;
            }
            return workbook;
        }

        private static string? GetString(ExcelRange excelRange)
        {
            var cell = excelRange.Value;
            if (cell is not null)
            {
                return cell.ToString();
            }

            throw new Exception(excelError = error + excelRange.Start.Address);
        }

        private static DateTime? GetDate(ExcelRange excelRange)
        {
            var cell = excelRange.Value;
            if (cell is not null)
            {
                try { 
                    return Convert.ToDateTime(cell.ToString());
                }
                catch (FormatException e) {
                    excelError = error + excelRange.Start.Address + "/ " + e.Message;
                }
            }
            throw new Exception(excelError = error + excelRange.Start.Address);
        }

        private static decimal? GetDecimal(ExcelRange excelRange)
        {
            var cell = excelRange.Value;
            if (cell is not null)
            {
                try
                {
                    return Convert.ToDecimal(cell);
                }
                catch (FormatException e)
                {
                    excelError = error + excelRange.Start.Address + "/ " + e.Message;
                }
            }
            throw new Exception(excelError = error + excelRange.Start.Address);
        }

        private static short? GetShort(ExcelRange excelRange)
        {
            var cell = excelRange.Value;
            if (cell is not null)
            {
                try
                {
                    return (short?)Convert.ToUInt32(cell);
                }
                catch (FormatException e)
                {
                    excelError = error + excelRange.Start.Address + "/ " + e.Message;
                }
            }
            throw new Exception(excelError = error + excelRange.Start.Address);
        }

        private static short? GetInt(ExcelRange excelRange)
        {
            var cell = excelRange.Value;
            if (cell is not null)
            {
                try
                {
                    return (short?)Convert.ToUInt32(cell);
                }
                catch (FormatException e)
                {
                    excelError = error + excelRange.Start.Address + "/ " + e.Message;
                }
            }
            throw new Exception(excelError = error + excelRange.Start.Address);
        }

        private static bool? GetBool(ExcelRange excelRange)
        {
            var cell = excelRange.Value;
            if (cell is not null)
            {
                try
                {
                    return Convert.ToBoolean(cell);
                }
                catch (FormatException e)
                {
                    excelError = error + excelRange.Start.Address + "/ " + e.Message;
                }
            }
            throw new Exception(excelError = error + excelRange.Start.Address);
        }

    }
}
