using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace RobotComponentsABB.Components.Utilities
{
    /// <summary>
    /// RobotComponents convert plane orientation to quarternion component. An inherent from the GH_Component Class.
    /// </summary>
    public class PlaneOrientationToQuaternion : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PlaneOrientation to Quarternion
        /// </summary>
        public PlaneOrientationToQuaternion()
          : base("Plane Orientation to Quaternion", "PtoQ",
              "Calculates the four coefficient values in a quarternion. "
                + "The first value a is the real part, while the rest multiplies i, j and k, that are imaginary. "
                + System.Environment.NewLine + System.Environment.NewLine + "quarternion = a + bi + ci + dk"
                + System.Environment.NewLine + System.Environment.NewLine 
                + "RobotComponents : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane as Plane", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("Quaternion A", "A", "The real part of the quaternion.");
            pManager.Register_DoubleParam("Quaternion B", "B", "The first imaginary coefficient of the quaternion.");
            pManager.Register_DoubleParam("Quaternion C", "C", "The second imaginary coefficient of the quaternion.");
            pManager.Register_DoubleParam("Quaternion D", "D", "The third imaginary coefficient of the quaternion.");
            pManager.Register_StringParam("String", "STR", "The quaternion in string format that can be used in RAPID code.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            List<Plane> planes = new List<Plane>();

            // Catch the input data
            if (!DA.GetDataList(0, planes)) { return; }

            // Output variables
            List<double> quatA = new List<double>();
            List<double> quatB = new List<double>();
            List<double> quatC = new List<double>();
            List<double> quatD = new List<double>();
            List<string> text = new List<string>();

            // Create variables for necessary for getting the quarternion
            Quaternion quat;
            Plane refPlane = Plane.WorldXY;

            // Loop over all the input planes and get the quarternion of the plane
            for (int i = 0; i < planes.Count; i++)
            {
                // Get the individual plane
                Plane plane = planes[i];

                // Get the quarternion
                quat = Quaternion.Rotation(refPlane, plane);

                // Save the four quarternion values
                quatA.Add(quat.A);
                quatB.Add(quat.B);
                quatC.Add(quat.C);
                quatD.Add(quat.D);

                // Writet the quarternion value in the string format that is used in the RAPID and BASE code
                text.Add("[" 
                    + quat.A.ToString("0.######") + ", " 
                    + quat.B.ToString("0.######") + ", " 
                    + quat.C.ToString("0.######") + ", " 
                    + quat.D.ToString("0.######") + "]");
            }

            // Output
            DA.SetDataList(0, quatA);
            DA.SetDataList(1, quatB);
            DA.SetDataList(2, quatC);
            DA.SetDataList(3, quatD);
            DA.SetDataList(4, text);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.PlaneToQuat_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("020BB960-87C9-4325-B69B-0B15CBE9D52A"); }
        }
    }
}