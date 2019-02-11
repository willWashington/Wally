using System.Collections.Generic;
using OpenQA.Selenium.Chrome;

namespace Wally.ConsoleApp {
    public class ChromeDriverCreator {
        public ChromeDriver Create(string userDataDirectory) {
            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments(new List<string> {"--start-fullscreen", "disable-infobars", "--disable-session-crashed-bubble", $@"--user-data-dir={userDataDirectory}"});
            var result = new ChromeDriver(service, chromeOptions);
            return result;
        }
    }
}