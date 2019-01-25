using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Wally.Tour {
    public class WindowStateChanger {
        private const int SwShowMinimized = 2;
        private const int SwShowMaximized = 3;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public void ShowMinimized(string processName) {
            var process = Process.GetProcessesByName(processName).FirstOrDefault(x => x.MainWindowHandle != IntPtr.Zero);
            Show(process, SwShowMinimized);
        }

        public void ShowMaximized(string processName) {
            var process = Process.GetProcessesByName(processName).FirstOrDefault(x => x.MainWindowHandle != IntPtr.Zero);
            Show(process, SwShowMaximized);
        }

        void Show(Process process, int command) {
            if (process != null) {
                ShowWindow(process.MainWindowHandle, command);
            }
        }
    }
}