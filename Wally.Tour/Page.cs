using System;
using OpenQA.Selenium;

namespace Wally.Tour {
    public class Page {
        public Page(string url, Func<IWebDriver, Action> driverAction = null, int? secondsToDisplayAfterAction = null) {
            Url = url;
            DriverAction = driverAction;
            SecondsToDisplayAfterAction = secondsToDisplayAfterAction ?? DefaultDisplayDurationInSeconds;
        }

        public int SecondsToDisplayAfterAction { get; }
        public string Url { get; }
        public Func<IWebDriver, Action> DriverAction { get; }
        public const int DefaultDisplayDurationInSeconds = 30;
    }
}