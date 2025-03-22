using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using IKVM.JdkUtil.Providers;

namespace IKVM.JdkUtil
{

    /// <summary>
    /// Describes a JDK installation.
    /// </summary>
    /// <param name="Path"></param>
    /// <param name="Version"></param>
    public record class Jdk(string Path, JdkVersion Version)
    {

        static readonly JdkProvider[] PROVIDERS = [
            new WindowsProvider(),
            new LinuxProvider(),
            new MacOSProvider(),
            new HomeBrewProvider(),
            new EnvProvider(),
        ];

        /// <summary>
        /// Finds and reads JDK installations.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Jdk> Resolve()
        {
            return Resolve([], true, []);
        }

        /// <summary>
        /// Finds and reads JDK installations.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Jdk> Resolve(IEnumerable<string> before, bool includeDefault, IEnumerable<string> after)
        {
            var defaultPaths = includeDefault ? Scan() : [];
            foreach (var d in Normalize(before.Concat(defaultPaths).Concat(after)).Distinct())
                if (TryReadJdk(d, out var jdk) && jdk is not null)
                    yield return jdk;
        }

        /// <summary>
        /// Normalizes the path elements.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        static IEnumerable<string> Normalize(IEnumerable<string> paths)
        {
            foreach (var path in paths)
                yield return Normalize(path);
        }

        /// <summary>
        /// Normalizes the path element, removing trailing slashes.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static string Normalize(string path)
        {
            path = System.IO.Path.GetFullPath(path);
            if (path.EndsWith(System.IO.Path.DirectorySeparatorChar) == false)
                path += System.IO.Path.DirectorySeparatorChar;

            return path;
        }

        /// <summary>
        /// Scans for the possible JDK directories.
        /// </summary>
        /// <returns></returns>
        static IEnumerable<string> Scan()
        {
            foreach (var p in PROVIDERS)
                foreach (var d in p.Scan())
                    if (Directory.Exists(d))
                        yield return System.IO.Path.GetFullPath(d);
        }

        /// <summary>
        /// Attempts to read the JDK in the specific path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="jdk"></param>
        /// <returns></returns>
        static bool TryReadJdk(string path, out Jdk? jdk)
        {
            jdk = null;

            // simple test for java executable in bin/ directory
            var javaExe = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "java.exe" : "java";
            if (File.Exists(System.IO.Path.Combine(path, "bin", javaExe)) == false)
                return false;

            // simple test for javac executable in bin/ directory
            var javacExe = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "javac.exe" : "javac";
            if (File.Exists(System.IO.Path.Combine(path, "bin", javacExe)) == false)
                return false;

            // attempt to read the version
            if (TryReadVersion(path, out var version) == false)
                return false;

            jdk = new Jdk(path, version);
            return true;
        }

        /// <summary>
        /// Attempts to determine the JDK version.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        static bool TryReadVersion(string path, out JdkVersion version)
        {
            if (TryReadVersionFromRelease(path, out version))
                return true;

            if (TryReadVersionFromPath(path, out version))
                return true;

            version = default;
            return false;
        }

        /// <summary>
        /// Attempts to read JDK information from the 'release' file, distributed in JDK9.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static bool TryReadVersionFromRelease(string path, out JdkVersion version)
        {
            version = default;

            if (File.Exists(System.IO.Path.Combine(path, "release")))
            {
                try
                {
                    var rel = File.ReadAllLines(System.IO.Path.Combine(path, "release"));

                    var ver = rel.FirstOrDefault(i => i.StartsWith("JAVA_VERSION="));
                    if (ver is null)
                        return false;

                    ver = ver.Remove(0, 13).Trim('"');
                    version = JdkVersion.Parse(ver);
                    return true;
                }
                catch (Exception)
                {

                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to probe the version from the path alone.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        static bool TryReadVersionFromPath(string path, out JdkVersion version)
        {
            version = default;
            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if the path looks like a JDK. This is a very simple test but avoids more complicated tests.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool LooksLikeJdkHome(string path)
        {
            return path.IndexOf("jdk", StringComparison.InvariantCultureIgnoreCase) != -1 || path.IndexOf("java", StringComparison.InvariantCultureIgnoreCase) != -1;
        }

        /// <summary>
        /// Gets the Java family version (8, 9, etc).
        /// </summary>
        public int Family => Version.Feature >= 9 ? Version.Feature : Version.Interim;

    }

}
