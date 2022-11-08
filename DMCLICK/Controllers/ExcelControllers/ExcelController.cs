using DMCLICK.Entityes;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DMCLICK.Controllers.ExcelControllers
{
    public class ExcelController
    {
        private string Path
        {
            get
            {
                //Тут забито гвозядми т.к. небыло смыла делать настройки
                return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\FilesFromParser\\";
            }
        }

        public void WriteDocumet(List<Apartament> apartaments, string complexName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excel = new ExcelPackage($"{Path}{complexName}_{Guid.NewGuid()}.xlsx"))
            {
                int x = 2;

                ExcelWorksheet sheetCosts = excel.Workbook.Worksheets.Add("Costs");

                sheetCosts.Cells[1, 1].Value = "Стоимость";
                sheetCosts.Cells[1, 2].Value = "Стоимость за м2";
                sheetCosts.Cells[1, 3].Value = "Площадь";
                sheetCosts.Cells[1, 4].Value = "Сроки строительства";
                sheetCosts.Cells[1, 5].Value = "Кол-во комнат";
                foreach (var apartament in apartaments)
                {
                    sheetCosts.Cells[x, 1].Value = apartament.Cost;
                    sheetCosts.Cells[x, 2].Value = apartament.CostFotM2;
                    sheetCosts.Cells[x, 3].Value = apartament.SquareFootage;
                    sheetCosts.Cells[x, 4].Value = apartament.Deadline;
                    sheetCosts.Cells[x, 5].Value = apartament.Type;
                    x++;
                }

                excel.Save();
            }
        }

        public void WriteDocumets(List<Complex> complices)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            foreach (var complex in complices)
            {
                using (ExcelPackage excel = new ExcelPackage($"{Path}{complex.ComplexName}_{Guid.NewGuid()}.xlsx"))
                {
                    int x = 2;

                    ExcelWorksheet sheetCosts = excel.Workbook.Worksheets.Add("Costs");

                    sheetCosts.Cells[1, 1].Value = "Стоимость";
                    sheetCosts.Cells[1, 2].Value = "Стоимость за м2";
                    sheetCosts.Cells[1, 3].Value = "Площадь";
                    sheetCosts.Cells[1, 4].Value = "Сроки строительства";
                    sheetCosts.Cells[1, 5].Value = "Кол-во комнат";
                    foreach (var flat in complex.Apartaments)
                    {
                        sheetCosts.Cells[x, 1].Value = flat.Cost;
                        sheetCosts.Cells[x, 2].Value = new Regex("[^0-9 ]").Replace(flat.CostFotM2, string.Empty);
                        sheetCosts.Cells[x, 3].Value = new Regex("[^0-9 ]").Replace(flat.SquareFootage, string.Empty);
                        sheetCosts.Cells[x, 4].Value = flat.Deadline;
                        sheetCosts.Cells[x, 5].Value = flat.Type;
                        x++;
                    }

                    excel.Save();
                }
            }
        }
    }
}
