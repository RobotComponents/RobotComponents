// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Controllers;
using RobotComponents.ABB.Gh.Parameters.Controllers;

namespace RobotComponents.ABB.Gh.Components.ControllerUtility
{
    /// <summary>
    /// Represents the component that get the base frames of the robots from a defined controller.
    /// </summary>
    public class GetRobotBaseFramesComponent : GH_RobotComponent
    {
        #region fields
        private Controller _controller;
        #endregion

        /// <summary>
        /// Initializes a new instance of the GetRobotBaseFrameComponent class.
        /// </summary>
        public GetRobotBaseFramesComponent() : base("Get Robot Base Frames", "GetBF", "Controller Utility",
              "Gets the robot base frame from an ABB controller."
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
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name of the robot as text", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Plane", "P", "Base frame of the robot as a plane", GH_ParamAccess.list);
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

            // Catch input data
            if (!DA.GetData(0, ref _controller)) { return; }

            Dictionary<string, Plane> output = new Dictionary<string, Plane>();

            try
            {
                output = _controller.GetRobotBaseFrames();
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
            }

            // Output
            DA.SetDataList(0, output.Keys);
            DA.SetDataList(1, output.Values);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary | GH_Exposure.obscure; }
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
            get { return Properties.Resources.GetRobotBaseFrame_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("A399E86A-1DFA-4825-895B-8FE4218390E1"); }
        }
        #endregion
    }
}