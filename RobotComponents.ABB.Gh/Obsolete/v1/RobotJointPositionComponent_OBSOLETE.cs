// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
// RobotComponents Libs
using RobotComponents.ABB.Gh.Goos.Actions.Declarations;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Action : Robot Joint Position component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class RobotJointPositionComponent_OBSOLETE : GH_Component, IObjectManager
    {
        #region fields
        private GH_Structure<GH_RobotJointPosition> _tree = new GH_Structure<GH_RobotJointPosition>();
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
        public RobotJointPositionComponent_OBSOLETE()
          : base("Robot Joint Position", "RJ",
              "Defines a Robot Joint Position for a Joint Target declaration."
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
            pManager.AddTextParameter("Name", "N", "Name as text", GH_ParamAccess.tree, string.Empty);
            pManager.AddNumberParameter("Robot joint position 1", "RJ1", "Defines the position of robot joint 1 in degrees.", GH_ParamAccess.tree, 0.0);
            pManager.AddNumberParameter("Robot joint position 2", "RJ2", "Defines the position of robot joint 2 in degrees.", GH_ParamAccess.tree, 0.0);
            pManager.AddNumberParameter("Robot joint position 3", "RJ3", "Defines the position of robot joint 3 in degrees.", GH_ParamAccess.tree, 0.0);
            pManager.AddNumberParameter("Robot joint position 4", "RJ4", "Defines the position of robot joint 4 in degrees.", GH_ParamAccess.tree, 0.0);
            pManager.AddNumberParameter("Robot joint position 5", "RJ5", "Defines the position of robot joint 5 in degrees.", GH_ParamAccess.tree, 0.0);
            pManager.AddNumberParameter("Robot joint position 6", "RJ6", "Defines the position of robot joint 6 in degrees.", GH_ParamAccess.tree, 0.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_RobotJointPosition(), "Robot Joint Position", "RJ", "The resulting robot joint position");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Variables
            GH_Structure<GH_String> names;
            GH_Structure<GH_Number> internalAxisValues1 = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> internalAxisValues2 = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> internalAxisValues3 = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> internalAxisValues4 = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> internalAxisValues5 = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> internalAxisValues6 = new GH_Structure<GH_Number>();

            // Catch input data
            if (!DA.GetDataTree(0, out names)) { return; }
            if (!DA.GetDataTree(1, out internalAxisValues1)) { return; }
            if (!DA.GetDataTree(2, out internalAxisValues2)) { return; }
            if (!DA.GetDataTree(3, out internalAxisValues3)) { return; }
            if (!DA.GetDataTree(4, out internalAxisValues4)) { return; }
            if (!DA.GetDataTree(5, out internalAxisValues5)) { return; }
            if (!DA.GetDataTree(6, out internalAxisValues6)) { return; }

            // Clear tree 
            _tree = new GH_Structure<GH_RobotJointPosition>();

            // Create the datatree structure with an other component (in the background, this component is not placed on the canvas)
            RobotJointPositionComponentDataTreeGenerator_OBSOLETE component = new RobotJointPositionComponentDataTreeGenerator_OBSOLETE();

            component.Params.Input[0].AddVolatileDataTree(names);
            component.Params.Input[1].AddVolatileDataTree(internalAxisValues1);
            component.Params.Input[2].AddVolatileDataTree(internalAxisValues2);
            component.Params.Input[3].AddVolatileDataTree(internalAxisValues3);
            component.Params.Input[4].AddVolatileDataTree(internalAxisValues4);
            component.Params.Input[5].AddVolatileDataTree(internalAxisValues5);
            component.Params.Input[6].AddVolatileDataTree(internalAxisValues6);

            component.ExpireSolution(true);
            component.Params.Output[0].CollectData();

            _tree = component.Params.Output[0].VolatileData as GH_Structure<GH_RobotJointPosition>;

            if (_tree.Branches[0][0].Value.Name != string.Empty)
            {
                // Update the variable names in the data trees
                UpdateVariableNames();
            }

            // Sets Output
            DA.SetDataTree(0, _tree);

            #region Object manager
            _toRegister.Clear();

            for (int i = 0; i < _tree.Branches.Count; i++)
            {
                _toRegister.AddRange(_tree.Branches[i].ConvertAll(item => item.Value.Name));
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

        #region properties
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
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.RobotJointPosition_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9EC1CBB1-3D33-4DF0-9335-651AD30BC0F6"); }
        }
        #endregion

        #region menu items
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
                        _tree.Branches[0][i].Value.Name = _tree.Branches[0][i].Value.Name + "_" + i.ToString();
                    }
                }

            }

            else if (check == true)
            {
                // Multiple branches with only one item per branch
                for (int i = 0; i < _tree.Branches.Count; i++)
                {
                    _tree.Branches[i][0].Value.Name = _tree.Branches[i][0].Value.Name + "_" + i.ToString();
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
                        _tree.Branches[i][j].Value.Name = _tree.Branches[i][j].Value.Name + "_" + pathString + "_" + j;
                    }
                }
            }
        }
        #endregion
    }
}
