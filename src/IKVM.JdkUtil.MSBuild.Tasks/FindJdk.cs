using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace IKVM.JdkUtil.MSBuild.Tasks
{

    /// <summary>
    /// Scans the system or provided paths for valid JDK installations.
    /// </summary>
    public class FindJdk : Task
    {

        /// <summary>
        /// Set of paths to scan before system paths.
        /// </summary>
        public string[]? Before { get; set; }

        /// <summary>
        /// Gets or sets whether to scan included system paths.
        /// </summary>
        public bool IncludeDefault { get; set; } = true;

        /// <summary>
        /// Set of paths to scan after system paths.
        /// </summary>
        public string[]? After { get; set; }

        /// <summary>
        /// Sets the JDK family version (8, 9, etc) to search for.
        /// </summary>
        public string? Family { get; set; }

        /// <summary>
        /// Home path of the resolved JDK.
        /// </summary>
        [Output]
        public string? JdkHomePath { get; set; }

        /// <summary>
        /// Version of the resolved JDK.
        /// </summary>
        [Output]
        public string? JdkVersion { get; set; }

        /// <inheritdoc />
        public override bool Execute()
        {
            var family = default(int?);
            if (string.IsNullOrWhiteSpace(Family) == false)
                family = int.Parse(Family);

            var q = Jdk.Resolve(Before ?? [], IncludeDefault, After ?? []);
            if (family != null)
                q = q.Where(i => i.Family == family);

            // find latest version
            q = q.OrderByDescending(i => i.Version);
            var jdk = q.FirstOrDefault();
            if (jdk == null)
                return true;

            // return result
            JdkHomePath = jdk.Path;
            JdkVersion = jdk.Version.ToString();
            return true;
        }

    }

}
