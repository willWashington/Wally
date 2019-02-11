using System;
using System.Linq;
using OpenQA.Selenium.Chrome;

namespace Wally.Tour {
    public class PageInitializer {
        private readonly ChromeDriver _driver;
        public PageInitializer(ChromeDriver driver) {
            _driver = driver;
        }

        private Tab Initialize(Page page, bool newTab) {
            string script;
            Func<string> windowHandleGetter;
            if (newTab) {
                var originalWindowHandles = _driver.WindowHandles.ToList();
                script = $"window.open('{page.Url}');";
                windowHandleGetter = () => _driver.WindowHandles.Except(originalWindowHandles).Single();
            } else {
                script = $"location.href = '{page.Url}';";
                windowHandleGetter = () => _driver.CurrentWindowHandle;
            }
            _driver.ExecuteScript(script);
            var windowHandle = windowHandleGetter();
            var result = new Tab(windowHandle);
            return result;

        }

        public Tab InitializeInCurrentTab(Page page) {
            return Initialize(page, false);
        }

        public Tab InitializeInNewTab(Page page) {
            return Initialize(page, true);
        }
    }
}