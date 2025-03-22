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
        public int? Family { get; set; }

        /// <summary>
        /// Destination for the resolved JDK path.
        /// </summary>
        [Output]
        public string? HomePath { get; set; }

        /// <inheritdoc />
        public override bool Execute()
        {
            var q = Jdk.Resolve(Before ?? [], IncludeDefault, After ?? []);
            if (Family != null)
                q = q.Where(i => i.Family == Family);

            // find latest version
            q = q.OrderByDescending(i => i.Version);
            var jdk = q.FirstOrDefault();
            if (jdk == null)
                return true;

            // return result
            HomePath = jdk.Path;
            return true;
        }

    }

}
