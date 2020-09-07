// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.Enumerations
{
    /// <summary>
    /// Defines if the axis moves linear or rotational
    /// </summary>
    public enum AxisType : int
    { 
        /// <summary>
        /// Linear motion
        /// </summary>
        LINEAR = 0, 

        /// <summary>
        /// Rotational motion
        /// </summary>
        ROTATIONAL = 1
    }

    /// <summary>
    /// Defines the movement type
    /// </summary>
    public enum MovementType : int
    {
        /// <summary>
        /// Absolute joint movement
        /// </summary>
        MoveAbsJ = 0,

        /// <summary>
        /// Linear Movement
        /// </summary>
        MoveL = 1,

        /// <summary>
        /// Joint Movement
        /// </summary>
        MoveJ = 2
    }

    /// <summary>
    /// Defines the code type
    /// </summary>
    public enum CodeType : int
    {
        /// <summary>
        /// Instruction
        /// </summary>
        Instruction = 0,

        /// <summary>
        /// Declaration
        /// </summary>
        Declaration = 1
    }
}
