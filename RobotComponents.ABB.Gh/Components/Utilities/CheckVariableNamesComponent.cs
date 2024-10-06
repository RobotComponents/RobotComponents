// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.Utilities
{
    /// <summary>
    /// RobotComponents Set Check Variable Names component. An inherent from the GH_Component Class.
    /// </summary>
    public class CheckVariableNamesComponent : GH_Component
    {
        #region fields
        private ObjectManager _objectManager;
        private GH_Document _doc;
        #endregion

        /// <summary>
        /// Initializes a new instance of the SetCheckVariableNamesComponent class.
        /// </summary>
        public CheckVariableNamesComponent()
          : base("Check Variable Names", "CVN",
              "Sets if the variable names should be checked for duplicates throughout this Grasshopper document."
             + System.Environment.NewLine + System.Environment.NewLine + "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Check", "C", "If set to true, the variable names will be checked throughout this Grasshopper document.", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            // This components has no output parameters.
        }

        /// <summary>
        /// Override this method if you want to be called before the first call to SolveInstance.
        /// </summary>
        protected override void BeforeSolveInstance()
        {
            base.BeforeSolveInstance();

            _doc = OnPingDocument();
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            bool check = true;

            // Catch the input data
            if (!DA.GetData(0, ref check)) { return; }

            // Get object manager
            _objectManager = DocumentManager.GetDocumentObjectManager(_doc);

            // Add component to collection
            if (!_objectManager.CheckVariableNamesComponents.ContainsKey(InstanceGuid))
            {
                _objectManager.CheckVariableNamesComponents.Add(InstanceGuid, this);
            }

            // Set value
            if (_objectManager.CheckVariableNamesComponents.Count == 1 & RunCount == 1)
            {
                _objectManager.IsCheckingVariableNames = check;
            }
            else if (_objectManager.CheckVariableNamesComponents.Count > 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The check variable names parameter was not changed. Only use one Check Variable Names component per document.");
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The check variable names parameter was not changed. Only one item is allowed as input.");
            }
        }

        /// <summary>
        /// Override this method if you want to be called after the last call to SolveInstance.
        /// </summary>
        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();

            if (_doc != null)
            {
                _doc.ObjectsDeleted += DocumentObjectsDeleted;
            }
        }

        #region object manager
        /// <summary>
        /// Detect if the components gets removed from the canvas and deletes the 
        /// objects created with this components from the object manager. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        public void DocumentObjectsDeleted(object sender, GH_DocObjectEventArgs e)
        {
            if (e.Objects.Contains(this))
            {
                _objectManager.CheckVariableNamesComponents.Remove(this.InstanceGuid);

                foreach (KeyValuePair<Guid, CheckVariableNamesComponent> entry in _objectManager.CheckVariableNamesComponents)
                {
                    entry.Value.ExpireSolution(true);
                }
            }
        }
        #endregion      

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.senary | GH_Exposure.obscure; }
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
            get { return Properties.Resources.VariableNameCheck_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7E565209-881B-4800-8BB8-50AEFE823403"); }
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