﻿// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Controller Utility : Get and read the Digital Outputs from a defined controller. An inherent from the GH_Component Class.
    /// </summary>
    public class GetDigitalOutputComponent_OBSOLETE : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetDigitalOutput class.
        /// </summary>
        [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
        public GetDigitalOutputComponent_OBSOLETE()
          : base("Get Digital Output", "GetDO",
              "Gets the signal of a defined digital output from an ABB IRC5 robot controller."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Controller Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot Controller", "RC", "Robot Controller to be connected to as Robot Controller", GH_ParamAccess.item);
            pManager.AddTextParameter("DO Name", "N", "Digital Output Name as text", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Signal", "S", "Signal of the Digital Output", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "This component is OBSOLETE. Pick the new component from the toolbar.");
        }

        #region properties
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
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return RobotComponents.ABB.Gh.Properties.Resources.GetDigitalOutput_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("15E9EB1D-3EC5-44FB-9694-0DAC7C37AD97"); }
        }
        #endregion
    }
}