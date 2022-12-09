// This file is part of RobotComponents. RobotComponents is licensed under 
// under the terms of GNU Lesser General Public License (LGPL
// published by the 
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
// RobotComponents Libs
using RobotComponents.Gh.Goos.Actions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Action : Zone Data component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class ZoneDataComponent_OBSOLETE : GH_Component, IObjectManager
    {
        #region fields
        private GH_Structure<GH_ZoneData> _tree = new GH_Structure<GH_ZoneData>();
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
        public ZoneDataComponent_OBSOLETE()
          : base("Zone Data", "ZD",
              "Defines a zone data declaration for robot movements in RAPID program code generation."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name of the Zone Data as text", GH_ParamAccess.tree, string.Empty);
            pManager.AddBooleanParameter("Fine Point", "FP", "Defines whether the movement is to terminate as a stop point (fine point) or as a fly-by point as a bool.", GH_ParamAccess.tree, false);
            pManager.AddNumberParameter("Path Zone TCP", "pzTCP", "The size (the radius) of the TCP zone in mm as a number.", GH_ParamAccess.tree, 0);
            pManager.AddNumberParameter("Path Zone Reorientation", "pzORI", "The zone size (the radius) for the tool reorientation in mm as a number. ", GH_ParamAccess.tree, 0);
            pManager.AddNumberParameter("Path Zone External Axes", "pzEA", "The zone size (the radius) for external axes in mm as a number.", GH_ParamAccess.tree, 0);
            pManager.AddNumberParameter("Zone Reorientation", "zORI", "The zone size for the tool reorientation in degrees as a number.", GH_ParamAccess.tree, 0);
            pManager.AddNumberParameter("Zone External Linear Axes", "zELA", "The zone size for linear external axes in mm as a number.", GH_ParamAccess.tree, 0);
            pManager.AddNumberParameter("Zone External Rotational Axes", "zERA", "The zone size for rotating external axes in degrees as a number.", GH_ParamAccess.tree, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_ZoneData(), "Zone Data", "ZD", "Resulting Zone Data declaration");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Sets inputs
            GH_Structure<GH_String> names;
            GH_Structure<GH_Boolean> fineps;
            GH_Structure<GH_Number> pzone_tcps;
            GH_Structure<GH_Number> pzone_oris;
            GH_Structure<GH_Number> pzone_eaxs;
            GH_Structure<GH_Number> zone_oris;
            GH_Structure<GH_Number> zone_leaxs;
            GH_Structure<GH_Number> zone_reaxs;

            // Catch the input data
            if (!DA.GetDataTree(0, out names)) { return; }
            if (!DA.GetDataTree(1, out fineps)) { return; }
            if (!DA.GetDataTree(2, out pzone_tcps)) { return; }
            if (!DA.GetDataTree(3, out pzone_oris)) { return; }
            if (!DA.GetDataTree(4, out pzone_eaxs)) { return; }
            if (!DA.GetDataTree(5, out zone_oris)) { return; }
            if (!DA.GetDataTree(6, out zone_leaxs)) { return; }
            if (!DA.GetDataTree(7, out zone_reaxs)) { return; }

            // Clear tree
            _tree = new GH_Structure<GH_ZoneData>();

            // Create the datatree structure with an other component (in the background, this component is not placed on the canvas)
            ZoneDataComponentDataTreeGenerator_OBSOLETE component = new ZoneDataComponentDataTreeGenerator_OBSOLETE();

            component.Params.Input[0].AddVolatileDataTree(names);
            component.Params.Input[1].AddVolatileDataTree(fineps);
            component.Params.Input[2].AddVolatileDataTree(pzone_tcps);
            component.Params.Input[3].AddVolatileDataTree(pzone_oris);
            component.Params.Input[4].AddVolatileDataTree(pzone_eaxs);
            component.Params.Input[5].AddVolatileDataTree(zone_oris);
            component.Params.Input[6].AddVolatileDataTree(zone_leaxs);
            component.Params.Input[7].AddVolatileDataTree(zone_reaxs);

            component.ExpireSolution(true);
            component.Params.Output[0].CollectData();

            _tree = component.Params.Output[0].VolatileData as GH_Structure<GH_ZoneData>;

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
            get { return Properties.Resources.ZoneData_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("E332392C-8C73-418A-9E52-A0DA5EE19377"); }
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

