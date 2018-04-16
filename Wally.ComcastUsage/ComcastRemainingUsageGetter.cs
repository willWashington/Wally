using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium.Chrome;

namespace Wally.ComcastUsage
{
    public class ComcastRemainingUsageGetter
    {
        public int GetRemainingGigabytes() {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments(new List<string> { "--incognito" });
            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            var driver = new ChromeDriver(service, chromeOptions);
            driver.Navigate().GoToUrl("https://customer.xfinity.com/#/devices");
            var userInput = driver.FindElementById("user");
            var passwordInput = driver.FindElementById("passwd");
            var signinButton = driver.FindElementById("sign_in");
            userInput.SendKeys("<username>");
            passwordInput.SendKeys("<passwd>");
            signinButton.Click();
            Thread.Sleep(4000);
            var usageDescriptorElement = driver.FindElementByXPath(@"//*[@id=""page-view""]/section[3]/div/div/div[3]/div[2]/div[1]/div/div/div[1]/p/span/b[1]");
            var remainingGigabytesWithUnitsLabel = usageDescriptorElement.Text;
            var remainingGigabytes = remainingGigabytesWithUnitsLabel.Replace("GB", string.Empty);
            var result = Convert.ToInt32(remainingGigabytes);
            driver.Close();
            return result;
        }
    }
}
