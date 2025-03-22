using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace IKVM.JdkUtil.Providers
{

    class WindowsProvider : JdkProvider
    {

        static readonly string[] PROGRAM_DIRS = [
            Environment.GetEnvironmentVariable("ProgramW6432"),
            Environment.GetEnvironmentVariable("ProgramFiles"),
            Environment.GetEnvironmentVariable("LOCALAPPDATA"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local")
        ];

        static readonly string[] POPULAR_DISTRIBUTIONS = [
            "Eclipse Foundation", // Adoptium
            "Eclipse Adoptium", // Eclipse Temurin
            "Java", // Oracle Java SE
            "Amazon Corretto",
            "Microsoft", // Microsoft OpenJDK
            Path.Combine("SapMachine", "JDK"), // SAP Machine
            "Zulu", // Azul OpenJDK
        ];

        /// <inheritdoc />
        public override IEnumerable<string> Scan()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                foreach (var programDir in PROGRAM_DIRS)
                    foreach (var distro in POPULAR_DISTRIBUTIONS)
                        if (Directory.Exists(Path.Combine(programDir, distro)))
                            foreach (var dir in Directory.GetDirectories(Path.Combine(programDir, distro)))
                                yield return dir;
        }

    }

}
