using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Wally.Tour {
    public class WindowStateChanger {
        private const int SwShowMinimized = 2;
        private const int SwShowMaximized = 3;

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetFocus(IntPtr hWnd);

        public void ShowMinimized(string processName) {
            var process = Process.GetProcessesByName(processName).FirstOrDefault(x => x.MainWindowHandle != IntPtr.Zero);
            Show(process, SwShowMinimized);
        }

        public void ShowMaximized(string processName) {
            var process = Process.GetProcessesByName(processName).FirstOrDefault(x => x.MainWindowHandle != IntPtr.Zero);
            Show(process, SwShowMaximized);
        }

        private void Show(Process process, int command) {
            if (process != null) {
                SetFocus(process.MainWindowHandle);
                ShowWindow(process.MainWindowHandle, command);
            }
        }
    }
}