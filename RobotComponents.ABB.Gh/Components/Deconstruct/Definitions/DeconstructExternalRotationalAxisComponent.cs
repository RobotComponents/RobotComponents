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
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Definitions;

namespace RobotComponents.ABB.Gh.Components.Deconstruct.Definitions
{
    /// <summary>
    /// RobotComponents Deconstruct External Rotational Axis Component. 
    /// </summary>
    public class DeconstructExternalRotationalAxisComponent : GH_RobotComponent
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructExternalRotationalAxisComponent class.
        /// </summary>
        public DeconstructExternalRotationalAxisComponent() : base("Deconstruct External Rotational Axis", "DeConERA", "Deconstruct",
              "Deconstructs an External Rotational Axis component into its parameters.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_ExternalRotationalAxis(), "External Rotational Axis", "ERA", "External Rotational Axis as External Rotational Axis", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Axis Name as a Text", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Axis Plane", "AP", "Axis Plane as Plane", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Axis Limits", "AL", "Axis Limits as Domain", GH_ParamAccess.item);
            pManager.AddMeshParameter("Base Mesh", "BM", "Base Mesh as Mesh", GH_ParamAccess.item);
            pManager.AddMeshParameter("Link Mesh", "LM", "Link Mesh as Mesh", GH_ParamAccess.item);
            pManager.AddTextParameter("Axis Logic Number", "AL", "Axis Logic Number as Text", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Moves Robot", "MR", "Moves Robot as Boolean", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            ExternalRotationalAxis externalRotationalAxis = null;

            // Catch the input data
            if (!DA.GetData(0, ref externalRotationalAxis)) { return; }

            if (externalRotationalAxis != null)
            {
                // Check if the object is valid
                if (!externalRotationalAxis.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The External Rotational Axis is invalid");
                }

                // Output
                DA.SetData(0, externalRotationalAxis.Name);
                DA.SetData(1, externalRotationalAxis.AxisPlane);
                DA.SetData(2, externalRotationalAxis.AxisLimits);
                DA.SetData(3, externalRotationalAxis.BaseMesh);
                DA.SetData(4, externalRotationalAxis.LinkMesh);
                DA.SetData(5, externalRotationalAxis.AxisLogic);
                DA.SetData(6, externalRotationalAxis.MovesRobot);
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
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
            get { return Properties.Resources.DeconstructExternalRotationalAxis_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("947F18D8-5789-485D-BD81-B93778124934"); }
        }
        #endregion
    }
}