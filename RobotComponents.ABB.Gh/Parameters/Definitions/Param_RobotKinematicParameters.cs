// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotcComponents Libs
using RobotComponents.ABB.Gh.Goos.Definitions;
using RobotComponents.ABB.Gh.Properties;

namespace RobotComponents.ABB.Gh.Parameters.Definitions
{
    /// <summary>
    /// Robot Kinematic Parameters parameter
    /// </summary>
    public class Param_RobotKinematicParameters : GH_RobotParam<GH_RobotKinematicParameters>
    {
        /// <summary>
        /// Initializes a new instance of the Param_RobotKinematicParameters class
        /// </summary>
        public Param_RobotKinematicParameters() : base("Robot Kinematic Parameters Parameter", "LDP", "Parameters",
                "Contains the data of a Robot Kinematic Parameters instance.")
        {
        }

        /// <summary>
        /// Converts this structure to a human-readable string.
        /// </summary>
        /// <returns> A string representation of the parameter. </returns>
        public override string ToString()
        {
            return "Robot Kinematic Parameters";
        }

        /// <summary>
        /// Gets or sets the name of the object. This field typically remains fixed during the lifetime of an object.
        /// </summary>
        public override string Name { get => "Robot Kinematic Parameters"; set => base.Name = value; }

        /// <summary>
        /// Override this function to supply a custom icon (24x24 pixels). 
        /// The result of this property is cached, so don't worry if icon retrieval is not very fast.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Resources.RobotKinematicParameters_Parameter_Icon; }
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.The default is to expose everywhere.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.septenary; }
        }

        /// <summary>
        /// Returns a consistent ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("642693A5-1A10-4700-8434-34A178606278"); }
        }
    }
}
