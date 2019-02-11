using System;

namespace Wally.Tour {
    public class Tab {
        private DateTime _lastShown = DateTime.MinValue;
        public string WindowHandle { get; }

        public Tab(string windowHandle) {
            WindowHandle = windowHandle;
        }
        public void UpdateLastShownMoment() {
            _lastShown = DateTime.Now;
        }
        public bool ShouldBeReloaded => DateTime.Now >= _lastShown.AddMinutes(10);
    }
}