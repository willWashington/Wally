using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium.Chrome;

namespace Wally.Tour {
    public class TabSwitcher {
        private readonly VoiceListener _voiceListener;
        private readonly ChromeDriver _driver;
        private DateTime _whenToNextAutomaticallySwitch = DateTime.Now;
        public TabSwitcher(VoiceListener voiceListener, ChromeDriver driver) {
            _voiceListener = voiceListener;
            _driver = driver;
        }

        public void Initialize(IReadOnlyCollection<Page> pages, IDictionary<string, Tab> tabs) {
            if (System.Windows.Forms.SystemInformation.TerminalServerSession) {
                Console.WriteLine("Terminal Server Session detected. Skipping voice parsing initialization.");
            } else {
                var pageVoiceCommandWords = pages.Select(x => x.VoiceCommandWord).ToList();
                _voiceListener.Listen(pageVoiceCommandWords, OnSpeechParsed);
                void OnSpeechParsed(string parsedSpeech) {
                    var url = pages.Single(x => x.VoiceCommandWord.Equals(parsedSpeech, StringComparison.InvariantCultureIgnoreCase)).Url;
                    Console.WriteLine($"parsedSpeech: '{parsedSpeech}'; url: ${url}");
                    if (tabs.ContainsKey(url)) {
                        _whenToNextAutomaticallySwitch = new DateTime(Math.Max(DateTime.Now.AddSeconds(30).Ticks, _whenToNextAutomaticallySwitch.Ticks));
                        _driver.SwitchTo().Window(tabs[url].WindowHandle);
                    } else {
                        Console.Beep();
                    }
                }
            }
        }

        public void WaitAtLeast(TimeSpan timeSpan) {
            _whenToNextAutomaticallySwitch = new DateTime(Math.Max(DateTime.Now.Add(timeSpan).Ticks, _whenToNextAutomaticallySwitch.Ticks));
            while (_whenToNextAutomaticallySwitch > DateTime.Now) {
                Thread.Sleep(TimeSpan.FromMilliseconds(250));
            }
        }
    }
}