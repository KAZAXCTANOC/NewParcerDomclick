using DMCLICK.Controllers;
using DMCLICK.Controllers.ExcelControllers;
using DMCLICK.Controllers.SeleniumControllers;
using DMCLICK.Entityes;
using DMCLICK.StaticFiles;
using DMCLICK.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DMCLICK
{
    class Program
    {
        #region launch console and UI

        public static Application WinApp { get; private set; }
        public static Window MainWindow { get; private set; }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole(); // Create console window

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow(); // Get console window handle

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void ShowConsole()
        {
            var handle = GetConsoleWindow();
            if (handle == IntPtr.Zero)
                AllocConsole();
            else
                ShowWindow(handle, SW_SHOW);
        }

        static void HideConsole()
        {
            var handle = GetConsoleWindow();
            if (handle != null)
                ShowWindow(handle, SW_HIDE);
        }
        static void InitializeWindows()
        {
            WinApp = new Application();
            WinApp.Run(MainWindow = new MainWindow()); // note: blocking call
        }
        #endregion

        private static ExcelController excelController { get; set; } = new ExcelController();
        private static SeleniumController SeleniumController { get; set; } = new SeleniumController();

        [STAThread]
        static void Main(string[] args)
        {
            ShowConsole(); // Show the console window (for Win App projects)
            Console.WriteLine("Opening window...");
            InitializeWindows(); // opens the WPF window and waits here
            Console.WriteLine("Exiting main...");


            #region MyRegion

            try
            {
                bool isFirst = true;
                bool parsing = true;
                SeleniumController.LoadStartPage();

                foreach (var complex in ComplexUrls.complices)
                {
                    if (complex.ComplexName == "Притяжение ЕКБ")
                    {
                        PritajenieParserController pritajenieParserController = new PritajenieParserController();
                        excelController.WriteDocumet(pritajenieParserController.ParcePritajenie(SeleniumController.driver), complex.ComplexName);
                    }
                    else
                    {
                        SeleniumController.Adress = complex.BaseHref;
                        SeleniumController.GoToTheNexComplex(isFirst);
                        SeleniumController.CurrentPage = 0;
                        isFirst = false;
                        parsing = true;

                        SeleniumController.CountePages = (SeleniumController.GetCountPages() - 1) * 10;

                        while (parsing)
                        {

                            SeleniumController.OpenApartamentsInThisPage();

                            SeleniumController.LoadApartamentsFromPages();
                            SeleniumController.CloseApartamentsInThisPage();
                            parsing = SeleniumController.GoToTheTextPage();
                        }

                        excelController.WriteDocumet(SeleniumController._apartaments, complex.ComplexName);
                        complex.Apartaments = SeleniumController._apartaments;
                    }
                }
            }
            catch (Exception e)
            {
                foreach (Process proc in Process.GetProcessesByName("chromedriver.exe"))
                {
                    proc.Kill();
                }
                Console.WriteLine(e.Message);
            }

            //excelController.WriteDocumets(complices);

            SeleniumController.driver.Quit();
            #endregion

            Console.ReadKey();
        }
    }
}
