using System;
using System.Collections.Generic;
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
        public string? JdkPath { get; set; }

        /// <summary>
        /// Version of the resolved JDK.
        /// </summary>
        [Output]
        public string? JdkVersion { get; set; }

        /// <summary>
        /// Gets the full set of JDKs that match.
        /// </summary>
        [Output]
        public ITaskItem[]? JdkList { get; set; }

        /// <inheritdoc />
        public override bool Execute()
        {
            var family = default(int?);
            if (string.IsNullOrWhiteSpace(Family) == false)
                family = int.Parse(Family);

            var q = Jdk.Resolve(Before ?? [], IncludeDefault, After ?? []);
            if (family != null)
                q = q.Where(i => i.Family == family);

            // sort and set the output list
            q = q.OrderByDescending(i => i.Version);
            JdkList = q.Select(ToTaskItem).ToArray();

            // select the primary match
            var jdk = q.FirstOrDefault();
            if (jdk is not null)
            {
                JdkPath = jdk.Path;
                JdkVersion = jdk.Version.ToString();
            }

            return true;
        }

        /// <summary>
        /// Transforms the <see cref="Jdk"/> into a <see cref="TaskItem"/>.
        /// </summary>
        /// <param name="jdk"></param>
        /// <returns></returns>
        TaskItem ToTaskItem(Jdk jdk)
        {
            return new TaskItem(jdk.Path, new Dictionary<string, string?>()
            {
                ["Path"] = jdk.Path,
                ["Version"] = jdk.Version.ToString(),
            });
        }

    }

}
