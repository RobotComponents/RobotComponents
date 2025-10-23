// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2021-2024 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2021-2024)
//
// For license details, see the LICENSE file in the project root.

// Robot Component Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;

namespace RobotComponents.ABB.Actions.Declarations
{
    /// <summary>
    /// Represents the interface for different declaration action types.
    /// </summary>
    public interface IDeclaration
    {
        #region constructors
        /// <summary>
        /// Returns an exact duplicate of this Declaration.
        /// </summary>
        /// <returns> 
        /// The exact copy of this Declaration. 
        /// </returns>
        IDeclaration DuplicateDeclaration();
        #endregion

        #region methods
        /// <summary>
        /// Returns the Declaration in RAPID code format.
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
        /// Gets or sets the scope
        /// </summary>
        Scope Scope { get; set; }

        /// <summary>
        /// Gets or sets the variable type. 
        /// </summary>
        VariableType VariableType { get; set; }

        /// <summary>
        /// Gets the RAPID datatype. 
        /// </summary>
        string Datatype { get; }

        /// <summary>
        /// Gets or sets the variable name of the declaration.
        /// </summary>
        /// <remarks>
        /// Each variable name has to be unique. 
        /// </remarks>
        string Name { get; set; }
        #endregion 
    }
}