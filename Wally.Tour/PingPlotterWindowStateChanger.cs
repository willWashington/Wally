namespace Wally.Tour {
    public class PingPlotterWindowStateChanger {
        private static readonly string ProcessName = "PingPlotter";
        private readonly WindowStateChanger _windowStateChanger;

        public PingPlotterWindowStateChanger(WindowStateChanger windowStateChanger) {
            _windowStateChanger = windowStateChanger;
        }

        public void Minimize() {
            _windowStateChanger.ShowMinimized(ProcessName);
        }

        public void Maximize() {
            _windowStateChanger.ShowMaximized(ProcessName);
        }
    }
}