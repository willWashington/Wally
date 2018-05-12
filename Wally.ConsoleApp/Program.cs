using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Wally.Tour;

namespace Wally.ConsoleApp
{
    static class Program
    {
        private static readonly IReadOnlyCollection<Page> Pages = new List<Page> {
            new Page("https://informeddelivery.usps.com/box/pages/secure/DashboardAction_input.action", driver => {
                var numberOfSecondsToWaitForPageToLoad = 15;
                Thread.Sleep(TimeSpan.FromSeconds(numberOfSecondsToWaitForPageToLoad));
                var element = driver.FindElement(By.CssSelector("#pkgtab > a"));
                ((IJavaScriptExecutor)driver).ExecuteScript("$(arguments[0]).click();", element);
                element = driver.FindElement(By.ClassName("navcontainer"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].parentNode.removeChild(arguments[0]);", element);
                element = driver.FindElement(By.ClassName("packageContainer"));
                ((IJavaScriptExecutor) driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
                return null;
            })
            //, new Page("https://www.icloud.com/#calendar", driver => {
            //    //void ShowAdditionalDays() {
            //    //    var numberOfAdditionalDaysToShow = 6;
            //    //    var daysDisplayTimeInSeconds = Page.DefaultDisplayDurationInSeconds / numberOfAdditionalDaysToShow;
            //    //    Thread.Sleep(TimeSpan.FromSeconds(daysDisplayTimeInSeconds));
            //    //    driver.SwitchTo().Frame(driver.FindElement(By.Name("calendar")));
            //    //    var element = driver.FindElement(By.CssSelector(@"#sc2702 > div"));
            //    //    for (var i = 0; i < numberOfAdditionalDaysToShow; i++) {
            //    //        element.Click();
            //    //        Thread.Sleep(TimeSpan.FromSeconds(daysDisplayTimeInSeconds));
            //    //    }
            //    //}
            //    var numberOfSecondsToWaitForPageToLoad = 5;
            //    Thread.Sleep(TimeSpan.FromSeconds(numberOfSecondsToWaitForPageToLoad));
            //    return null; // ShowAdditionalDays;
            //})
            , new Page("https://www.wunderground.com/weather/us/tn/memphis", driver => {
                var numberOfSecondsToWaitForPageToLoad = 5;
                Thread.Sleep(TimeSpan.FromSeconds(numberOfSecondsToWaitForPageToLoad));
                var element = driver.FindElement(By.CssSelector("#inner-content > div.city-body > div.row.current-forecast > div > div.row.city-forecast > div > div > city-today-forecast > div > div.small-12.medium-12.large-3.columns.alert-signup-wrap"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].parentNode.removeChild(arguments[0]);", element);
                element = driver.FindElement(By.CssSelector(@"#inner-content > div.city-body > div.row.current-forecast > div > div.row.city-forecast > div"));
                ((IJavaScriptExecutor) driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
                return null;
            })
            //, new Page("https://redditp.com/r/landscapeporn", driver => {
            //    var titleDiv = driver.FindElement(By.Id("titleDiv"));
            //    var subredditLink = titleDiv.FindElement(By.TagName("a"));
            //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].style.visibility='hidden'", subredditLink);
            //    var controlsDiv = driver.FindElement(By.Id("controlsDiv"));
            //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].style.visibility='hidden'", controlsDiv);
            //    return null;
            //}, 105)
            , new Page("https://howmanydaystill.com/its/disney-vacation-17")
            , new Page("https://customer.xfinity.com/#/services/internet", driver => {
                var numberOfSecondsToWaitForPageToLoad = 10;
                Thread.Sleep(TimeSpan.FromSeconds(numberOfSecondsToWaitForPageToLoad));
                var element = driver.FindElement(By.CssSelector(@"#page-view > section > div > div > div:nth-child(6) > div"));
                ((IJavaScriptExecutor) driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
                return null;
            })
            , new Page("https://www.msn.com/en-us/weather/fullscreenmaps")
            , new Page("https://uptimerobot.com/dashboard#tvMode")
        };

        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            _handler = ConsoleEventCallback;
            SetConsoleCtrlHandler(_handler, true);
            try {
                foreach (var process in Process.GetProcessesByName("chromedriver"))
                {
                    process.Kill();
                }

                var internetConnectionChecker = new InternetConnectionChecker();
                var windowStateChanger = new WindowStateChanger();
                var tourGuide = new TourGuide(internetConnectionChecker, windowStateChanger);
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArguments(new List<string> { "disable-infobars", "--disable-session-crashed-bubble", "--start-fullscreen", $@"--user-data-dir={ConfigurationManager.AppSettings["ChromeUserDataDirectory"]}" });
                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                _driver = new ChromeDriver(service, chromeOptions);
                var pages = Pages.OrderBy(x => "https://customer.xfinity.com/#/services/internet".Equals(x.Url) ? 0 : 1).ToList();
                tourGuide.Guide(pages, _driver);
            }
            catch (Exception) {
                ConsoleEventCallback(CtrlCloseEvent);
                Environment.Exit(1);
            }
        }

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == CtrlCloseEvent)
            {
                _driver?.Quit();
                _driver?.Quit();
            }
            return false;
        }
        static ConsoleEventDelegate _handler;
        private static int CtrlCloseEvent = 2;

        private static ChromeDriver _driver;

        // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

    }
}
