using DMCLICK.Controllers.ExcelControllers;
using DMCLICK.Entityes;
using DMCLICK.StaticFiles;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DMCLICK.Controllers.NewtonparkControllers
{
    class NewtonparkController
    {
        public List<string> UnopenedLinks { get; set; } = new List<string>();
        public Apartament GetApartament()
        {
            string deadline = "null";
            string Type = "null";
            string Cost = "null";
            string SquareFootage = "null";
            string CostFotM2 = "null";

            var r_flat_info = driver.FindElement(By.ClassName("r-flat-info"));

            var r_flat_name = driver.FindElement(By.ClassName("r-flat-name"));

            Type = new string(r_flat_name.Text.Where(c => char.IsLetter(c) || char.IsWhiteSpace(c)).ToArray());
            if (new string(Type.ToArray()).Last() == 'м' || new string(Type.ToArray()).Last() == 'М' || new string(Type.ToArray()).Last() == 'm' || new string(Type.ToArray()).Last() == 'M')
            {
                Type = Type.Remove(Type.Length - 1, 1);
            }

            SquareFootage = new string(r_flat_name.Text.Where(c => (char.IsPunctuation(c) || char.IsNumber(c)) && c != '-').ToArray());
            if (SquareFootage.ToArray().First() == ',')
            {
                SquareFootage = SquareFootage.Remove(0, 1);
            }
            if (SquareFootage.ToArray().Last() == '2' || SquareFootage.ToArray().Last() == '²')
            {
                SquareFootage = SquareFootage.Remove(SquareFootage.Length - 1, 1);
            }

            Cost = driver.FindElement(By.ClassName("fpsd__current__item__value")).Text;
            if (Cost.ToArray().Last() == '₽')
            {
                Cost = Cost.Remove(Cost.Length - 1, 1);
            }
            Cost = new string(Cost.ToArray().Where(el => !char.IsWhiteSpace(el)).ToArray());

            deadline = r_flat_info.FindElements(By.TagName("p")).FirstOrDefault(el => el.GetAttribute("itemprop") == null).Text;

            CostFotM2 = Math.Round(Convert.ToDouble(Cost) / Convert.ToDouble(SquareFootage), 2).ToString();

            if (Type.Contains("одно"))
            {
                Type = "1";
            }

            if (Type.Contains("двум"))
            {
                Type = "2";
            }

            if (Type.Contains("трем"))
            {
                Type = "3";
            }

            if (Type.Contains("студи"))
            {
                Type = "Студия";
            }

            return new Apartament { Cost = Cost, Deadline = deadline, SquareFootage = SquareFootage, Type = Type, CostFotM2 = CostFotM2 };
        }
        public void SaveData(List<Apartament> apartaments)
        {
            ExcelController excelController = new ExcelController();
            excelController.WriteDocumet(apartaments, "NewtonPark");
        }
        public List<Apartament> GetApartamentFromPage(string path)
        {
            List<Apartament> apartaments = new List<Apartament>();
            ReadOnlyCollection<IWebElement> r_select_list_inner_options = driver.FindElements(By.ClassName("r-select__list__inner__option"));
            if (r_select_list_inner_options.Count > 1)
            {
                for (int i = 1; i < r_select_list_inner_options.Count; i++)
                {
                    try
                    {
                        var apartament = GetApartament();

                        driver.FindElement(By.ClassName("r-select__current")).Click();
                        Thread.Sleep(200);
                        r_select_list_inner_options = driver.FindElements(By.ClassName("r-select__list__inner__option"));

                        IJavaScriptExecutor javascriptExecutor = (IJavaScriptExecutor)driver;
                        javascriptExecutor.ExecuteScript("arguments[0].click();", r_select_list_inner_options[i]);

                        apartaments.Add(apartament);
                    }
                    catch (Exception e)
                    {
                        UnopenedLinks.Add(path);
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else
            {
                apartaments.Add(GetApartament());
            }
            return apartaments;
        }
        public NewtonparkController(ChromeDriver _driver = null)
        {
            if (_driver == null)
            {
                ChromeOptions options = new ChromeOptions
                {
                    BinaryLocation = @"C:\Program Files\Google\Chrome Beta\Application\chrome.exe"
                };
                options.AddArgument("--window-size=1500,920");

                this.driver = new ChromeDriver(options);
            }
            else
            {
                this.driver = _driver;
            }
        }

        private ChromeDriver driver { get; set; }
        public string NewtonparkUrl { get { return ComplexUrls.complices.FirstOrDefault(el => el.ComplexName == "Ньютон парк").BaseHref; } }

        public void ParceNewtonPark()
        {
            List<Apartament> AllApartaments = new List<Apartament>();
            try
            {
                driver.Navigate().GoToUrl(NewtonparkUrl);
            }
            catch (Exception e)
            {

            }

            List<string> hrefs = new List<string>();
            ReadOnlyCollection<IWebElement> flat_preview_cards = driver.FindElements(By.ClassName("flat-preview-card"));

            foreach (var flat_preview_card in flat_preview_cards)
            {
                hrefs.Add(flat_preview_card.FindElement(By.TagName("a")).GetAttribute("href"));
            }

            foreach (var href in hrefs)
            {
                driver.Navigate().GoToUrl(href);
                Thread.Sleep(2000);
                var apartaments = GetApartamentFromPage(href);
                foreach (var apartament in apartaments)
                {
                    AllApartaments.Add(apartament);
                }
            }

            SaveData(AllApartaments);
        }
    }
}
