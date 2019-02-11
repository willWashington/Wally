using System.Diagnostics;

namespace Wally.ConsoleApp {
    public class ChromeDriverTerminator {
        public void TerminateAll() {
            foreach (var process in Process.GetProcessesByName("chromedriver")) {
                process.Kill();
            }
        }
    }
}