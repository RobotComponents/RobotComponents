// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System lib
using System.Collections.Generic;
// Robot Components Libs
using RobotComponents.Definitions;
using RobotComponents.Enumerations;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents the interface for different Joint Positions.
    /// </summary>
    public interface IJointPosition
    {
        #region methods
        /// <summary>
        /// Returns the Joint Position as an array with axis values.
        /// </summary>
        /// <returns> The array containing the axis values of the joint position. </returns>
        double[] ToArray();

        /// <summary>
        /// Returns the Joint Position as a list with axis values.
        /// </summary>
        /// <returns> The list containing the axis values of the joint position. </returns>
        List<double> ToList();

        /// <summary>
        /// Returns the Joint Position in RAPID code format, e.g. "[0, 0, 0, 0, 45, 0]".
        /// </summary>
        /// <returns> The string with axis values. </returns>
        string ToRAPID();

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        string ToRAPIDDeclaration(Robot robot);

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator);

        /// <summary>
        /// Sets all the elements in the joint position back to its default value.
        /// </summary>
        void Reset();
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the Joint Position variable name.
        /// Each Joint Position variable name has to be unique.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the Reference Type. 
        /// </summary>
        ReferenceType ReferenceType { get; set;  }

        /// <summary>
        /// Gets the number of elements in the joint position.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets or sets the axis values through the indexer. 
        /// </summary>
        /// <param name="index"> The index number. </param>
        /// <returns> The axis value located at the given index. </returns>
        double this[int index] { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the joint position array has a fixed size.
        /// </summary>
        bool IsFixedSize { get; }
        #endregion
    }
}
