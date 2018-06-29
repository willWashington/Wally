using System;
using OpenQA.Selenium;

namespace Wally.Tour {
    public class Page {
        public Page(string url, Func<IWebDriver, Action> driverAction = null, int? secondsToDisplayAfterAction = null, DateTime? expiration = null) {
            Url = url;
            DriverAction = driverAction;
            Expiration = expiration;
            SecondsToDisplayAfterAction = secondsToDisplayAfterAction ?? DefaultDisplayDurationInSeconds;
        }

        public int SecondsToDisplayAfterAction { get; }
        public string Url { get; }
        public Func<IWebDriver, Action> DriverAction { get; }
        public DateTime? Expiration { get; }
        const int DefaultDisplayDurationInSeconds = 30;
        public bool IsExpired => DateTime.Now > (Expiration ?? DateTime.Now.AddMinutes(1));
    }
}