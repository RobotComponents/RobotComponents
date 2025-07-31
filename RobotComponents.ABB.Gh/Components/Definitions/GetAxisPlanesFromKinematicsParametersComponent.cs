// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Rhino Libs
using Rhino.Geometry;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;

namespace RobotComponents.ABB.Gh.Components.Definitions
{
    /// <summary>
    /// RobotComponents Get Axis Planes from Kinematics Parameters component.
    /// </summary>
    public class GetAxisPlanesFromKinematicsParametersComponent : GH_RobotComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public GetAxisPlanesFromKinematicsParametersComponent() : base("Get Axis Planes from Kinematics Parameters", "KiPa2AxPl", "Definitions",
              "Gets the robot axis planes from the given robot kinematics parameters.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Position Plane", "P", "Position Plane of the Robot as Plane", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddNumberParameter("A1", "A1", "The shoulder offset as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("A2", "A2", "The elbow offset as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("A3", "A3", "The wrist offset as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("B", "B", "The lateral offset as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C1", "C1", "The first link length as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C2", "C2", "The second link length as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C3", "C3", "The third link length as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C4", "C4", "The fourth link length as a Number.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Axis Planes", "AP", "The Axis Planes as a list with Planes.", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Mounting Frame", "MF", "The tool mounting frame as a Plane.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            Plane plane = Plane.WorldXY;
            double a1 = 0;
            double a2 = 0;
            double a3 = 0;
            double b = 0;
            double c1 = 0;
            double c2 = 0;
            double c3 = 0;
            double c4 = 0;

            // Catch the input data
            if (!DA.GetData(0, ref plane)) { return; }
            if (!DA.GetData(1, ref a1)) { return; }
            if (!DA.GetData(2, ref a2)) { return; }
            if (!DA.GetData(3, ref a3)) { return; }
            if (!DA.GetData(4, ref b)) { return; }
            if (!DA.GetData(5, ref c1)) { return; }
            if (!DA.GetData(6, ref c2)) { return; }
            if (!DA.GetData(7, ref c3)) { return; }
            if (!DA.GetData(8, ref c4)) { return; }

            // Axis Planes
            Plane[] axisPlanes = Robot.GetAxisPlanesFromKinematicsParameters(plane, a1, a2, a3, b, c1, c2, c3, c4, out Plane mountingFrame);

            // Output
            DA.SetDataList(0, axisPlanes);
            DA.SetData(1, mountingFrame);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.senary; }
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
            get { return Properties.Resources.GetAxisPlanesFromKinematicsParameters_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("AA7A04CC-FFE2-487A-95D3-AF12E62D5BDB"); }
        }
        #endregion
    }
}