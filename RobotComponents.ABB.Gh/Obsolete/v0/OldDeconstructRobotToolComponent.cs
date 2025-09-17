﻿// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Definitions;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Deconstruct Robot Tool component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldDeconstructRobotToolComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructRobotTool class.
        /// </summary>
        public OldDeconstructRobotToolComponent()
          : base("Deconstruct Robot Tool", "DeRobTool",
              "Deconstructs a Robot Tool component into its parameters."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "RobotComponents", "Deconstruct")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return true; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_RobotTool(), "Robot Tool", "RT", "Robot Tool as Robot Tool", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Robot Tool Name as String", GH_ParamAccess.item);
            pManager.AddMeshParameter("Mesh", "M", "Robot Tool Mesh as Mesh", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Attachment Plane", "AP", "Robot Tool Attachment Plane as Plane", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Tool Plane", "TP", "Robot Tool Plane as Plane", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            RobotTool robotTool = null;

            // Catch the input data
            if (!DA.GetData(0, ref robotTool)) { return; }

            // Check if the object is valid
            if (!robotTool.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Robot Tool is not valid");
            }

            // Output
            DA.SetData(0, robotTool.Name);
            DA.SetData(1, robotTool.Mesh);
            DA.SetData(2, robotTool.AttachmentPlane);
            DA.SetData(3, robotTool.ToolPlane);
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructRobotTool_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("786e8c00-f24e-4dda-953d-cc8cffefa131"); }
        }
    }
}