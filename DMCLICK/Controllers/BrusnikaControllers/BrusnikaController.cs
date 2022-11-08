using DMCLICK.Controllers.ExcelControllers;
using DMCLICK.Entityes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DMCLICK.Controllers.BrusnikaControllers
{
    public class BrusnikaController
    {
        public void Save(List<Apartament> apartaments)
        {
            ExcelController excelController = new ExcelController();
            excelController.WriteDocumet(apartaments, "brusnikavakademicheskom");
        }
        public List<Apartament> ParceBrusnika()
        {
            List<Apartament> apartaments = new List<Apartament>();
            var task = BrusnikaApiController.GetResultsAsync();
            foreach (var item in task.Result)
            {
                apartaments.Add(new Apartament
                {
                    Cost = item.price.ToString(),
                    CostFotM2 = Math.Round(item.price / item.square, 2).ToString(),
                    Deadline = item.completion_date.ToString(),
                    SquareFootage = item.square.ToString(),
                    Type = item.rooms.ToString()
                });
            }
            return apartaments;
        }
    }
}
