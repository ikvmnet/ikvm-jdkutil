using System.Collections.Generic;

namespace IKVM.JdkUtil
{

    /// <summary>
    /// A <see cref="JdkProvider"/> provides the paths to some JDK installations.
    /// </summary>
    abstract class JdkProvider
    {

        /// <summary>
        /// Finds possible locations of any JDKs.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<string> Scan();

    }

}
