// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2023-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2023-2025)
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
    /// RobotComponents Deconstruct Configuration Data component.
    /// </summary>
    public class DeconstructConfigurationDataComponent : GH_RobotComponent
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructConfigurationData class.
        /// </summary>
        public DeconstructConfigurationDataComponent() : base("Deconstruct Configuration Data", "DeConConf", "Deconstruct",
              "Deconstructs a Configuration Data component into its parameters.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_ConfigurationData(), "Configuration Data", "CD", "Configuration Data as Configuration Data or as a Number", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Name as string", GH_ParamAccess.item);
            pManager.Register_IntegerParam("CF1", "CF1", "The current quadrant of axis 1 as a number");
            pManager.Register_IntegerParam("CF4", "CF4", "The current quadrant of axis 4 as a number");
            pManager.Register_IntegerParam("CF6", "CF6", "The current quadrant of axis 6 as a number");
            pManager.Register_IntegerParam("CFX", "CFX", "The current robot configuration as a number");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            ConfigurationData configurationData = null;

            // Catch the input data
            if (!DA.GetData(0, ref configurationData)) { return; }

            if (configurationData != null)
            {
                // Check if the object is valid
                if (!configurationData.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Configuration Data is not valid");
                }

                // Output
                DA.SetData(0, configurationData.Name);
                DA.SetData(1, configurationData.Cf1);
                DA.SetData(2, configurationData.Cf4);
                DA.SetData(3, configurationData.Cf6);
                DA.SetData(4, configurationData.Cfx);
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
            get { return Properties.Resources.DeconstructConfigurationData_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5220758E-7864-46E1-AACD-AED9F21FC41E"); }
        }
        #endregion
    }
}