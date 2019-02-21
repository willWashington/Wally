using System;
using OpenQA.Selenium;

namespace Wally.Tour {
    public class Page {
        private const int DefaultDisplayDurationInSeconds = 600;

        public Page(string voiceCommandWord, string url, Func<IWebDriver, Action> driverAction = null, int? secondsToDisplayAfterAction = null, DateTime? expiration = null) {
            VoiceCommandWord = voiceCommandWord;
            Url = url;
            DriverAction = driverAction;
            Expiration = expiration;
            SecondsToDisplayAfterAction = secondsToDisplayAfterAction ?? DefaultDisplayDurationInSeconds + 300;
        }

        public int SecondsToDisplayAfterAction { get; }
        public string VoiceCommandWord { get; }
        public string Url { get; }
        public Func<IWebDriver, Action> DriverAction { get; }
        public DateTime? Expiration { get; }
        public bool IsExpired => DateTime.Now > (Expiration ?? DateTime.Now.AddMinutes(1));
    }
}