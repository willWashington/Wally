using OpenQA.Selenium.Chrome;

namespace Wally.Tour {
    public class TabLoader {
        private readonly PingPlotterWindowStateChanger _pingPlotterWindowStateChanger;
        private readonly ChromeDriver _driver;
        public TabLoader(ChromeDriver driver, PingPlotterWindowStateChanger pingPlotterWindowStateChanger) {
            _driver = driver;
            _pingPlotterWindowStateChanger = pingPlotterWindowStateChanger;
        }

        public void Load(Tab tab, Page page) {
            _pingPlotterWindowStateChanger.Maximize();
            _driver.Navigate().GoToUrl(page.Url);
            _driver.Navigate().Refresh();
            var driverAction = page.DriverAction?.Invoke(_driver);
            driverAction?.Invoke();
            _pingPlotterWindowStateChanger.Minimize();
            tab.UpdateLastShownMoment();
        }
    }
}