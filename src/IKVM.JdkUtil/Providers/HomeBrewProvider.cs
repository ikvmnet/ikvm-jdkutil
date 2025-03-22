using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace IKVM.JdkUtil.Providers
{

    class HomeBrewProvider : JdkProvider
    {

        const string HOMEBREW_DIR_INTEL = "/usr/local/opt";
        const string HOMEBREW_DIR_APPLE_SILLICON = "/opt/homebrew/opt";
        const string HOMEBREW_DIR_LINUX = "/home/linuxbrew/.linuxbrew/opt";

        public override IEnumerable<string> Scan()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                    foreach (var i in ScanHomeBrew(HOMEBREW_DIR_INTEL))
                        yield return i;

                if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                    foreach (var i in ScanHomeBrew(HOMEBREW_DIR_APPLE_SILLICON))
                        yield return i;
            }


            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                foreach (var i in ScanHomeBrew(HOMEBREW_DIR_LINUX))
                    yield return i;
            }
        }

        IEnumerable<string> ScanHomeBrew(string homeBrewPath)
        {
            if (Directory.Exists(homeBrewPath))
                foreach (var dir in Directory.EnumerateDirectories(homeBrewPath))
                    if (Jdk.LooksLikeJdkHome(dir))
                        yield return dir;
        }

    }

}
