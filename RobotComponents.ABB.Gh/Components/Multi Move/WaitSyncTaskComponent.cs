﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Gh.Parameters.Actions.Instructions;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;
using RobotComponents.ABB.Gh.Goos.Actions.Instructions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.MultiMove
{
    /// <summary>
    /// RobotComponents Action : Wait Sync Task component. An inherent from the GH_Component Class.
    /// </summary>
    public class WaitSyncTaskComponent : GH_Component, IObjectManager
    {
        #region fields
        private GH_Structure<GH_WaitSyncTask> _tree = new GH_Structure<GH_WaitSyncTask>();
        private List<string> _registered = new List<string>();
        private readonly List<string> _toRegister = new List<string>();
        private ObjectManager _objectManager;
        private string _lastName = "";
        private bool _isUnique = true;
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public WaitSyncTaskComponent()
          : base("Wait Sync Task", "WST",
              "Defines a Wait Sync Task synchronization point for Multi Move programming."
               + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Multi Move")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Sync ID", "ID", "The name of the synchronization point.", GH_ParamAccess.item);
            pManager.AddParameter(new Param_TaskList(), "Task List", "TL", "The RAPID program tasks to synchronize as a Task List.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("In Position", "IP", "Specifies whether or not the robot and external axes must have come to a standstill in its synchronization point.", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_WaitSyncTask(), "Wait Sync Task", "WST", "Resulting Wait Sync Task instruction.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            string name = "";
            TaskList taskList = new TaskList();
            bool inPosition = false;

            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref taskList)) { return; }
            if (!DA.GetData(2, ref inPosition)) { return; }

            // Check variable name
            name = HelperMethods.ReplaceSpacesAndRemoveNewLines(name);

            // Create the action
            WaitSyncTask waitSyncTask = new WaitSyncTask(name, taskList, inPosition);

            // Output
            DA.SetData(0, waitSyncTask);
        }

        /// <summary>
        /// Override this method if you want to be called after the last call to SolveInstance.
        /// </summary>
        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();

            _tree = this.Params.Output[0].VolatileData as GH_Structure<GH_WaitSyncTask>;

            if (_tree.Branches.Count != 0)
            {
                if (_tree.Branches[0][0].Value.SyncID != string.Empty)
                {
                    UpdateVariableNames();
                }

                #region Object manager
                _toRegister.Clear();

                for (int i = 0; i < _tree.Branches.Count; i++)
                {
                    _toRegister.AddRange(_tree.Branches[i].ConvertAll(item => item.Value.SyncID));
                }

                GH_Document doc = this.OnPingDocument();
                _objectManager = DocumentManager.GetDocumentObjectManager(doc);
                _objectManager.CheckVariableNames(this);

                if (doc != null)
                {
                    doc.ObjectsDeleted += this.DocumentObjectsDeleted;
                }
                #endregion
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
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
            get { return Properties.Resources.WaitSyncTask_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("90823561-0B38-4A88-8D3D-324A5EF8B3DC"); }
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
                _objectManager.DeleteManagedData(this);
            }
        }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the variable names that are generated by this component are unique.
        /// </summary>
        public bool IsUnique
        {
            get { return _isUnique; }
            set { _isUnique = value; }
        }

        /// <summary>
        /// Gets or sets the current registered names.
        /// </summary>
        public List<string> Registered
        {
            get { return _registered; }
            set { _registered = value; }
        }

        /// <summary>
        /// Gets the variables names that need to be registered by the object manager.
        /// </summary>
        public List<string> ToRegister
        {
            get { return _toRegister; }
        }
        #endregion

        #region additional methods
        /// <summary>
        /// Updates the variable names in the data tree
        /// </summary>
        private void UpdateVariableNames()
        {
            // Check if it is a datatree with multiple branches that have one item
            bool check = true;
            for (int i = 0; i < _tree.Branches.Count; i++)
            {
                if (_tree.Branches[i].Count != 1)
                {
                    check = false;
                    break;
                }
            }

            if (_tree.Branches.Count == 1)
            {
                if (_tree.Branches[0].Count == 1)
                {
                    // Do nothing: there is only one item in the whole datatree
                }
                else
                {
                    // Only rename the items in this single branche with + "_0", "_1" etc...
                    for (int i = 0; i < _tree.Branches[0].Count; i++)
                    {
                        _tree.Branches[0][i].Value.SyncID = _tree.Branches[0][i].Value.SyncID + "_" + i.ToString();
                    }
                }

            }

            else if (check == true)
            {
                // Multiple branches with only one item per branch
                for (int i = 0; i < _tree.Branches.Count; i++)
                {
                    _tree.Branches[i][0].Value.SyncID = _tree.Branches[i][0].Value.SyncID + "_" + i.ToString();
                }
            }

            else
            {
                // Rename everything. There are multiple branches with branches that have multiple items. 
                List<GH_Path> originalPaths = new List<GH_Path>();
                for (int i = 0; i < _tree.Paths.Count; i++)
                {
                    originalPaths.Add(_tree.Paths[i]);
                }

                _tree.Simplify(GH_SimplificationMode.CollapseLeadingOverlaps);

                List<GH_Path> simplifiedPaths = new List<GH_Path>();
                for (int i = 0; i < _tree.Paths.Count; i++)
                {
                    simplifiedPaths.Add(_tree.Paths[i]);
                }

                for (int i = 0; i < _tree.Branches.Count; i++)
                {
                    _tree.ReplacePath(simplifiedPaths[i], originalPaths[i]);
                }

                for (int i = 0; i < _tree.Branches.Count; i++)
                {
                    GH_Path iPath = simplifiedPaths[i];
                    string pathString = iPath.ToString();
                    pathString = pathString.Replace("{", "").Replace(";", "_").Replace("}", "");

                    for (int j = 0; j < _tree.Branches[i].Count; j++)
                    {
                        _tree.Branches[i][j].Value.SyncID = _tree.Branches[i][j].Value.SyncID + "_" + pathString + "_" + j;
                    }
                }
            }
        }
        #endregion
    }

}
