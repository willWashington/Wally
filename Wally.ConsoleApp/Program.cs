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

            new Page("weather", "https://www.wunderground.com/weather/us/tn/knoxville", driver => {
                var numberOfSecondsToWaitForPageToLoad = 5;
                Thread.Sleep(TimeSpan.FromSeconds(numberOfSecondsToWaitForPageToLoad));
                var element = driver.FindElement(By.CssSelector("#inner-content > div.city-body > div.row.current-forecast > div > div.row.city-forecast > div > div > city-today-forecast > div > div.small-12.medium-12.large-3.columns.alert-signup-wrap"));
                ((IJavaScriptExecutor) driver).ExecuteScript("arguments[0].parentNode.removeChild(arguments[0]);", element);
                element = driver.FindElement(By.CssSelector(@"#body")); //not working
                ((IJavaScriptExecutor) driver).ExecuteScript("document.body.style.backgroundColor = #5a5a5a');", 0); //not working
                return null;
            })
            //, new Page("radar", "https://www.msn.com/en-us/weather/fullscreenmaps"
            //    , driver => { Thread.Sleep(2000);
            //        return null;
            //    })





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