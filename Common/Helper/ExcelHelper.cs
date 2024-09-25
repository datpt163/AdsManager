using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace FBAdsManager.Common.Helper
{
    public interface IExcelHelper
    {
        Task<List<AdsAccountExcelModel>> ReadExcelFileAsync(IFormFile file);
    }

    public class ExcelHelper : IExcelHelper
    {
        public async Task<List<AdsAccountExcelModel>> ReadExcelFileAsync(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 
            var result = new List<AdsAccountExcelModel>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Bắt đầu từ hàng thứ 2 (bỏ qua tiêu đề)
                    {
                        var item = new AdsAccountExcelModel
                        {
                            AccountID = worksheet.Cells[row, 1].Text,
                            Email = worksheet.Cells[row, 2].Text,
                            PmId = worksheet.Cells[row, 3].Text
                        };

                        result.Add(item);
                    }
                }
            }

            return result;
        }
    }

    public class AdsAccountExcelModel
    {
        public string AccountID { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PmId { get; set; } = string.Empty;
    }
}
