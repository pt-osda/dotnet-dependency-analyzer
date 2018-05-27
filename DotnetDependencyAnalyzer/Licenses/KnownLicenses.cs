using System.Collections.Generic;

namespace DotnetDependencyAnalyzer.Licenses
{
    public class KnownLicenses
    {
        public static Dictionary<string, string> licensesUrl = new Dictionary<string, string>() {
            {"http://www.apache.org/licenses/LICENSE-1.1" ,"Apache-1.1" },
            {"http://www.apache.org/licenses/LICENSE-2.0" ,"Apache-2.0" },
            {"https://opensource.org/licenses/BSD-2-Clause", "BSD-2-Clause" },
            {"https://opensource.org/licenses/BSD-3-Clause", "BSD-3-Clause" },
            {"http://repository.jboss.org/licenses/cc0-1.0.txt", "CC0-1.0"},
            {"https://www.eclipse.org/legal/cpl-v10.html", "CPL-1.0"},
            {"https://www.eclipse.org/legal/epl-v10.html", "EPL-1.0"},
            {"https://www.eclipse.org/org/documents/epl-2.0/EPL-2.0.txt", "EPL-2.0"},
            {"https://www.gnu.org/licenses/gpl-1.0", "GPL-1.0"},
            {"https://www.gnu.org/licenses/gpl-2.0", "GPL-2.0"},
            {"https://www.gnu.org/licenses/gpl-3.0", "GPL-3.0"},
            {"https://www.gnu.org/licenses/lgpl-2.1", "LGPL-2.1"},
            {"https://www.gnu.org/licenses/lgpl-3.0", "LGPL-3.0"},
            {"https://opensource.org/licenses/MIT", "MIT"},
            {"https://www.mozilla.org/en-US/MPL/1.1", "MPL-1.1"},
            {"https://www.mozilla.org/en-US/MPL/2.0", "MPL-2.0"},
            {"https://www.microsoft.com/web/webpi/eula/net_library_eula_ENU.htm", "MICROSOFT SOFTWARE LICENSE" }
        };
    }
}
