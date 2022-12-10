// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

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
    /// RobotComponents Zone Data Value List. An inherent from the GH_Component Class.
    /// Creates a value list with the predefined zonedata. Since the value list is a special
    /// parameter, this component creates this value list and places it on the canvas. After
    /// constructing the value list this component will be deleted. This component only calls 
    /// the method to create the value list and defines the location of the values list. 
    /// </summary>
    public class ZoneDataValueList : GH_Component
    {
        #region fields
        private bool _created = false;
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public ZoneDataValueList()
          : base("Predefined Zone Data", "PZD",
              "Defines a value list with predefined zone data"
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
                _created = HelperMethods.CreateValueList(typeof(PredefinedZoneData), location);
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
            get { return GH_Exposure.quarternary; }
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
            get { return Properties.Resources.ZoneDataValueList_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B1B58A50-0116-49BC-9D69-C23C64DDE86E"); }
        }
        #endregion
    }
}
