// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Gh.Parameters.Actions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Wait for Analog Input component. An inherent from the GH_Component Class.
    /// </summary>
    public class WaitAIComponent : GH_Component
    {
        #region fields
        private bool _expire = false;
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public WaitAIComponent()
          : base("Wait for Analog Input", "WAI",
              "Defines an instruction to wait for the signal of a Analog Input from the ABB robot controller."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.ABB.Utils.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Code Generation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name of the analog input signal as text.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Value", "V", "Desired value of the analog input signal as number.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Inequalty", "IS", "Inequality symbol that defines if the instruction waits until the value is less than or greater than the defined signal value.", GH_ParamAccess.item, 0);

            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_WaitAI(), "Wait AI", "WAI", "Resulting Wait for Analog Input instruction");   
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Creates the input value list and attachs it to the input parameter
            if (this.Params.Input[2].SourceCount == 0)
            {
                _expire = true;
                HelperMethods.CreateValueList(this, typeof(InequalitySymbol), 2);
            }

            // Expire solution of this component
            if (_expire == true)
            {
                _expire = false;
                this.ExpireSolution(true);
            }

            // Input variables
            string name = "";
            double value = 0.0;
            int inequality = 0;

            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref value)) { return; }
            if (!DA.GetData(2, ref inequality)) { return; }

            // Check inequality value
            if (inequality != 0 && inequality != 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Inequality value <" + inequality + "> is invalid. " +
                    "In can only be set to 0 or 1. Use 0 for less than (LT) and 1 for greater than (GT).");
            }

            // Check name
            name = HelperMethods.ReplaceSpacesAndRemoveNewLines(name);

            if (HelperMethods.StringExeedsCharacterLimit32(name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Analog input name exceeds character limit of 32 characters.");
            }
            if (HelperMethods.StringStartsWithNumber(name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Analog input name starts with a number which is not allowed in RAPID Code.");
            }
            if (HelperMethods.StringStartsWithNumber(name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Analog input name constains special characters which is not allowed in RAPID Code.");
            }

            // Create the action
            WaitAI waitAI = new WaitAI(name, value, (InequalitySymbol)inequality);

            // Sets Output
            DA.SetData(0, waitAI);
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
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.WaitAI_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9DB10042-0A29-466D-8008-7D33934066D5"); }
        }
        #endregion

        #region menu item
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Documentation", MenuItemClickComponentDoc, Properties.Resources.WikiPage_MenuItem_Icon);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Documentation" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickComponentDoc(object sender, EventArgs e)
        {
            string url = Documentation.ComponentWeblinks[this.GetType()];
            Documentation.OpenBrowser(url);
        }
        #endregion
    }

}
