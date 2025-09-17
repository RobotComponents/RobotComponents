// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2020 EDEK Uni Kassel
// Copyright (c) 2020-2024 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2020-2024)
//
// For license details, see the LICENSE file in the project root.

// System lib
using System.Collections.Generic;
// Robot Components Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;

namespace RobotComponents.ABB.Actions.Declarations
{
    /// <summary>
    /// Represents the interface for different Joint Positions.
    /// </summary>
    public interface IJointPosition
    {
        #region constructors
        /// <summary>
        /// Returns an exact duplicate of this IJointPosition.
        /// </summary>
        /// <returns> 
        /// The exact copy of this IJointPosition. 
        /// </returns>
        IJointPosition DuplicateJointPosition();
        #endregion

        #region methods
        /// <summary>
        /// Returns the Joint Position as an array with axis values.
        /// </summary>
        /// <returns> 
        /// The array containing the joint positions. 
        /// </returns>
        double[] ToArray();

        /// <summary>
        /// Returns the Joint Position as a list with axis values.
        /// </summary>
        /// <returns> 
        /// The list containing the the joint positions. 
        /// </returns>
        List<double> ToList();

        /// <summary>
        /// Returns the Joint Position in RAPID code format, e.g. "[0, 0, 0, 0, 45, 0]".
        /// </summary>
        /// <returns> 
        /// The string with joint positions. 
        /// </returns>
        string ToRAPID();

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// The RAPID code line. 
        /// </returns>
        string ToRAPIDDeclaration(Robot robot);

        /// <summary>
        /// Creates declarations and instructions in the RAPID program module inside the RAPID Generator.
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.
        /// </remarks>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        void ToRAPIDGenerator(RAPIDGenerator RAPIDGenerator);

        /// <summary>
        /// Sets all the elements in the joint position back to its default value.
        /// </summary>
        void Reset();
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the Joint Position variable name.
        /// </summary>
        /// <remarks>
        /// Each variable name has to be unique.
        /// </remarks>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the variable type. 
        /// </summary>
        VariableType VariableType { get; set; }

        /// <summary>
        /// Gets the number of elements in the joint position.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets or sets the joint position through the indexer. 
        /// </summary>
        /// <param name="index"> The index number. </param>
        /// <returns> 
        /// The joint position located at the given index. 
        /// </returns>
        double this[int index] { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the joint position array has a fixed size.
        /// </summary>
        bool IsFixedSize { get; }
        #endregion
    }
}