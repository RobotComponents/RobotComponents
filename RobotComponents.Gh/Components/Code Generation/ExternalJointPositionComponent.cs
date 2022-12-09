// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Gh.Goos.Actions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Ext Joint Position component. An inherent from the GH_Component Class.
    /// </summary>
    public class ExternalJointPositionComponent : GH_Component, IGH_VariableParameterComponent, IObjectManager
    {
        #region fields
        private GH_Structure<GH_ExternalJointPosition> _tree = new GH_Structure<GH_ExternalJointPosition>();
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
        public ExternalJointPositionComponent()
          : base("External Joint Position", "EJ",
              "Defines an External Joint Position for a Robot Target or Joint Target declaration."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
            // Create the component label with a message
            Message = "EXTENDABLE";
        }

        /// <summary>
        /// Stores the variable input parameters in an array.
        /// </summary>
        private readonly IGH_Param[] externalAxisParameters = new IGH_Param[6]
        {
            new Param_Number() { Name = "External joint position A", NickName = "EJa", Description = "Defines the position of external logical axis A", Access = GH_ParamAccess.item, Optional = true }, // fixed
            new Param_Number() { Name = "External joint position B", NickName = "EJb", Description = "Defines the position of external logical axis B", Access = GH_ParamAccess.item, Optional = true }, // fixed
            new Param_Number() { Name = "External joint position C", NickName = "EJc", Description = "Defines the position of external logical axis C", Access = GH_ParamAccess.item, Optional = true }, // variable
            new Param_Number() { Name = "External joint position D", NickName = "EJd", Description = "Defines the position of external logical axis D", Access = GH_ParamAccess.item, Optional = true }, // variable
            new Param_Number() { Name = "External joint position E", NickName = "EJe", Description = "Defines the position of external logical axis E", Access = GH_ParamAccess.item, Optional = true }, // variable
            new Param_Number() { Name = "External joint position F", NickName = "EJf", Description = "Defines the position of external logical axis F", Access = GH_ParamAccess.item, Optional = true } // variable
        };

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name as text", GH_ParamAccess.item, string.Empty);
            pManager.AddNumberParameter(externalAxisParameters[0].Name, externalAxisParameters[0].NickName, externalAxisParameters[0].Description, externalAxisParameters[0].Access, 9e9);
            pManager.AddNumberParameter(externalAxisParameters[1].Name, externalAxisParameters[1].NickName, externalAxisParameters[1].Description, externalAxisParameters[1].Access, 9e9);

            pManager[1].Optional = externalAxisParameters[0].Optional;
            pManager[2].Optional = externalAxisParameters[1].Optional;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_ExternalJointPosition(), "External Joint Position", "EJ", "The resulting external joint position");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            string name = string.Empty;
            double externalJointPositionA = 9e9;
            double externalJointPositionB = 9e9;
            double externalJointPositionC = 9e9;
            double externalJointPositionD = 9e9;
            double externalJointPositionE = 9e9;
            double externalJointPositionF = 9e9;

            // Catch input data
            if (!DA.GetData(0, ref name)) { return; }

            // External joint position A
            if (Params.Input.Any(x => x.Name == externalAxisParameters[0].Name))
            {
                if (!DA.GetData(externalAxisParameters[0].Name, ref externalJointPositionA)) { externalJointPositionA = 9e9; }
            }
            // External joint position B
            if (Params.Input.Any(x => x.Name == externalAxisParameters[1].Name))
            {
                if (!DA.GetData(externalAxisParameters[1].Name, ref externalJointPositionB)) { externalJointPositionB = 9e9; }
            }
            // External joint position C
            if (Params.Input.Any(x => x.Name == externalAxisParameters[2].Name))
            {
                if (!DA.GetData(externalAxisParameters[2].Name, ref externalJointPositionC)) { externalJointPositionC = 9e9; }
            }
            // External joint position D
            if (Params.Input.Any(x => x.Name == externalAxisParameters[3].Name))
            {
                if (!DA.GetData(externalAxisParameters[3].Name, ref externalJointPositionD)) { externalJointPositionD = 9e9; }
            }
            // External joint position E
            if (Params.Input.Any(x => x.Name == externalAxisParameters[4].Name))
            {
                if (!DA.GetData(externalAxisParameters[4].Name, ref externalJointPositionE)) { externalJointPositionE = 9e9; }
            }
            // External joint position F
            if (Params.Input.Any(x => x.Name == externalAxisParameters[5].Name))
            {
                if (!DA.GetData(externalAxisParameters[5].Name, ref externalJointPositionF)) { externalJointPositionF = 9e9; }
            }

            // Create external joint position
            ExternalJointPosition externalJointPosition = new ExternalJointPosition(name, externalJointPositionA, externalJointPositionB, externalJointPositionC, externalJointPositionD, externalJointPositionE, externalJointPositionF);

            // Sets Output
            DA.SetData(0, externalJointPosition);
        }

        /// <summary>
        /// Override this method if you want to be called after the last call to SolveInstance.
        /// </summary>
        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();

            _tree = this.Params.Output[0].VolatileData as GH_Structure<GH_ExternalJointPosition>;

            if (_tree.Branches.Count != 0)
            {
                if (_tree.Branches[0][0].Value.Name != string.Empty)
                {
                    UpdateVariableNames();
                }

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
            get { return Properties.Resources.ExternalJointPosition_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D405C21A-AEA9-45D5-A987-F87CAA163AC6"); }
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

        #region variable input parameters
        /// <summary>
        /// This function needs to be called to add an input parameter to override the external axis value. 
        /// </summary>
        /// <param name="index"> The index number of the variable input parameter that needs to be added.
        /// In this case the index number of the array with variable input parameters. </param>
        private void AddExternalAxisValueParameter(int index)
        {
            // Pick the parameter that needs to be added
            IGH_Param parameter = externalAxisParameters[index - 1];

            // Register the input parameter
            Params.RegisterInputParam(parameter, index);

            // Refresh parameters since they changed
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        /// <summary>
        /// This function will get called before an attempt is made to insert a parameter. 
        /// Since this method is potentially called on Canvas redraws, it must be fast.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if your component supports a variable parameter at the given location </returns>
        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            // Don't allow for insert before or in between the fixed input parameters
            if (side == GH_ParameterSide.Input && index < 3)
            {
                return false;
            }
            // Don't allow for insert if all variable input parameters are already added
            else if (side == GH_ParameterSide.Input && index == (externalAxisParameters.Length + 1))
            {
                return false;
            }
            // Allow insert after the last input parameters
            else if (side == GH_ParameterSide.Input && index == Params.Input.Count)
            {
                return true;
            }
            // Don't allow for inserting new output parameters
            else
            {
                return false;
            }
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
            // If the first external axis override parameter is added it is allowed to remove parameters
            if (Params.Input.Any(x => x.Name == externalAxisParameters[0].Name))
            {
                // Makes it impossible to remove the fixed input parameters
                if (side == GH_ParameterSide.Input && index < 3)
                {
                    return false;
                }
                // Makes it possible to remove the last variable input parameter
                else if (side == GH_ParameterSide.Input && index == Params.Input.Count - 1)
                {
                    return true;
                }
                // Makes it impossible to remove all the other input and output parameters
                else
                {
                    return false;
                }
            }

            else
            {
                return false;
            }
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
            // Add input parameter
            AddExternalAxisValueParameter(index);

            // This method always returns a null value
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
            // If the first external axis is added it is allowed to destroy input parameters
            if (Params.Input.Any(x => x.Name == externalAxisParameters[0].Name))
            {
                // Makes it impossible to detroy the fixed input parameters
                if (side == GH_ParameterSide.Input && index < 3)
                {
                    return false;
                }

                // Makes it impossible to destroy the output parameters
                else if (side == GH_ParameterSide.Output)
                {
                    return false;
                }

                // Makes it possible to destroy all the other parameters
                else
                {
                    return true;
                }
            }

            else
            {
                return false;
            }
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
