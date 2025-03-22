using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using CliWrap;

namespace IKVM.JdkUtil.Providers
{

    class MacOSProvider : JdkProvider
    {

        static readonly string[] JDK_BASE_DIRS = [
            "/Library/Java/JavaVirtualMachines",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library/Java/JavaVirtualMachines")
        ];

        public override IEnumerable<string> Scan()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) == false)
                yield break;

            foreach (var i in FromJavaHomeUtil())
                yield return i;

            foreach (var i in FromDefaultInstallationLocation())
                yield return i;
        }

        IEnumerable<string> FromDefaultInstallationLocation()
        {
            foreach (var baseDir in JDK_BASE_DIRS)
                if (Directory.Exists(baseDir))
                    foreach (var dir in Directory.GetDirectories(Path.Combine(baseDir)))
                        yield return Path.Combine(dir, "Contents", "Home");
        }

        /// <summary>
        /// Runs the OSX java_home utility to detect SDKs.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> FromJavaHomeUtil()
        {
            if (File.Exists("/usr/libexec/java_home") == false)
                yield break;

            var lines = new List<string>();
            try
            {
                Cli.Wrap("/usr/libexec/java_home")
                    .WithStandardErrorPipe(PipeTarget.ToDelegate(lines.Add))
                    .ExecuteAsync()
                    .GetAwaiter()
                    .GetResult();
            }
            catch
            {

            }

            foreach (var line in lines)
                if (Directory.Exists(line))
                    yield return line;
        }

    }

}
