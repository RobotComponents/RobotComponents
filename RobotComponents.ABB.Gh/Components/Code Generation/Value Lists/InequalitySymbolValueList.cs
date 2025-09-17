// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2020-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2020-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Drawing;
// Grasshopper Libs
using Grasshopper;
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.CodeGeneration.ValueLists
{
    /// <summary>
    /// RobotComponents Inequality Symbol Value List. An inherent from the GH_Component Class.
    /// Creates a value list with the inequality symbols. Since the value list is a special
    /// parameter, this component creates this value list and places it on the canvas. After
    /// constructing the value list this component will be deleted. This component only calls 
    /// the method to create the value list and defines the location of the values list. 
    /// </summary>
    public class InequalitySymbolValueList : GH_RobotComponent
    {
        #region fields
        private bool _created = false;
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public InequalitySymbolValueList() : base("Inequality Symbols", "IS", "Code Generation",
              "Defines a value list with inequality symbols.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Create the value list
            if (_created == false)
            {
                PointF location = new PointF(this.Attributes.Pivot.X, this.Attributes.Pivot.Y);
                _created = HelperMethods.CreateValueList(typeof(InequalitySymbol), location);
            }

            // Removes this component from the canvas
            Instances.ActiveCanvas.Document.RemoveObject(this, true);
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
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.InequalitySymbolValueList_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("AB8B12E5-F35C-4EC1-9E35-2CC0B17241EF"); }
        }
        #endregion
    }
}
