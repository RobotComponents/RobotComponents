// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// Rhino Libs
using Rhino.Geometry;

namespace RobotComponents.Utils
{
    /// <summary>
    /// Represents the class with internal helper methods.
    /// </summary>
    internal static class HelperMethods
    {
        /// <summary>
        /// Writes a 3x3 matrix to the Rhinoceros command line for debugging purposes.
        /// </summary>
        /// <param name="m"> The matrix to be written to the command line. </param>
        public static void WriteMatrix3x3(Matrix m)
        {
            if (m.ColumnCount != 3 || m.RowCount != 3)
            {
                Rhino.RhinoApp.WriteLine("Matrix dimensions do not match the expected size.");
            }
            else
            {
                Rhino.RhinoApp.WriteLine($"{m[0, 0]}, {m[0, 1]}, {m[0, 2]}");
                Rhino.RhinoApp.WriteLine($"{m[1, 0]}, {m[1, 1]}, {m[1, 2]}");
                Rhino.RhinoApp.WriteLine($"{m[2, 0]}, {m[2, 1]}, {m[2, 2]}");
            }
        }
    }
}
