﻿// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Action : Target component. An inherent from the GH_Component Class.
    /// This is a dummy component that is hidden in Grasshopper. It is only called and used in
    /// the background to create a datatree structure that follows the Grasshopper logic.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class RobotTargetComponentDataTreeGenerator_OBSOLETE : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public RobotTargetComponentDataTreeGenerator_OBSOLETE()
          : base("Robot Target", "RT",
              "Defines a Robot Target declaration for an Instruction : Movement or Inverse Kinematics component."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Code Generation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name as text", GH_ParamAccess.item, string.Empty);
            pManager.AddPlaneParameter("Plane", "P", "Plane as Plane", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Reference Plane", "RP", "Reference Plane as a Plane", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Axis Configuration", "AC", "Axis Configuration as int. This will modify the fourth value of the Robot Configuration Data in the RAPID Movement code line.", GH_ParamAccess.item, 0);
            pManager.AddParameter(new Param_ExternalJointPosition(), "External Joint Position", "EJ", "The resulting external joint position", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_RobotTarget(), "Robot Target", "RT", "Resulting Robot Target");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Sets inputs and creates target
            string name = string.Empty;
            Plane plane = Plane.WorldXY;
            Plane referencePlane = Plane.WorldXY;
            int axisConfig = 0;
            ExternalJointPosition externalJointPosition = new ExternalJointPosition();

            // Catch inputs
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref plane)) { return; }
            if (!DA.GetData(2, ref referencePlane)) { return; }
            if (!DA.GetData(3, ref axisConfig)) { return; }
            if (!DA.GetData(4, ref externalJointPosition)) { return; }

            // Replace spaces
            name = HelperMethods.ReplaceSpacesAndRemoveNewLines(name);

            RobotTarget target = new RobotTarget(name, plane, referencePlane, new ConfigurationData(0, 0, 0, axisConfig), externalJointPosition);

            // Sets Output
            DA.SetData(0, target);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            // This component is hidden. It is only used to create a datatree inside the real Robot Target component.
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
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.RobTarget_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("850C3C56-8E81-43AF-A17E-ADF4382F740E"); }
        }
        #endregion
    }
}
