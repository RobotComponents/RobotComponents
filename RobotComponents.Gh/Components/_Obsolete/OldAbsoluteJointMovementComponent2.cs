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
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GH_IO.Serialization;
// Robot Components Libs
using RobotComponents.Actions;
using RobotComponents.Definitions;
using RobotComponents.Gh.Parameters.Obsolete;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Gh.Utils;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.10.000

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Action : Absolute Joint Movement component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldAbsoluteJointMovementComponent2 : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, subcategory the panel. 
        /// If you use non-existing tab or panel names new tabs/panels will automatically be created.
        /// </summary>
        public OldAbsoluteJointMovementComponent2()
          : base("Absolute Joint Movement", "AJM",
              "Defines an Aboslute Joint Movement instruction."
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
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name of the joint target as text value", GH_ParamAccess.list, new List<string> { "default" });
            pManager.AddNumberParameter("Internal Axis Values", "IAV", "Internal Axis Values as datatree with numbers", GH_ParamAccess.tree, new List<double> { 0, 0, 0, 0, 0, 0 });
            pManager.AddNumberParameter("External Axis Values", "EAV", "External Axis Values as datatree with numbers", GH_ParamAccess.tree);
            pManager.AddParameter(new SpeedDataParameter(), "Speed Data", "SD", "Speed Data as Speed Data or as a number (vTCP)", GH_ParamAccess.list);
            pManager.AddParameter(new ZoneDataParameter(), "Zone Data", "ZD", "Zone Data as Zone Data or as a number (path zone TCP)", GH_ParamAccess.list);

            pManager[2].Optional = true;
            pManager[4].Optional = true;
        }

        // Register the number of fixed input parameters
        private readonly int fixedParamNumInput = 5;

        // Create an array with the variable input parameters
        readonly IGH_Param[] variableInputParameters = new IGH_Param[1]
        {
            new RobotToolParameter() { Name = "Robot Tool", NickName = "RT", Description = "Robot Tool as as list", Access = GH_ParamAccess.list, Optional = true}
        };

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new OldAbsoluteJointMovementParameter(), "Absolute Joint Movement", "AJM", "Resulting Absolute Joint Movement instruction", GH_ParamAccess.list);  //Todo: beef this up to be more informative.
        }

        // Fields
        private bool _overrideRobotTool = false;
        private readonly List<string> _targetNames = new List<string>();
        private string _lastName = "";
        private bool _namesUnique;
        private ObjectManager _objectManager;
        private List<AbsoluteJointMovement> _jointMovements = new List<AbsoluteJointMovement>();

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed in the future. Instead, " +
                "combine Joint Targets with Movements.");

            // Input variables
            List<string> names = new List<string>();
            GH_Structure<GH_Number> internalAxisValuesTree = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> externalAxisValuesTree = new GH_Structure<GH_Number>();
            List<SpeedData> speedDatas = new List<SpeedData>();
            List<ZoneData> zoneDatas = new List<ZoneData>();
            List<RobotTool> robotTools = new List<RobotTool>();

            // Create an empty Robot Tool
            RobotTool emptyRobotTool = new RobotTool();
            emptyRobotTool.Clear();

            // Catch the input data from the fixed parameters
            if (!DA.GetDataList(0, names)) { return; }
            if (!DA.GetDataTree(1, out internalAxisValuesTree)) { return; }
            if (!DA.GetDataTree(2, out externalAxisValuesTree)) { return; }
            if (!DA.GetDataList(3, speedDatas)) { return; }
            if (!DA.GetDataList(4, zoneDatas)) { zoneDatas = new List<ZoneData>() { new ZoneData(0) }; }

            // Catch the input data from the variable parameteres
            if (Params.Input.Any(x => x.Name == variableInputParameters[0].Name))
            {
                if (!DA.GetDataList(variableInputParameters[0].Name, robotTools))
                {
                    robotTools = new List<RobotTool>() { new RobotTool(emptyRobotTool) };
                }
            }

            // Make sure variable input parameters have a default value
            if (robotTools.Count == 0)
            {
                robotTools.Add(new RobotTool(emptyRobotTool)); // Empty Robot Tool
            }

            // Get longest Input List
            int[] sizeValues = new int[6];
            sizeValues[0] = names.Count;
            sizeValues[1] = internalAxisValuesTree.PathCount;
            sizeValues[2] = externalAxisValuesTree.PathCount;
            sizeValues[3] = speedDatas.Count;
            sizeValues[4] = zoneDatas.Count;
            sizeValues[5] = robotTools.Count;

            int biggestSize = HelperMethods.GetBiggestValue(sizeValues);

            // Keeps track of used indicies
            int namesCounter = -1;
            int internalValueCounter = -1;
            int externalValueCounter = -1;
            int speedDataCounter = -1;
            int precisionCounter = -1;
            int robotToolCounter = -1;

            // Clear list
            _jointMovements.Clear();

            // Creates movements
            for (int i = 0; i < biggestSize; i++)
            {
                string name;
                List<double> internalAxisValues = new List<double>();
                List<double> externalAxisValues = new List<double>();

                SpeedData speedData;
                ZoneData zoneData;
                RobotTool robotTool;

                // Target counter
                if (i < sizeValues[0])
                {
                    name = names[i];
                    namesCounter++;
                }
                else
                {
                    name = names[namesCounter] + "_" + (i - namesCounter);
                }

                // internal axis values counter
                if (i < sizeValues[1])
                {
                    internalAxisValues = internalAxisValuesTree[i].ConvertAll(x => (double)x.Value);
                    internalValueCounter++;
                }
                else
                {
                    internalAxisValues = internalAxisValuesTree[internalValueCounter].ConvertAll(x => (double)x.Value);
                }

                // External axis values counter
                if (sizeValues[2] == 0) // In case no external axis values are defined.
                {
                    externalAxisValues = new List<double>() { };
                }

                else
                {
                    if (i < sizeValues[2])
                    {
                        externalAxisValues = externalAxisValuesTree[i].ConvertAll(x => (double)x.Value);
                        externalValueCounter++;
                    }
                    else
                    {
                        externalAxisValues = externalAxisValuesTree[externalValueCounter].ConvertAll(x => (double)x.Value);
                    }
                }

                // SpeedData counter
                if (i < sizeValues[3])
                {
                    speedData = speedDatas[i];
                    speedDataCounter++;
                }
                else
                {
                    speedData = speedDatas[speedDataCounter];
                }

                // Precision counter
                if (i < sizeValues[4])
                {
                    zoneData= zoneDatas[i];
                    precisionCounter++;
                }
                else
                {
                    zoneData = zoneDatas[precisionCounter];
                }

                // Robot tool counter
                if (i < sizeValues[5])
                {
                    robotTool = robotTools[i];
                    robotToolCounter++;
                }
                else
                {
                    robotTool = robotTools[robotToolCounter];
                }

                // JointMovement constructor
                AbsoluteJointMovement jointMovement = new AbsoluteJointMovement(name, internalAxisValues, externalAxisValues, speedData, zoneData, robotTool);
                _jointMovements.Add(jointMovement);
            }

            // Check if an exact predefined zonedata value is used
            for (int i = 0; i < zoneDatas.Count; i++)
            {
                if (zoneDatas[i].ExactPredefinedValue == false & zoneDatas[i].PreDefinied == true)
                    {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Predefined zonedata value <" + i + "> is invalid. " +
                        "The nearest valid predefined speeddata value is used. Valid predefined zonedata values are -1, " +
                        "0, 1, 5, 10, 15, 20, 30, 40, 50, 60, 80, 100, 150 or 200. " +
                        "A value of -1 will be interpreted as fine movement in RAPID Code.");
                    break;
                }
            }

            // Check if an exact predefined speeddata value is used
            for (int i = 0; i < speedDatas.Count; i++)
            {
                if (speedDatas[i].ExactPredefinedValue == false & speedDatas[i].PreDefinied == true)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Predefined speeddata value <" + i + "> is invalid. " +
                        "The nearest valid predefined speeddata value is used. Valid predefined speeddata values are 5, 10, " +
                        "20, 30, 40, 50, 60, 80, 100, 150, 200, 300, 400, 500, 600, 800, 1000, 1500, 2000, 2500, 3000, 4000, " +
                        "5000, 6000 and 7000.");
                    break;
                }
            }

            // Output
            DA.SetDataList(0, _jointMovements);

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

            // Adds Component to JointTargetsByGuid Dictionary
            if (!_objectManager.OldJointTargetsByGuid2.ContainsKey(this.InstanceGuid))
            {
                _objectManager.OldJointTargetsByGuid2.Add(this.InstanceGuid, this);
            }

            // Checks if target name is already in use and counts duplicates
            #region Check name in object manager
            _namesUnique = true;
            for (int i = 0; i < names.Count; i++)
            {
                if (_objectManager.TargetNames.Contains(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Target Name already in use.");
                    _namesUnique = false;
                    _lastName = "";
                    break;
                }
                else
                {
                    // Adds Target Name to list
                    _targetNames.Add(names[i]);
                    _objectManager.TargetNames.Add(names[i]);

                    // Run SolveInstance on other Targets with no unique Name to check if their name is now available
                    _objectManager.UpdateTargets();

                    _lastName = names[i];
                }

                // Checks if variable name exceeds max character limit for RAPID Code
                if (HelperMethods.VariableExeedsCharacterLimit32(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Target Name exceeds character limit of 32 characters.");
                    break;
                }

                // Checks if variable name starts with a number
                if (HelperMethods.VariableStartsWithNumber(names[i]))
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
                _objectManager.OldJointTargetsByGuid2.Remove(this.InstanceGuid);

                // Run SolveInstance on other Targets with no unique Name to check if their name is now available
                _objectManager.UpdateTargets();
            }
        }

        /// <summary>
        /// The Absolute Joint Movements created by this component
        /// </summary>
        public List<AbsoluteJointMovement> AbsoluteJointMovements
        {
            get { return _jointMovements; }
        }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName
        {
            get { return _lastName; }
        }


        // Methods and properties for creating custom menu items and event handlers when the custom menu items are clicked
        #region menu items
        /// <summary>
        /// Boolean that indicates if the custom menu item for overriding the Robot Tool is checked
        /// </summary>
        public bool OverrideRobotTool
        {
            get { return _overrideRobotTool; }
            set { _overrideRobotTool = value; }
        }

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Override Robot Tool", OverrideRobotTool);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            OverrideRobotTool = reader.GetBoolean("Override Robot Tool");
            return base.Read(reader);
        }

        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Override Robot Tool", MenuItemClickRobotTool, true, OverrideRobotTool);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Robot Tool" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickRobotTool(object sender, EventArgs e)
        {
            RecordUndoEvent("Override Robot Tool");
            OverrideRobotTool = !OverrideRobotTool;
            AddParameter(0);
        }

        /// <summary>
        /// Adds or destroys the input parameter to the component.
        /// </summary>
        /// <param name="index"> The index number of the parameter that needs to be added. </param>
        private void AddParameter(int index)
        {
            // Pick the parameter
            IGH_Param parameter = variableInputParameters[index];
            string name = variableInputParameters[index].Name;

            // If the parameter already exist: remove it
            if (Params.Input.Any(x => x.Name == name))
            {
                Params.UnregisterInputParameter(Params.Input.First(x => x.Name == name), true);
            }

            // Else remove the variable input parameter
            else
            {
                // The index where the parameter should be added
                int insertIndex = fixedParamNumInput;

                // Check if other parameters are already added and correct the insert index
                for (int i = 0; i < index; i++)
                {
                    if (Params.Input.Any(x => x.Name == variableInputParameters[i].Name))
                    {
                        insertIndex += 1;
                    }
                }

                // Register the input parameter
                Params.RegisterInputParam(parameter, insertIndex);
            }

            // Expire solution and refresh parameters since they changed
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
            get { return RobotComponents.Gh.Properties.Resources.AbsoluteJointMovement_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2928D07D-BFFD-4C0A-931E-B1BF4AD27D04"); }
        }

    }

}