using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace IKVM.JdkUtil.Providers
{

    class LinuxProvider : JdkProvider
    {

        readonly static string[] JDK_BASE_DIRS = [
            "/usr/lib/jvm", // Ubuntu
            "/usr/java", // Java 8 on CentOS?
        ];

        /// <inheritdoc />
        public override IEnumerable<string> Scan()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                foreach (var baseDir in JDK_BASE_DIRS)
                    if (Directory.Exists(baseDir))
                        foreach (var dir in Directory.GetDirectories(baseDir))
                            yield return dir;
        }

    }

}
