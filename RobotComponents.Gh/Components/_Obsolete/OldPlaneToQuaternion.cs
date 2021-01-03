// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.07.002.
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents convert plane orientation to quarternion component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldPlaneToQuaternion : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Plane to Quarternion
        /// </summary>
        public OldPlaneToQuaternion()
          : base("Plane to Quaternion", "PtoQ",
              "Calculates the four coefficient values in a quarternion. "
                + "The first value a is the real part, while the rest multiplies i, j and k, that are imaginary. "
                + System.Environment.NewLine + System.Environment.NewLine + "quarternion = a + bi + ci + dk"
                + System.Environment.NewLine + System.Environment.NewLine 
                + "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Utility")
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
            pManager.Register_PointParam("Origin", "O", "The plane origin as a Point");
            pManager.Register_StringParam("String", "STR", "The quaternion in string format that can be used in RAPID code.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed " +
                "in the future. Remove this component from your canvas and replace it by picking the new component " +
                "from the ribbon.");

            // Input variables
            List<Plane> planes = new List<Plane>();

            // Catch the input data
            if (!DA.GetDataList(0, planes)) { return; }

            // Output variables
            List<double> quatA = new List<double>();
            List<double> quatB = new List<double>();
            List<double> quatC = new List<double>();
            List<double> quatD = new List<double>();
            List<Point3d> points = new List<Point3d>();
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

                // Add points
                points.Add(plane.Origin);

                // Write the quarternion value in the string format that is used in the RAPID and BASE code
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
            DA.SetDataList(4, points);
            DA.SetDataList(5, text);
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