using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace IKVM.JdkUtil.Providers
{

    class EnvProvider : JdkProvider
    {

        public override IEnumerable<string> Scan()
        {
            if (Environment.GetEnvironmentVariable("JDK_HOME") is string jdkHome)
                if (Directory.Exists(jdkHome))
                    yield return jdkHome;

            if (Environment.GetEnvironmentVariable("JAVA_HOME") is string javaHome)
                if (Directory.Exists(javaHome))
                    yield return javaHome;

            foreach (var i in ScanPath())
                yield return i;
        }

        /// <summary>
        /// Scans the PATH environmental variable.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> ScanPath()
        {
            var path = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrWhiteSpace(path))
                yield break;

            foreach (var dir in path.Split(Path.PathSeparator))
                if (ScanPathElement(dir) is string p)
                    yield return p;
        }

        /// <summary>
        /// Checks a PATH element for java.exe, and if so, returns the parent.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        string? ScanPathElement(string dir)
        {
            var javaExe = "java";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                javaExe += ".exe";

            if (File.Exists(Path.Combine(dir, javaExe)) == false)
                return null;

            var jdk = Path.GetDirectoryName(dir);
            if (Jdk.LooksLikeJdkHome(jdk) == false)
                return null;

            return jdk;
        }

    }

}
