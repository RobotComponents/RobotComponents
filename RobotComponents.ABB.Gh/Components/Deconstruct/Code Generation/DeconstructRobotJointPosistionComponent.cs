// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2020 EDEK Uni Kassel
// Copyright (c) 2020-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2020-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;

namespace RobotComponents.ABB.Gh.Components.Deconstruct.CodeGeneration
{
    /// <summary>
    /// RobotComponents Deconstruct Rob Joint Position component. 
    /// </summary>
    public class DeconstructRobotJointPosistionComponent : GH_RobotComponent
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructExtJointPos class.
        /// </summary>
        public DeconstructRobotJointPosistionComponent() : base("Deconstruct Robot Joint Position", "DeConRobJoint", "Deconstruct",
              "Deconstructs a Robot Joint Position Component into its parameters.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_RobotJointPosition(), "Robot Joint Position", "RJ", "The Robot Joint Position.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Robot joint position variable name as text");
            pManager.Register_DoubleParam("Robot joint position 1", "RJ1", "Defines the position of robot joint 1 in degrees.");
            pManager.Register_DoubleParam("Robot joint position 2", "RJ2", "Defines the position of robot joint 2 in degrees.");
            pManager.Register_DoubleParam("Robot joint position 3", "RJ3", "Defines the position of robot joint 3 in degrees.");
            pManager.Register_DoubleParam("Robot joint position 4", "RJ4", "Defines the position of robot joint 4 in degrees.");
            pManager.Register_DoubleParam("Robot joint position 5", "RJ5", "Defines the position of robot joint 5 in degrees.");
            pManager.Register_DoubleParam("Robot joint position 6", "RJ6", "Defines the position of robot joint 6 in degrees.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            RobotJointPosition robJointPosition = null;

            // Catch the input data
            if (!DA.GetData(0, ref robJointPosition)) { return; }

            if (robJointPosition != null)
            {
                // Check if the object is valid
                if (!robJointPosition.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Robot Joint Position is invalid");
                }

                // Output
                DA.SetData(0, robJointPosition.Name);
                DA.SetData(1, robJointPosition[0]);
                DA.SetData(2, robJointPosition[1]);
                DA.SetData(3, robJointPosition[2]);
                DA.SetData(4, robJointPosition[3]);
                DA.SetData(5, robJointPosition[4]);
                DA.SetData(6, robJointPosition[5]);
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructRobotJointPosition_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F1308FD1-F3E3-43EC-8ACA-54990B1664FF"); }
        }
        #endregion
    }
}