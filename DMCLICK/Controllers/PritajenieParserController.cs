using DMCLICK.Controllers.SeleniumControllers;
using DMCLICK.Entityes;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMCLICK.Controllers
{
    class PritajenieParserController
    {
        public List<Apartament> ParcePritajenie(ChromeDriver driver)
        {
            driver.SwitchTo().Window(SeleniumController.BaseWindow);
            driver.Navigate().GoToUrl("https://притяжение-екб.рф/kvartiry/?page=3000");
            foreach (var item in driver.WindowHandles)
            {
                if (item != SeleniumController.BaseWindow)
                {
                    driver.SwitchTo().Window(item);
                }
            }
            var flats = driver.FindElements(By.ClassName("parametric-results__loader"));
            List<Apartament> Apartaments = new List<Apartament>();

            for (int i = 0; i < flats.Count; i += 7)
            {
                string name = "0";
                if (flats[i + 1].Text.Contains("Трехкомнатная квартира"))
                {
                    name = "3";
                }
                if (flats[i + 1].Text.Contains("Двухкомнатная квартира"))
                {
                    name = "2";
                }
                if (flats[i + 6].Text.AsEnumerable().Any(ch => char.IsLetter(ch)))
                {
                    Apartaments.Add(new Apartament
                    {
                        Type = name,
                        SquareFootage = flats[i + 3].Text,
                        Deadline = flats[i + 5].Text,
                        Cost = "",
                        CostFotM2 = ""
                    });
                }
                else
                {
                    Apartaments.Add(new Apartament
                    {
                        Type = name,
                        SquareFootage = flats[i + 3].Text,
                        Deadline = flats[i + 5].Text,
                        Cost = flats[i + 6].Text.Replace(" ", ""),
                        CostFotM2 = Math.Round(Convert.ToDouble(flats[i + 6].Text.Replace(" ", "")) / Convert.ToDouble(flats[i + 3].Text.Replace('.', ',')), 2).ToString()
                    });
                }
            }
            return Apartaments;
        }
    }
}
