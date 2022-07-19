using DMCLICK.Entityes;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace DMCLICK.Controllers.SeleniumControllers
{
    public class SeleniumController
    {
        public List<Apartament> _apartaments { get; set; } = new List<Apartament>();
        public int CurrentPage { get; set; } = 0;
        public string Adress { get; set; } = "https://ekaterinburg.domclick.ru/search?deal_type=sale&category=living&offer_type=flat&offer_type=layout&complex_ids=60802&complex_name=ЖК%20Солнечный-3&from_developer=1&offset=";
        public int CountePages { get; set; }
        public static string BaseWindow { get; set; }

        private const string CardPrizeClassName = "flatSelection_cardPrice";
        private const string CardPrizeAreaClassName = "flatSelection_cardArea";
        private const string CardHeaderClassName = "flatSelection_cardHeader";
        private const string RowListDataClassName = "sc_flatInfoList_rowValue";
        private const string BuildingInfoDataTestIdNameData = "data-test-id";
        private const string BuildingInfoDataTestIdName = "constructionPeriod";
        private const string BuildingInfoClassName = "complexInfo_buildingInfoItem";
        private const string BareSqaureCostClassName = "shortSummary_bareSqaurePrice";
        private const string RowListClassName = "sc_flatInfoList_row";
        private const string Error = "Error";
        private const string Price = "shortSummary_barePrice";
        private const string CostSymmaryClassName = "shortSummary_barePriceWrapper";
        private const string SeveralApartmentsClassName = "flatSelection_title";
        private const string ApartamentClassName = "layoutSnippet_layout_href";
        private const string PageInMainWindowClassName = "mesVG";
        
        public void GoToTheNexComplex(bool isFirst)
        {
            if (!isFirst)
            {
                driver.SwitchTo().Window(BaseWindow);
            }
            driver.Navigate().GoToUrl(Adress);
            _apartaments = new List<Apartament>();
            Thread.Sleep(5000);
        }
        public bool GoToTheTextPage()
        {
            int nextPage = CurrentPage + 10;
            if (nextPage <= CountePages)
            {
                CurrentPage += 10;
                driver.SwitchTo().Window(BaseWindow);
                driver.Navigate().GoToUrl($"{Adress}{nextPage}");
                Thread.Sleep(3000);
                return true;
            }
            else
            {
                return false;
            }
        }
        private List<Apartament> LoadApartamentFromPage()
        {
            string Type = Error;
            string Cost = Error;
            string SquareFootage = Error;
            string Deadline = Error;
            string CostFotM2 = Error;

            bool isOneApartament = true;
            try
            {
                driver.FindElement(By.ClassName(SeveralApartmentsClassName));
                isOneApartament = false;
            }
            catch { }

            if (isOneApartament)
            {
                //Парсинг сроков строительства
                try
                {
                    Deadline = driver.FindElements(By.ClassName(BuildingInfoClassName)).FirstOrDefault(el => el.GetAttribute(BuildingInfoDataTestIdNameData) == BuildingInfoDataTestIdName).Text;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"При парсинге сроков строительства квартиры {driver.Url} произошла ошибка: \n{e.Message}");
                }

                //Парсин цены 
                try
                {
                    Cost = driver.FindElement(By.ClassName(CostSymmaryClassName)).FindElement(By.ClassName(Price)).Text.Replace("₽", "").Replace(" ", "");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"При парсинге цены на квартиру {driver.Url} произошла ошибка: \n{e.Message}");
                }

                //Парсинг цены за m^2
                try
                {
                    CostFotM2 = driver.FindElement(By.ClassName(BareSqaureCostClassName)).Text;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"При парсинге цены зацены за m^2 квартиры {driver.Url} произошла ошибка: \n{e.Message}");
                }

                //Парсинг Общей площади + Комнаты
                try
                {
                    var rows = driver.FindElements(By.ClassName(RowListClassName));
                    for (int i = 0; i < rows.Count; i++)
                    {
                        if (rows[i].Text.Contains("Общая площадь"))
                        {
                            SquareFootage = rows[i].FindElement(By.ClassName(RowListDataClassName)).Text;
                        }
                        if (rows[i].Text.Contains("Комнаты"))
                        {
                            Type = rows[i].FindElement(By.ClassName(RowListDataClassName)).Text;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"При общей площади + кол-во комнат квартиры {driver.Url} произошла ошибка: \n{e.Message}");
                }

                return new List<Apartament>() 
                { 
                    new Apartament()
                    {
                        Deadline = Deadline,
                        Cost = Cost,
                        CostFotM2 = CostFotM2,
                        SquareFootage = SquareFootage,
                        Type = Type
                    } 
                };
            }
            else
            {
                List<Apartament> apartaments = new List<Apartament>();

                //Паринг сроков строительства
                try
                {
                    //TODO перенести в константы
                    Deadline = driver.FindElements(By.ClassName("layoutInfo_buildingInfoItem")).FirstOrDefault(el => el.GetAttribute(BuildingInfoDataTestIdNameData) == BuildingInfoDataTestIdName).Text;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"При парсинге сроков строительства квартиры {driver.Url} произошла ошибка: \n{e.Message}");
                }

                //Парсинг Общей площади + Комнаты
                try
                {
                    var rows = driver.FindElements(By.ClassName(RowListClassName));
                    for (int i = 0; i < rows.Count; i++)
                    {
                        if (rows[i].Text.Contains("Общая площадь"))
                        {
                            SquareFootage = rows[i].FindElement(By.ClassName(RowListDataClassName)).Text;
                        }
                        if (rows[i].Text.Contains("Комнаты"))
                        {
                            Type = rows[i].FindElement(By.ClassName(RowListDataClassName)).Text;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"При общей площади + кол-во комнат квартиры {driver.Url} произошла ошибка: \n{e.Message}");
                }

                ReadOnlyCollection<IWebElement> ButtonsOnPage = null;
                try
                {
//                                                                          |
//                                                                          |
//                                  есть ли страницы с квартирами на сайте  |
//                                                                         \|/
                    ButtonsOnPage = driver.FindElements(By.ClassName("sc_pagination_button"));
                }
                catch { }
                if (ButtonsOnPage.Count() != 0) //да <------------------------------------------------------------------------------------
                {
                    foreach (var Button in ButtonsOnPage)
                    {
                        IJavaScriptExecutor javascriptExecutor = (IJavaScriptExecutor)driver;
                        javascriptExecutor.ExecuteScript("arguments[0].click();", Button);
                        //Парсинг цены + цены за m^2
                        try
                        {
                            var flatSelection_cardHeaders = driver.FindElements(By.ClassName(CardHeaderClassName));
                            foreach (var item in flatSelection_cardHeaders)
                            {
                                Cost = item.FindElement(By.ClassName(CardPrizeClassName)).Text.Replace("₽", "").Replace(" ", "");
                                CostFotM2 = item.FindElement(By.ClassName(CardPrizeAreaClassName)).Text.Replace("₽/м²", "").Replace(" ", "");
                                apartaments.Add(new Apartament()
                                {
                                    Deadline = Deadline,
                                    Cost = Cost,
                                    CostFotM2 = CostFotM2,
                                    SquareFootage = SquareFootage,
                                    Type = Type
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"При парсинге цены + цены за m^2 {driver.Url} произошла ошибка: \n{e.Message}");
                        }
                    }
                }            
                else //нет <-----------------------------------------------------------------------------------
                {
                    //Парсинг цены + цены за m^2
                    try
                    {
                        var flatSelection_cardHeaders = driver.FindElements(By.ClassName(CardHeaderClassName));
                        foreach (var item in flatSelection_cardHeaders)
                        {
                            Cost = item.FindElement(By.ClassName(CardPrizeClassName)).Text.Replace("₽", "").Replace(" ", "");
                            CostFotM2 = item.FindElement(By.ClassName(CardPrizeAreaClassName)).Text.Replace("₽/м²", "").Replace(" ", "");
                            apartaments.Add(new Apartament()
                            {
                                Deadline = Deadline,
                                Cost = Cost,
                                CostFotM2 = CostFotM2,
                                SquareFootage = SquareFootage,
                                Type = Type
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"При парсинге сроков строительства квартиры {driver.Url} произошла ошибка: \n{e.Message}");
                    }
                }

                return apartaments;
            }
        }
        public void LoadApartamentsFromPages()
        {
            List<Apartament> apartaments = new List<Apartament>();
            var ListPagesWithApartaments = GetAllPagesWithApartaments();
            foreach (var pagesWithApartaments in ListPagesWithApartaments)
            {
                driver.SwitchTo().Window(pagesWithApartaments);
                foreach (var item in LoadApartamentFromPage())
                {
                    apartaments.Add(item);
                }
            }
            foreach (var apartament in apartaments)
            {
                _apartaments.Add(apartament);
            }
        }
        public void OpenApartamentsInThisPage()
        {
            BaseWindow = driver.WindowHandles[0];

            ReadOnlyCollection<IWebElement> Apartaments = driver.FindElements(By.ClassName(ApartamentClassName));
            foreach (var Apartament in Apartaments)
            {
                Apartament.Click();
            }
            Thread.Sleep(5000);
        }
        public List<string> GetAllPagesWithApartaments()
        {
            List<string> ListPagesWithApartaments = new List<string>();
            for (int i = 0; i < driver.WindowHandles.Count; i++)
            {
                if (driver.WindowHandles[i] != BaseWindow)
                {
                    ListPagesWithApartaments.Add(driver.WindowHandles[i]);
                }
            }
            return ListPagesWithApartaments;
        }
        public void CloseApartamentsInThisPage()
        {
            var ListPagesWithApartaments = GetAllPagesWithApartaments();
            for (int i = 0; i < ListPagesWithApartaments.Count; i++)
            {
                if (ListPagesWithApartaments[i] != BaseWindow)
                {
                    driver.SwitchTo().Window(ListPagesWithApartaments[i]);
                    driver.Close();
                }
            }
        }
        public int GetCountPages()
        {
            var count = driver.FindElements(By.ClassName(PageInMainWindowClassName)).Last().Text;
            Console.WriteLine($"Найденное кол-во странниц {count} на странице {driver.Url}");
            return Convert.ToInt32(count);
        }
        public ChromeDriver driver { get; set; }
        public ChromeDriver LoadStartPage()
        {
            Console.WriteLine("Начало открытия драйвера");
            ChromeDriver driver;
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--disable-blink-features=AutomationControlled");
            //options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36");
            options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.5060.114 Safari/537.36");
            options.AddArgument("--remote-debugging-port=5552");
            options.AddArgument("--window-size=1500,920");
            options.AddArguments("--disable-infobars");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--ignore-ssl-errors");
            options.AddArgument("no-sandbox");

            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            driver = new ChromeDriver(service, options);

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript($"window.open('{Adress}{CurrentPage}')");

            ReadOnlyCollection<string> windowHandles = driver.WindowHandles;
            driver.SwitchTo().Window(windowHandles[0]);
            driver.Close();
            driver.SwitchTo().Window(windowHandles[1]);

            Thread.Sleep(5000);
            this.driver = driver;
            Console.WriteLine("Открытие драйвера успешно завершенно !!!");
            return driver;
        }
    }
}
