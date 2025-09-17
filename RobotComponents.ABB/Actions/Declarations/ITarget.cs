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

// Robot Component Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;

namespace RobotComponents.ABB.Actions.Declarations
{
    /// <summary>
    /// Represents the interface for different target types.
    /// </summary>
    public interface ITarget
    {
        #region constructors
        /// <summary>
        /// Returns an exact duplicate of this Target.
        /// </summary>
        /// <returns> The exact copy of this Target. </returns>
        ITarget DuplicateTarget();
        #endregion

        #region methods

        /// <summary>
        /// Returns the Target in RAPID code format.
        /// </summary>
        /// <returns> 
        /// The RAPID data string. 
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
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets or sets the Variable Type.
        /// </summary>
        VariableType VariableType { get; set; }

        /// <summary>
        /// Gets or sets the Target variable name.
        /// Each Target variable name has to be unique.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the External Joint Position.
        /// </summary>
        ExternalJointPosition ExternalJointPosition { get; set; }
        #endregion
    }
}