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
// RobotComponents Libs
using RobotComponents.Gh.Utils;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.07.002.
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Utilities
{
    /// <summary>
    /// RobotComponents convert quarternion to plane component. An inherent from the GH_Component Class.
    /// </summary>
    public class OldQuaternionToPlane : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the QuarernionToPlane class
        /// </summary>
        public OldQuaternionToPlane()
          : base("Quaternion to Plane", "QtoP",
              "Calculates the plane from the four coefficient values in a quarternion. "
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
            pManager.AddNumberParameter("Quaternion A", "A", "The real part of the quaternion.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Quaternion B", "B", "The first imaginary coefficient of the quaternion.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Quaternion C", "C", "The second imaginary coefficient of the quaternion.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Quaternion D", "D", "The third imaginary coefficient of the quaternion.", GH_ParamAccess.list);
            pManager.AddPointParameter("Origin", "O", "The plane origin as a Point", GH_ParamAccess.list, new List<Point3d>() { new Point3d(0, 0, 0) });
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_PlaneParam("Plane", "P", "Plane as a Plane.");
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
            List<double> quatA = new List<double>();
            List<double> quatB = new List<double>();
            List<double> quatC = new List<double>();
            List<double> quatD = new List<double>();
            List<Point3d> points = new List<Point3d>();

            // Catch the input data
            if (!DA.GetDataList(0, quatA)) { return; }
            if (!DA.GetDataList(1, quatB)) { return; }
            if (!DA.GetDataList(2, quatC)) { return; }
            if (!DA.GetDataList(3, quatD)) { return; }
            if (!DA.GetDataList(4, points)) { return; }

            // Output variables
            List<Plane> planes = new List<Plane>();

            // Initiate other variables
            Quaternion quat;

            // Get longest Input List
            int[] sizeValues = new int[5];
            sizeValues[0] = points.Count;
            sizeValues[1] = quatA.Count;
            sizeValues[2] = quatB.Count;
            sizeValues[3] = quatC.Count;
            sizeValues[4] = quatD.Count;
            int biggestSize = HelperMethods.GetBiggestValue(sizeValues);

            // Keeps track of used indicies
            int pointCounter = -1;
            int aCounter = -1;
            int bCounter = -1;
            int cCounter = -1;
            int dCounter = -1;

            // Creates planes (asynchronous)
            for (int i = 0; i < biggestSize; i++)
            {
                Point3d point;
                double A;
                double B;
                double C;
                double D;

                // Points
                if (i < sizeValues[0])
                {
                    point = points[i];
                    pointCounter++;
                }
                else
                {
                    point = points[pointCounter];
                }

                // Quat A
                if (i < sizeValues[1])
                {
                    A = quatA[i];
                    aCounter++;
                }
                else
                {
                    A = quatA[aCounter];
                }

                // Quat B
                if (i < sizeValues[2])
                {
                    B = quatB[i];
                    bCounter++;
                }
                else
                {
                    B = quatB[bCounter];
                }

                // Quat C
                if (i < sizeValues[3])
                {
                    C = quatC[i];
                    cCounter++;
                }
                else
                {
                    C = quatC[cCounter];
                }

                // Quat D
                if (i < sizeValues[4])
                {
                    D = quatD[i];
                    dCounter++;
                }
                else
                {
                    D = quatD[dCounter];
                }

                // Create quaternion
                quat = new Quaternion(A, B, C, D);

                // Convert to plane and add to list
                quat.GetRotation(out Plane plane);
                plane = new Plane(point, plane.XAxis, plane.YAxis);
                planes.Add(plane);
            }

            // Output
            DA.SetDataList(0, planes);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.QuatToPlane_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D116DCB9-AE4E-450B-B401-47075AF97D5A"); }
        }
    }
}