// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Gh.Goos.Actions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Speed Data component. An inherent from the GH_Component Class.
    /// </summary>
    public class SpeedDataComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public SpeedDataComponent()
          : base("Speed Data", "SD", 
              "Defines a speed data declaration for Move components."
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
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name of the Speed Data as text", GH_ParamAccess.tree, "default_speed");
            pManager.AddNumberParameter("TCP Velocity", "vTCP", "TCP Velocity in mm/s as number", GH_ParamAccess.tree);
            pManager.AddNumberParameter("ORI Velocity", "vORI", "Reorientation Velocity of the tool in degree/s as number", GH_ParamAccess.tree, 500);
            pManager.AddNumberParameter("LEAX Velocity", "vLEAX", "Linear External Axes Velocity in mm/s", GH_ParamAccess.tree, 5000);
            pManager.AddNumberParameter("REAX Velocity", "vREAX", "Reorientation of the External Rotational Axes in degrees/s", GH_ParamAccess.tree, 1000);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new SpeedDataParameter(), "Speed Data", "SD", "Resulting Speed Data declaration");
        }

        // Fields
        private readonly List<string> _speedDataNames = new List<string>();
        private string _lastName = "";
        private bool _namesUnique;
        private GH_Structure<GH_SpeedData> _tree = new GH_Structure<GH_SpeedData>();
        private List<GH_SpeedData> _list = new List<GH_SpeedData>();
        private ObjectManager _objectManager;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Sets inputs and creates speeddatas
            GH_Structure<GH_String> names;
            GH_Structure<GH_Number> v_tcps;
            GH_Structure<GH_Number> v_oris;
            GH_Structure<GH_Number> v_leaxs;
            GH_Structure<GH_Number> v_reaxs;

            // Catch the input data
            if (!DA.GetDataTree(0, out names)) { return; }
            if (!DA.GetDataTree(1, out v_tcps)) { return; }
            if (!DA.GetDataTree(2, out v_oris)) { return; }
            if (!DA.GetDataTree(3, out v_leaxs)) { return; }
            if (!DA.GetDataTree(4, out v_reaxs)) { return; }

            // Clear tree and list
            _tree = new GH_Structure<GH_SpeedData>();
            _list = new List<GH_SpeedData>();

            // Create the datatree structure with an other component (in the background, this component is not placed on the canvas)
            SpeedDataComponentDataTreeGenerator component = new SpeedDataComponentDataTreeGenerator();

            component.Params.Input[0].AddVolatileDataTree(names);
            component.Params.Input[1].AddVolatileDataTree(v_tcps);
            component.Params.Input[2].AddVolatileDataTree(v_oris);
            component.Params.Input[3].AddVolatileDataTree(v_leaxs);
            component.Params.Input[4].AddVolatileDataTree(v_reaxs);

            component.ExpireSolution(true);
            component.Params.Output[0].CollectData();

            _tree = component.Params.Output[0].VolatileData as GH_Structure<GH_SpeedData>;

            // Update the variable names in the data trees
            UpdateVariableNames();

            // Make a list
            for (int i = 0; i < _tree.Branches.Count; i++)
            {
                _list.AddRange(_tree.Branches[i]);
            }

            // Sets Output
            DA.SetDataTree(0, _tree);

            #region Object manager
            // Gets ObjectManager of this document
            _objectManager = DocumentManager.GetDocumentObjectManager(this.OnPingDocument());

            // Clears speedDataNames
            for (int i = 0; i < _speedDataNames.Count; i++)
            {
                _objectManager.SpeedDataNames.Remove(_speedDataNames[i]);
            }
            _speedDataNames.Clear();

            // Removes lastName from speedDataNameList
            if (_objectManager.SpeedDataNames.Contains(_lastName))
            {
                _objectManager.SpeedDataNames.Remove(_lastName);
            }

            // Adds Component to SpeedDataByGuid Dictionary
            if (!_objectManager.SpeedDatasByGuid.ContainsKey(this.InstanceGuid))
            {
                _objectManager.SpeedDatasByGuid.Add(this.InstanceGuid, this);
            }

            // Checks if speed Data name is already in use and counts duplicates
            #region Check name in the object manager
            _namesUnique = true;
            for (int i = 0; i < _list.Count; i++)
            {
                if (_objectManager.SpeedDataNames.Contains(_list[i].Value.Name))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Speed Data Name already in use.");
                    _namesUnique = false;
                    _lastName = "";
                }
                else
                {
                    // Adds Speed Data Name to list
                    _speedDataNames.Add(_list[i].Value.Name);
                    _objectManager.SpeedDataNames.Add(_list[i].Value.Name);

                    // Run SolveInstance on other Speed Data with no unique Name to check if their name is now available
                    _objectManager.UpdateSpeedDatas();

                    _lastName = _list[i].Value.Name;
                }

                // Check variable name: character limit
                if (HelperMethods.VariableExeedsCharacterLimit32(_list[i].Value.Name))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Speed Data Name exceeds character limit of 32 characters.");
                    break;
                }

                // Check variable name: start with number is not allowed
                if (HelperMethods.VariableStartsWithNumber(_list[i].Value.Name))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Speed Data Name starts with a number which is not allowed in RAPID Code.");
                    break;
                }
            }
            #endregion

            // Recognizes if Component is Deleted and removes it from Object Managers speed Data and name list
            GH_Document doc = this.OnPingDocument();
            if (doc != null)
            {
                doc.ObjectsDeleted += DocumentObjectsDeleted;
            }
            #endregion
        }

        /// <summary>
        /// Detect if the components gets removed from the canvas and deletes the 
        /// objects created with this components from the object manager. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void DocumentObjectsDeleted(object sender, GH_DocObjectEventArgs e)
        {
            if (e.Objects.Contains(this))
            {
                if (_namesUnique == true)
                {
                    for (int i = 0; i < _speedDataNames.Count; i++)
                    {
                        _objectManager.SpeedDataNames.Remove(_speedDataNames[i]);
                    }
                }
                _objectManager.SpeedDatasByGuid.Remove(this.InstanceGuid);

                // Run SolveInstance on other Speed Data instances with no unique Name to check if their name is now available
                _objectManager.UpdateSpeedDatas();
            }
        }

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

        /// <summary>
        /// The Speed Datas created by this component
        /// </summary>
        public List<SpeedData> SpeedDatas
        {
            get { return _list.ConvertAll(item => item.Value); }
        }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName
        {
            get { return _lastName; }
        }

        #region menu item methods
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

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.SpeedData_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("91296EEB-3DA3-4F9F-8516-398BD369795E"); }
        }
    }
}

