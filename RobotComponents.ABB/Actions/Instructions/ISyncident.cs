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

namespace RobotComponents.ABB.Actions.Instructions
{
    /// <summary>
    /// Represents the interface for actions that contain a synchronization identity.
    /// </summary>
    public interface ISyncident
    {
        #region constructors
        /// <summary>
        /// Returns an exact duplicate of this Syncident.
        /// </summary>
        /// <returns>
        /// The exact copy of this Syncident.
        /// </returns>
        ISyncident DuplicateSyncident();
        #endregion

        #region methods
        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// The RAPID code line. 
        /// </returns>
        string ToRAPIDDeclaration(Robot robot);

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        string ToRAPIDInstruction(Robot robot);

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
        /// Gets or sets the variable type of the syncident.
        /// </summary>
        VariableType VariableType { get; set; }

        /// <summary>
        /// Gets or sets the name of the synchronization (meeting) point (syncident).
        /// </summary>
        string SyncID { get; set; }
        #endregion
    }
}