// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2022-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2022-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Robot Components Libs
using RobotComponents.ABB.Controllers;
using RobotComponents.ABB.Gh.Parameters.Controllers;

namespace RobotComponents.ABB.Gh.Components.ControllerUtility
{
    /// <summary>
    /// Represents the component that runs a program.
    /// </summary>
    public class RunProgramComponent : GH_RobotComponent
    {
        #region fields
        private Controller _controller;
        private string _status = "-";
        private bool _succeeded = true;
        #endregion

        /// <summary>
        /// Initializes a new instance of the RunProgramComponent class.
        /// </summary>
        public RunProgramComponent() : base("Run Program", "RP", "Controller Utility",
              "Starts and stops RAPID programs directly on a real or virtual ABB controller."
                + System.Environment.NewLine + System.Environment.NewLine +
                "This component uses the ABB PC SDK.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Controller(), "Controller", "C", "Controller as Controller", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Run", "R", "Run as bool", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Stop", "S", "Stop/Pause as bool", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Reset", "R", "Resets the program pointer of all tasks as bool", GH_ParamAccess.item, false);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Status", "S", "Controller status.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Check the operating system
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is only supported on Windows operating systems.");
                return;
            }

            // Declare input variables
            bool run = false;
            bool stop = false;
            bool reset = false;

            // Catch the input data
            if (!DA.GetData(0, ref _controller)) { return; }
            if (!DA.GetData(1, ref run)) { run = false; }
            if (!DA.GetData(2, ref stop)) { stop = false; }
            if (!DA.GetData(3, ref reset)) { reset = false; }

            if (run)
            {
                _succeeded = _controller.RunProgram(out _status);
            }

            if (stop)
            {
                _succeeded = _controller.StopProgram(out _status);
            }

            if (reset)
            {
                _succeeded = _controller.ResetProgramPointers(out _status);
            }

            if (_succeeded == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, _status);
            }

            // Output
            DA.SetData(0, _status);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.RunProgram_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("89A12C4C-EA6F-435A-A068-9E4806FFFF62"); }
        }
        #endregion
    }
}