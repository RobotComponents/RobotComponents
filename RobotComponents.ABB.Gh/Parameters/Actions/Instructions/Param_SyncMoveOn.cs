// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2021-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2021-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Gh.Goos.Actions.Instructions;

namespace RobotComponents.ABB.Gh.Parameters.Actions.Instructions
{
    /// <summary>
    /// Sync Move On parameter
    /// </summary>
    public class Param_SyncMoveOn : GH_RobotParam<GH_SyncMoveOn>
    {
        /// <summary>
        /// Initializes a new instance of the Param_SyncMoveOn class
        /// </summary>
        public Param_SyncMoveOn() : base("Sync Move On Parameter", "SMOn", "Parameters",
                "Contains the data of a Sync Move On synchronization point.")
        {
        }

        /// <summary>
        /// Converts this structure to a human-readable string.
        /// </summary>
        /// <returns> A string representation of the parameter. </returns>
        public override string ToString()
        {
            return "Sync Move On";
        }

        /// <summary>
        /// Gets or sets the name of the object. This field typically remains fixed during the lifetime of an object.
        /// </summary>
        public override string Name { get => "Sync Move On"; set => base.Name = value; }

        /// <summary>
        /// Override this function to supply a custom icon (24x24 pixels). 
        /// The result of this property is cached, so don't worry if icon retrieval is not very fast.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.SyncMoveOn_Parameter_Icon; }
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.The default is to expose everywhere.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary; }
        }

        /// <summary>
        /// Returns a consistent ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3E5F3E71-7582-4801-B24C-6F510410D2AF"); }
        }
    }
}
