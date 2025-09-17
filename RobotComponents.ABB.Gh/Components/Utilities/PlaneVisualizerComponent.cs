// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2018-2020 EDEK Uni Kassel
// Copyright (c) 2020-2025 Arjen Deetman
//
// Authors:
//   - Gabriel Rumph (2018-2020)
//   - Benedikt Wannemacher (2018-2020)
//   - Arjen Deetman (2019-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;

namespace RobotComponents.ABB.Gh.Components.Utilities
{
    /// <summary>
    /// RobotComponents Plane visualization component.
    /// </summary>
    public class PlaneVisualizerComponent : GH_RobotComponent
    {
        #region fields
        private readonly List<Plane> _planes = new List<Plane>();
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public PlaneVisualizerComponent() : base("Plane Visualizer", "PV", "Utility",
              "Visualizes the orientation vectors of a plane.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane as Plane", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            // This component has no ouput parameters. It only visualizes the plane orientation.
        }

        /// <summary>
        /// Override this method if you want to be called before the first call to SolveInstance.
        /// </summary>
        protected override void BeforeSolveInstance()
        {
            base.BeforeSolveInstance();

            _planes.Clear();
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            Plane plane = Plane.Unset;

            // Catch input data
            if (!DA.GetData(0, ref plane)) { return; }

            // Add plane to list
            _planes.Add(plane);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.Plane_Visualizer_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F861C697-DE9D-483E-9651-C53649775412"); }
        }
        #endregion

        #region custom preview method
        /// <summary>
        /// This methods displays the three vectors of all the planes in the list.
        /// </summary>
        /// <param name="args"> Preview display arguments for IGH_PreviewObjects. </param>
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            for (int i = 0; i < _planes.Count; i++)
            {
                Plane plane = _planes[i];

                if (plane != null)
                {
                    if (plane.IsValid == true)
                    {
                        args.Display.DrawDirectionArrow(plane.Origin, plane.ZAxis, System.Drawing.Color.Blue);
                        args.Display.DrawDirectionArrow(plane.Origin, plane.XAxis, System.Drawing.Color.Red);
                        args.Display.DrawDirectionArrow(plane.Origin, plane.YAxis, System.Drawing.Color.Green);
                    }
                }
            }
        }
        #endregion
    }
}
