using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotComponents.Utils
{
    internal static class VersionNumbering
    {
        /// <summary>
        /// Returns the Current Version of the RobotComponent Plugin. Has to be manually updated each time
        /// 0.XX.XXX ---> mature release
        /// X.00.XXX ---> minor release(for example new functions new components...etc.)
        /// X.XX.000 ---> Bug fixes small improvements
        /// </summary>
        public static readonly string CurrentVersion = "0.04.015";

    }
}
