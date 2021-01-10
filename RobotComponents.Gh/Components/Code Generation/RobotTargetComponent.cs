// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Gh.Goos.Actions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Target component. An inherent from the GH_Component Class.
    /// </summary>
    public class RobotTargetComponent : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public RobotTargetComponent()
          : base("Robot Target", "RT",
              "Defines a Robot Target declaration for an Instruction : Movement or Inverse Kinematics component."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
            // Create the component label with a message
            Message = "EXTENDABLE";
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
            pManager.AddPlaneParameter("Plane", "P", "Plane as Plane", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Axis Configuration", "AC", "Axis Configuration as int. This will modify the fourth value of the Robot Configuration Data in the RAPID Movement code line.", GH_ParamAccess.tree, 0);
        }

        // Register the number of fixed input parameters
        private readonly int fixedParamNumInput = 3;

        // Create an array with the variable input parameters
        readonly IGH_Param[] parameters = new IGH_Param[2]
        {
            new Param_Plane() { Name = "Reference Plane", NickName = "RP",  Description = "Reference Plane as a Plane", Access = GH_ParamAccess.tree, Optional = true },
            new ExternalJointPositionParameter() { Name = "External Joint Position", NickName = "EJ", Description = "The resulting external joint position", Access = GH_ParamAccess.tree, Optional = true }
    };

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new RobotTargetParameter(), "Robot Target", "RT", "Resulting Robot Target");
        }

        // Fields
        private readonly List<string> _targetNames = new List<string>();
        private string _lastName = "";
        private bool _namesUnique;
        private GH_Structure<GH_RobotTarget> _tree = new GH_Structure<GH_RobotTarget>();
        private List<GH_RobotTarget> _list = new List<GH_RobotTarget>();
        private ObjectManager _objectManager;

        private bool _setReferencePlane = false;
        private bool _setExternalJointPosition = false;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Sets inputs and creates target
            GH_Structure<GH_String> names;
            GH_Structure<GH_Plane> planes;
            GH_Structure<GH_Plane> referencePlanes = new GH_Structure<GH_Plane>();
            GH_Structure<GH_Integer> axisConfigs = new GH_Structure<GH_Integer>();
            GH_Structure<GH_ExternalJointPosition> externalJointPositions = new GH_Structure<GH_ExternalJointPosition>();

            // Catch inputs
            if (!DA.GetDataTree(0, out names)) { return; } // Fixed index
            if (!DA.GetDataTree(1, out planes)) { return; } // Fixed index
            if (Params.Input.Any(x => x.Name == parameters[0].Name)) 
            {            
                if (!DA.GetDataTree(parameters[0].Name, out referencePlanes)) { return; }
            }
            if (Params.Input.Any(x => x.Name == "Axis Configuration"))
            {
                if (!DA.GetDataTree("Axis Configuration", out axisConfigs)) { return; }
            }
            if (Params.Input.Any(x => x.Name == parameters[1].Name))
            {
                if (!DA.GetDataTree(parameters[1].Name, out externalJointPositions)) { return; }
            }

            // Inputs shoud not be empty
            if (referencePlanes.Branches.Count == 0)
            {
                referencePlanes.Append(new GH_Plane(Plane.WorldXY), new GH_Path(0));
            }
            if (axisConfigs.Branches.Count == 0)
            {
                axisConfigs.Append(new GH_Integer(0), new GH_Path(0));
            }
            if (externalJointPositions.Branches.Count == 0)
            {
                externalJointPositions.Append(new GH_ExternalJointPosition(new ExternalJointPosition()), new GH_Path(0));
            }

            // Clear tree and list
            _tree = new GH_Structure<GH_RobotTarget>();
            _list = new List<GH_RobotTarget>();

            // Create the datatree structure with an other component (in the background, this component is not placed on the canvas)
            RobotTargetComponentDataTreeGenerator component = new RobotTargetComponentDataTreeGenerator();

            component.Params.Input[0].AddVolatileDataTree(names);
            component.Params.Input[1].AddVolatileDataTree(planes);
            component.Params.Input[2].AddVolatileDataTree(referencePlanes);
            component.Params.Input[3].AddVolatileDataTree(axisConfigs);
            component.Params.Input[4].AddVolatileDataTree(externalJointPositions);

            component.ExpireSolution(true);
            component.Params.Output[0].CollectData();

            _tree = component.Params.Output[0].VolatileData as GH_Structure<GH_RobotTarget>;

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
            if (!_objectManager.RobotTargetsByGuid.ContainsKey(this.InstanceGuid))
            {
                _objectManager.RobotTargetsByGuid.Add(this.InstanceGuid, this); //TODO
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
                _objectManager.RobotTargetsByGuid.Remove(this.InstanceGuid);

                // Run SolveInstance on other Targets with no unique Name to check if their name is now available
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
        public List<RobotTarget> RobotTargets
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
        /// Boolean that indicates if the custom menu item for setting the Reference Plane is checked
        /// </summary>
        public bool SetReferencePlane
        {
            get { return _setReferencePlane; }
            set { _setReferencePlane = value; }
        }

        /// <summary>
        /// Boolean that indicates if the custom menu item for setting the External Joint Position
        /// </summary>
        public bool SetExternalJointPosition
        {
            get { return _setExternalJointPosition; }
            set { _setExternalJointPosition = value; }
        }

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Set Reference Plane", SetReferencePlane);
            writer.SetBoolean("Set External Joint Position", SetExternalJointPosition);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            SetReferencePlane = reader.GetBoolean("Set Reference Plane");
            SetExternalJointPosition = reader.GetBoolean("Set External Joint Position");
            return base.Read(reader);
        }

        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Reference Plane", MenuItemClickReferencePlane, true, SetReferencePlane);
            Menu_AppendItem(menu, "External Joint Position", MenuItemClickExternalJointPosition, true, SetExternalJointPosition);
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

        /// <summary>
        /// Handles the event when the custom menu item "Reference Plane" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickReferencePlane(object sender, EventArgs e)
        {
            // Change bool
            RecordUndoEvent("Set Reference Plane");
            SetReferencePlane = !SetReferencePlane;

            // Input parameter
            IGH_Param parameter = parameters[0];

            // If the parameter already exist: unregister it
            if (Params.Input.Any(x => x.Name == parameter.Name))
            {
                // Unregister the parameter
                Params.UnregisterInputParameter(Params.Input.First(x => x.Name == parameter.Name), true);
            }

            // Else add the reference plane parameter
            else
            {
                // The index where the parameter should be added
                int index = 2;

                // Register the input parameter
                Params.RegisterInputParam(parameters[0], index);
            }

            // Expire solution and refresh parameters since they changed
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        /// <summary>
        /// Registers the event when the custom menu item "External Joint Position" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickExternalJointPosition(object sender, EventArgs e)
        {
            // Change bool
            RecordUndoEvent("Set External Joint Position");
            SetExternalJointPosition = !SetExternalJointPosition;

            // Input parameter
            IGH_Param parameter = parameters[1];

            // If the parameter already exist: unregister it
            if (Params.Input.Any(x => x.Name == parameter.Name))
            {
                // Unregister the parameter
                Params.UnregisterInputParameter(Params.Input.First(x => x.Name == parameter.Name), true);
            }

            // Else add the reference plane parameter
            else
            {
                // The index where the parameter should be added
                int index = fixedParamNumInput;

                // Correction for the index number if the reference place was already added
                if (Params.Input.Any(x => x.Name == "Reference Plane"))
                {
                    index += 1;
                }

                // Register the input parameter
                Params.RegisterInputParam(parameter, index);
            }

            // Refresh parameters since they changed
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        #endregion

        // Methods of variable parameter interface which handles (de)serialization of the variable input parameters
        #region variable input parameters
        /// <summary>
        /// This function will get called before an attempt is made to insert a parameter. 
        /// Since this method is potentially called on Canvas redraws, it must be fast.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if your component supports a variable parameter at the given location </returns>
        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        /// <summary>
        /// This function will get called before an attempt is made to insert a parameter. 
        /// Since this method is potentially called on Canvas redraws, it must be fast.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if your component supports a variable parameter at the given location. </returns>
        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        /// <summary>
        /// This function will be called when a new parameter is about to be inserted. 
        /// You must provide a valid parameter or insertion will be skipped. 
        /// You do not, repeat not, need to insert the parameter yourself.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> A valid IGH_Param instance to be inserted. In our case a null value. </returns>
        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
        {
            return null;
        }

        /// <summary>
        /// This function will be called when a parameter is about to be removed. 
        /// You do not need to do anything, but this would be a good time to remove 
        /// any event handlers that might be attached to the parameter in question.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if the parameter in question can indeed be removed. Note, this is only in emergencies, 
        /// typically the CanRemoveParameter function should return false if the parameter in question is not removable. </returns>
        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        /// <summary>
        /// This method will be called when a closely related set of variable parameter operations completes. 
        /// This would be a good time to ensure all Nicknames and parameter properties are correct. 
        /// This method will also be called upon IO operations such as Open, Paste, Undo and Redo.
        /// </summary>
        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {

        }
        #endregion

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.RobTarget_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("EA79575D-5AED-46F2-8E50-A00BF5B65620"); }
        }

    }
}
