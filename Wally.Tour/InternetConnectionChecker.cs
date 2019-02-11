using System.Net;

namespace Wally.Tour {
    public class InternetConnectionChecker {
        public bool InternetConnectionIsAvailable() {
            try {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204")) {
                    return true;
                }
            }
            catch {
                return false;
            }
        }
    }
}