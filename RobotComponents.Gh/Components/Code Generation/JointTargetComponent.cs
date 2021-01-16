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
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Gh.Goos.Actions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Joint Target component. An inherent from the GH_Component Class.
    /// </summary>
    public class JointTargetComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public JointTargetComponent()
          : base("Joint Target", "JT",
              "Defines a Joint Target for a Move instruction."
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
            pManager.AddTextParameter("Name", "N", "Name as text", GH_ParamAccess.tree, "defaultTar");
            pManager.AddParameter(new RobotJointPositionParameter(), "Robot Joint Position", "RJ", "Defines the robot joint position", GH_ParamAccess.tree);
            pManager.AddParameter(new ExternalJointPositionParameter(), "External Joint Position", "EJ", "Defines the external axis joint position", GH_ParamAccess.tree);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new JointTargetParameter(), "Joint Target", "JT", "The resulting Joint Target");
        }

        // Fields
        private readonly List<string> _targetNames = new List<string>();
        private string _lastName = "";
        private bool _namesUnique;
        private GH_Structure<GH_JointTarget> _tree = new GH_Structure<GH_JointTarget>();
        private List<GH_JointTarget> _list = new List<GH_JointTarget>();
        private ObjectManager _objectManager;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Variables
            GH_Structure<GH_String> names;
            GH_Structure<GH_RobotJointPosition> robotJointPositions;
            GH_Structure<GH_ExternalJointPosition> externalJointPositions;

            // Catch input data
            if (!DA.GetDataTree(0, out names)) { return; }
            if (!DA.GetDataTree(1, out robotJointPositions)) { robotJointPositions = new GH_Structure<GH_RobotJointPosition>(); }
            if (!DA.GetDataTree(2, out externalJointPositions)) { externalJointPositions = new GH_Structure<GH_ExternalJointPosition>(); }

            // Clear tree and list
            _tree = new GH_Structure<GH_JointTarget>();
            _list = new List<GH_JointTarget>();

            // Create the datatree structure with an other component (in the background, this component is not placed on the canvas)
            JointTargetComponentDataTreeGenerator component = new JointTargetComponentDataTreeGenerator();

            component.Params.Input[0].AddVolatileDataTree(names);
            component.Params.Input[1].AddVolatileDataTree(robotJointPositions);
            component.Params.Input[2].AddVolatileDataTree(externalJointPositions);

            component.ExpireSolution(true);
            component.Params.Output[0].CollectData();

            _tree = component.Params.Output[0].VolatileData as GH_Structure<GH_JointTarget>;

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

            // Clears targetNames
            for (int i = 0; i < _targetNames.Count; i++)
            {
                _objectManager.TargetNames.Remove(_targetNames[i]);
            }
            _targetNames.Clear();

            // Removes lastName from targetNameList
            if (_objectManager.TargetNames.Contains(_lastName))
            {
                _objectManager.TargetNames.Remove(_lastName);
            }

            // Adds Component to TargetByGuid Dictionary
            if (!_objectManager.JointTargetsByGuid.ContainsKey(this.InstanceGuid))
            {
                _objectManager.JointTargetsByGuid.Add(this.InstanceGuid, this);
            }

            // Checks if target name is already in use and counts duplicates
            #region Check name in object manager
            _namesUnique = true;
            for (int i = 0; i < _list.Count; i++)
            {
                if (_objectManager.TargetNames.Contains(_list[i].Value.Name))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Target Name already in use.");
                    _namesUnique = false;
                    _lastName = "";
                    break;
                }
                else
                {
                    // Adds Target Name to list
                    _targetNames.Add(_list[i].Value.Name);
                    _objectManager.TargetNames.Add(_list[i].Value.Name);

                    // Run SolveInstance on other Targets with no unique Name to check if their name is now available
                    _objectManager.UpdateTargets();

                    _lastName = _list[i].Value.Name;
                }

                // Checks if variable name exceeds max character limit for RAPID Code
                if (HelperMethods.VariableExeedsCharacterLimit32(_list[i].Value.Name))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Target Name exceeds character limit of 32 characters.");
                    break;
                }

                // Checks if variable name starts with a number
                if (HelperMethods.VariableStartsWithNumber(_list[i].Value.Name))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Target Name starts with a number which is not allowed in RAPID Code.");
                    break;
                }
            }
            #endregion

            // Recognizes if Component is Deleted and removes it from Object Managers target and name list
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
                    for (int i = 0; i < _targetNames.Count; i++)
                    {
                        _objectManager.TargetNames.Remove(_targetNames[i]);
                    }
                }
                _objectManager.JointTargetsByGuid.Remove(this.InstanceGuid);

                // Runs SolveInstance on all other Targets to check if robot tool names are unique.
                _objectManager.UpdateTargets();
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
        /// The Targets created by this component
        /// </summary>
        public List<JointTarget> JointTargets
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

        // Methods for creating custom menu items and event handlers when the custom menu items are clicked
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

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.JointTarget_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("AC253421-ADB3-4FAA-AC55-E24BDF86F110"); }
        }

    }
}
