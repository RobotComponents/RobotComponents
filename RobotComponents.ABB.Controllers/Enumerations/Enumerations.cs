// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.ABB.Controllers.Enumerations
{
    /// <summary>
    /// Defines the coordinate system.
    /// </summary>
    public enum CoordinateSystemType : int
    {
        /// <summary>
        /// World coordinate system
        /// </summary>
        World = 1,

        /// <summary>
        /// Base coordinate system.
        /// </summary>
        Base = 2,

        /// <summary>
        /// Workobject coordite system.
        /// </summary>
        Workbobject = 4
    }
}
