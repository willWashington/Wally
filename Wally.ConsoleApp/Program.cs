using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Wally.Tour;

namespace Wally.ConsoleApp {
    internal static class Program {
        private static readonly IReadOnlyCollection<Page> Pages = new List<Page> {
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
            new Page("weather", "https://www.wunderground.com/weather/us/tn/memphis", driver => {
                var numberOfSecondsToWaitForPageToLoad = 5;
                Thread.Sleep(TimeSpan.FromSeconds(numberOfSecondsToWaitForPageToLoad));
                var element = driver.FindElement(By.CssSelector("#inner-content > div.city-body > div.row.current-forecast > div > div.row.city-forecast > div > div > city-today-forecast > div > div.small-12.medium-12.large-3.columns.alert-signup-wrap"));
                ((IJavaScriptExecutor) driver).ExecuteScript("arguments[0].parentNode.removeChild(arguments[0]);", element);
                element = driver.FindElement(By.CssSelector(@"#inner-content > div.city-body > div.row.current-forecast > div > div.row.city-forecast > div"));
                ((IJavaScriptExecutor) driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
                return null;
            })
            //, new Page("https://walls.io/y3bsc", driver => { return null; })
            //, new Page("https://howmanydaystill.com/its/quinns-birthday", expiration: new DateTime(2018, 7, 1))
            //, new Page("https://howmanydaystill.com/its/baby-beckham", expiration: new DateTime(2018, 11, 23), driverAction: driver => {
            //    ((IJavaScriptExecutor) driver).ExecuteScript(@"var img = document.createElement('img');img.style='position:absolute;right:0;bottom:0;width:219px;height:182px;';img.src='data:image/png;base64,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';document.body.appendChild(img);");
            //    return null;
            //})
            , new Page("quota", "https://customer.xfinity.com/#/services/internet#usage", driver => {
                var numberOfSecondsToWaitForPageToLoad = 15;
                Thread.Sleep(TimeSpan.FromSeconds(numberOfSecondsToWaitForPageToLoad));
                //var element = driver.FindElement(By.CssSelector(@"#bar-chart"));
                //((IJavaScriptExecutor) driver).ExecuteScript("arguments[0].scrollIntoView(true);window.scrollBy(0,-240);", element);
                var now = DateTime.Now;
                if (now.Day >= 5) {
                    var monthProgress = decimal.Divide(now.Day, DateTime.DaysInMonth(now.Year, now.Month));
                    var selectorToFindUsageDiv = @"#usage > div > div:nth-child(1) > div > div > div > p > span > b:nth-child(1)";
                    var selectorToFindGraphDiv = @"#usage > div > div:nth-child(2) > div";
                    var by = By.CssSelector(selectorToFindUsageDiv);
                    var element = driver.FindElement(by);
                    var gigabytesRemaining = Convert.ToInt32(((IJavaScriptExecutor) driver).ExecuteScript(@"return parseInt(arguments[0].innerHTML.match(/\d/g).join(''));", element));
                    var quotaProgress = 1 - decimal.Divide(gigabytesRemaining, 1024);
                    var warningProgress = monthProgress * .91m;
                    if (quotaProgress >= warningProgress) {
                        element = driver.FindElement(By.CssSelector(selectorToFindGraphDiv));
                        ((IJavaScriptExecutor) driver).ExecuteScript(@"var n=document.createElement('div');n.style.position='absolute';n.style.right=0;n.style.marginRight='29px';n.style.border='4px solid red';n.style.top='34%';n.style.backgroundColor='lightblue';n.style.fontSize='96pt';n.style.lineHeight=1.0;n.style.fontWeight='bold';n.innerHTML='!!!';n.style.zIndex=1000;console.log(n);arguments[0].prepend(n);", element);
                    }
                }

                return null;
            }),
            new Page("radar", "https://www.msn.com/en-us/weather/fullscreenmaps"
                , driver => { Thread.Sleep(2000);
                    return null;
                })
            // , new Page("https://uptimerobot.com/dashboard#tvMode")
        };

        private static ConsoleEventDelegate _handler;
        private static readonly int CtrlCloseEvent = 2;

        private static TourGuide _tourGuide;
        private static ChromeDriver _driver;

        private static void Main() {
            _handler = ConsoleEventCallback;
            SetConsoleCtrlHandler(_handler, true);
            var chromeDriverTerminator = new ChromeDriverTerminator();
            var chromeDriverCreator = new ChromeDriverCreator();
            var internetConnectionChecker = new InternetConnectionChecker();
            SpeechRecognitionEngine speechRecognitionEngine = new SpeechRecognitionEngine(SpeechRecognitionEngine.InstalledRecognizers().First(x => x.Culture.ToString() == "en-US"));
            var voiceListener = new VoiceListener(speechRecognitionEngine);
            chromeDriverTerminator.TerminateAll();
            _driver = chromeDriverCreator.Create(ConfigurationManager.AppSettings["ChromeUserDataDirectory"]);
            var pingPlotterWindowStateChanger = new PingPlotterWindowStateChanger(new WindowStateChanger());
            var tabLoader = new TabLoader(_driver, pingPlotterWindowStateChanger);
            var pageInitializer = new PageInitializer(_driver);
            var tabSwitcher = new TabSwitcher(voiceListener, _driver);
            _tourGuide = new TourGuide(internetConnectionChecker, tabLoader, pageInitializer, pingPlotterWindowStateChanger, tabSwitcher);
            var pages = Pages.OrderBy(x => "https://reg.usps.com/entreg/LoginAction_input?app=Phoenix&appURL=https://informeddelivery.usps.com".Equals(x.Url) ? 0 : 1).ToList();
            try {
                _tourGuide.Guide(pages, _driver, OnLoadingPage, OnShowingPage, OnInitializingPage, OnExpired, OnError);

                void OnInitializingPage(int pageNumber, Page page) {
                    OnVerbingPage(pageNumber, page, "Initializing");
                }

                void OnLoadingPage(int pageNumber, Page page) {
                    OnVerbingPage(pageNumber, page, "Loading");
                }

                void OnShowingPage(int pageNumber, Page page) {
                    OnVerbingPage(pageNumber, page, "Showing");
                }

                void OnVerbingPage(int pageNumber, Page page, string verb) {
                    Console.WriteLine($"{DateTime.Now}: {verb} ({pageNumber}/{pages.Count}): '{page.Url}'");
                }

                void OnExpired(Page page) {
                    Console.WriteLine($"{DateTime.Now}: Expired: '{page.Url}'");
                }

                void OnError(Page page, Exception exception) {
                    Console.WriteLine($"ERROR - {DateTime.Now}: {exception}");
                    Thread.Sleep(5000);
                }
            }
            catch (Exception e) {
                Console.WriteLine($"Program {DateTime.Now}: {e}");
                Thread.Sleep(20000);
                ConsoleEventCallback(CtrlCloseEvent);
                Environment.Exit(1);
            }
        }

        private static bool ConsoleEventCallback(int eventType) {
            if (eventType == CtrlCloseEvent) {
                _driver?.Quit();
            }

            return false;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
        private delegate bool ConsoleEventDelegate(int eventType);
    }
}