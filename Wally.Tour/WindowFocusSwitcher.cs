using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Automation;

namespace Wally.Tour {
    public class WindowFocusSwitcher {
        public void FocusProcessWindow(string processName) {
            var process = Process.GetProcessesByName(processName).FirstOrDefault(x => x.MainWindowHandle != IntPtr.Zero);
            if (process != null) {
                var element = AutomationElement.FromHandle(process.MainWindowHandle);
                if (element != null) {
                    element.SetFocus();
                }
            }
        }
    }
}