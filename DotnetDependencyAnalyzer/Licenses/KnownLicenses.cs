using System.Collections.Generic;

namespace DotnetDependencyAnalyzer.Licenses
{
    public class KnownLicenses
    {
        public static List<string> licensesName = new List<string>()
        {
            "Apache Software License, Version 1.1",
            "Apache License, Version 2.0",
            "The 2-Clause BSD License",
            "The 3-Clause BSD License",
            "Creative Commons Legal Code",
            "COMMON DEVELOPMENT AND DISTRIBUTION LICENSE (CDDL), Version 1.0",
            "Common Public License - v 1.0",
            "Eclipse Public License - v 1.0",
            "Eclipse Public License - v 2.0",
            "GNU GENERAL PUBLIC LICENSE, Version 1",
            "GNU GENERAL PUBLIC LICENSE, Version 2",
            "GNU GENERAL PUBLIC LICENSE, Version 3",
            "GNU LESSER GENERAL PUBLIC LICENSE, Version 2.1",
            "GNU LESSER GENERAL PUBLIC LICENSE, Version 3",
            "MIT License",
            "Mozilla Public License Version 1.1",
            "Mozilla Public License, Version 2.0",
            "Microsoft .NET Library"
        };

        public static Dictionary<string, string> licensesUrl = new Dictionary<string, string>() {
            {"http://www.apache.org/licenses/LICENSE-1.1" ,"Apache Software License, Version 1.1" },
            {"http://www.apache.org/licenses/LICENSE-2.0" ,"Apache Software License, Version 2.0" },
            {"https://opensource.org/licenses/BSD-2-Clause", "The 2-Clause BSD License" },
            {"https://opensource.org/licenses/BSD-3-Clause", "The 3-Clause BSD License" },
            {"http://repository.jboss.org/licenses/cc0-1.0.txt", "Creative Commons Legal Code"},
            {"https://www.eclipse.org/legal/cpl-v10.html", "Common Public License - v 1.0"},
            {"https://www.eclipse.org/legal/epl-v10.html", "Eclipse Public License - v 1.0"},
            {"https://www.eclipse.org/org/documents/epl-2.0/EPL-2.0.txt", "Eclipse Public License - v 2.0"},
            {"https://www.gnu.org/licenses/gpl-1.0", "GNU GENERAL PUBLIC LICENSE, Version 1"},
            {"https://www.gnu.org/licenses/gpl-2.0", "GNU GENERAL PUBLIC LICENSE, Version 2"},
            {"https://www.gnu.org/licenses/gpl-3.0", "GNU GENERAL PUBLIC LICENSE, Version 3"},
            {"https://www.gnu.org/licenses/lgpl-2.1", "GNU LESSER GENERAL PUBLIC LICENSE, Version 2.1"},
            {"https://www.gnu.org/licenses/lgpl-3.0", "GNU LESSER GENERAL PUBLIC LICENSE, Version 3"},
            {"https://opensource.org/licenses/MIT", "MIT License"},
            {"https://www.mozilla.org/en-US/MPL/1.1", "Mozilla Public License Version 1.1"},
            {"https://www.mozilla.org/en-US/MPL/2.0", "Mozilla Public License, Version 2.0"},
            {"https://www.microsoft.com/net/dotnet_library_license.htm", "Microsoft .NET Library License" }
        };
    }
}
