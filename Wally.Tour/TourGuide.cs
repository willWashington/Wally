using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium.Chrome;

namespace Wally.Tour
{
    public class TourGuide {
        readonly InternetConnectionChecker _internetConnectionChecker;
        readonly WindowStateChanger _windowStateChanger;
        static readonly int SecondsBetweenNetworkChecks = 3;

        public TourGuide(InternetConnectionChecker internetConnectionChecker, WindowStateChanger windowStateChanger) {
            _internetConnectionChecker = internetConnectionChecker;
            _windowStateChanger = windowStateChanger;
        }

        public void Guide(IReadOnlyCollection<Page> pages, ChromeDriver driver) {
            while (DateTime.Now < DateTime.MaxValue) {
                foreach (var page in pages)
                {
                    try {
                        if (_internetConnectionChecker.InternetConnectionIsAvailable()) {
                            _windowStateChanger.ShowMaximized("PingPlotter");
                            var pingPlotterDisplayMoment = DateTime.Now;
                            Console.WriteLine($"{DateTime.Now}: Showing '{page.Url}'");
                            driver.Navigate().GoToUrl(page.Url);
                            var driverAction = page.DriverAction?.Invoke(driver);
                            while (TimeSpan.FromSeconds(5) > DateTime.Now - pingPlotterDisplayMoment) {
                                Thread.Sleep(TimeSpan.FromSeconds(1));
                            }

                            _windowStateChanger.ShowMinimized("PingPlotter");
                            driverAction?.Invoke();
                        }

                        for (var i = 0; i < page.SecondsToDisplayAfterAction / SecondsBetweenNetworkChecks; i++) {
                            Thread.Sleep(TimeSpan.FromSeconds(SecondsBetweenNetworkChecks));
                            if (!_internetConnectionChecker.InternetConnectionIsAvailable()) {
                                _windowStateChanger.ShowMaximized("PingPlotter");
                                break;
                            }
                        }
                    }
                    catch (Exception e) {
                        Console.WriteLine($"{DateTime.Now}: {e}");
                        _windowStateChanger.ShowMaximized("PingPlotter");
                        Thread.Sleep(5000);
                    }
                }
            }
        }
    }
}
