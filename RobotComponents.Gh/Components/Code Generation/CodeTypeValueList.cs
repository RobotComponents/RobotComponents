// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Drawing;
// Grasshopper Libs
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
// RobotComponents Libs
using RobotComponents.Enumerations;

namespace RobotComponents.Gh.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Code Type Value List. An inherent from the GH_Component Class.
    /// Creates a value list with the code types. Since the value list is a special
    /// parameter, this component creates this value list and places it on the canvas. After
    /// constructing the value list this component will be deleted. This component only calls 
    /// the method to create the value list and defines the location of the values list. 
    /// </summary>
    public class CodeTypeValueList : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public CodeTypeValueList()
          : base("Code Types", "CT",
              "Defines a value list with code types"
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

        }

        // Bool that indicates if the value list is created. This avoids infinite loops (because of the expire solution that is called).
        private bool _created = false;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Create the value list
            if (_created == false)
            {
                CreateValueList();
            }

            // Removes this component from the canvas
            Instances.ActiveCanvas.Document.RemoveObject(this, true);
        }

        #region valuelist
        /// <summary>
        /// Creates the value list for the motion type and connects it the input parameter is other source is connected
        /// </summary>
        private void CreateValueList()
        {
            // Creates the empty value list
            GH_ValueList obj = new GH_ValueList();
            obj.CreateAttributes();
            obj.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;
            obj.ListItems.Clear();

            // Add the items to the value list
            string[] names = Enum.GetNames(typeof(CodeType));
            int[] values = (int[])Enum.GetValues(typeof(CodeType));

            for (int i = 0; i < names.Length; i++)
            {
                obj.ListItems.Add(new GH_ValueListItem(names[i], values[i].ToString()));
            }

            // Make point where the valuelist should be created on the canvas
            obj.Attributes.Pivot = new PointF(this.Attributes.Pivot.X - obj.Attributes.Bounds.Width / 4, this.Attributes.Pivot.Y - obj.Attributes.Bounds.Height / 2);

            // Add the value list to the active canvas
            Instances.ActiveCanvas.Document.AddObject(obj, false);

            // Created
            _created = true;

            // Expire value list
            obj.ExpireSolution(true);
        }
        #endregion

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.CodeTypeValueList_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("15148FED-6A25-43FC-A427-97F9473A2FF4"); }
        }

    }
}
