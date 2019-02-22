using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium.Chrome;


namespace Wally.Tour {
    public class TourGuide {
        private readonly InternetConnectionChecker _internetConnectionChecker;
        private readonly TabLoader _tabLoader;
        private readonly PageInitializer _pageInitializer;
        private readonly PingPlotterWindowStateChanger _pingPlotterWindowStateChanger;
        private readonly TabSwitcher _tabSwitcher;
        private readonly IDictionary<string, Tab> _tabs = new Dictionary<string, Tab>();

        public TourGuide(InternetConnectionChecker internetConnectionChecker, TabLoader tabLoader, PageInitializer pageInitializer, PingPlotterWindowStateChanger pingPlotterWindowStateChanger, TabSwitcher tabSwitcher) {
            _internetConnectionChecker = internetConnectionChecker;
            _tabLoader = tabLoader;
            _pageInitializer = pageInitializer;
            _pingPlotterWindowStateChanger = pingPlotterWindowStateChanger;
            _tabSwitcher = tabSwitcher;
        }

        public delegate void OnLoadingPage(int pageNumber, Page page);
        public delegate void OnShowingPage(int pageNumber, Page page);
        public delegate void OnInitializingPage(int pageNumber, Page page);
        public delegate void OnExpired(Page page);
        public delegate void OnError(Page page, Exception exception);

        public void Guide(IReadOnlyCollection<Page> pages, ChromeDriver driver, OnLoadingPage onLoadingPage, OnShowingPage onShowingPage, OnInitializingPage onInitializingPage, OnExpired onExpired, OnError onError) {
            _tabSwitcher.Initialize(pages, _tabs);
            while (true) {
                Thread.Sleep(250); // just in case the while loop ever gets out of control...
                foreach (var item in pages.Select((pageValue, pageIndex) => new {pageIndex, pageValue})) {
                    var page = item.pageValue;
                    var index = item.pageIndex;
                    if (page.IsExpired) {
                        onExpired(page);
                    }
                    else {
                        try {
                            if (_internetConnectionChecker.InternetConnectionIsAvailable()) {
                                if (_tabs.TryGetValue(page.Url, out var tab)) {
                                    driver.SwitchTo().Window(tab.WindowHandle);
                                    if (tab.ShouldBeReloaded) {
                                        onLoadingPage(index + 1, page);
                                        _tabLoader.Load(tab, page);
                                    } else {
                                        onShowingPage(index + 1, page);
                                    }
                                } else {
                                    var tabInitializer = index == 0 ? (Func<Page, Tab>)_pageInitializer.InitializeInCurrentTab : _pageInitializer.InitializeInNewTab;
                                    onInitializingPage(index + 1, page);
                                    _tabs[page.Url] = tabInitializer(page);
                                    continue;
                                }
                            } else {
                                _pingPlotterWindowStateChanger.Maximize();
                            }
                        }
                        catch (Exception exception) {
                            onError(page, exception);
                        }
                        int time = 0;
                        if (page.Url.Contains("radar"))
                        {
                            time = 40;
                        } else
                        {
                            time = 10;
                        }

                        Console.WriteLine($"{DateTime.Now}: Delay time is {time}");
                        _tabSwitcher.WaitAtLeast(TimeSpan.FromSeconds(time));
                        
                        //tab switch timer controller
                                               
                    }
                }
            }
// ReSharper disable once FunctionNeverReturns
        }
    }
}