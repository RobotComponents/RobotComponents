// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System lib
using System.Collections.Generic;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// Defines the interface for different joint position types
    /// </summary>
    public interface IJointPosition
    {
        #region methods
        /// <summary>
        /// Copies the axis values of the joint position to a new array
        /// </summary>
        /// <returns> An array containing the axis values of the joint position. </returns>
        double[] ToArray();

        /// <summary>
        /// Copies the axis values of the joint position to a new list
        /// </summary>
        /// <returns> A list containing the axis values of the joint position. </returns>
        List<double> ToList();

        /// <summary>
        /// Sets all the elements in the joint position back to its default value.
        /// </summary>
        void Reset();
        #endregion

        #region properties
        /// <summary>
        /// Defines the number of elements in the joint position
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets or sets axis values through the indexer. 
        /// </summary>
        /// <param name="index"> The index number. </param>
        /// <returns> The axis value located at the given index. </returns>
        double this[int index] { get; set; }

        /// <summary>
        /// Gets a value indicating whether the joint position array has a fixed size.
        /// </summary>
        bool IsFixedSize { get; }
        #endregion
    }
}
