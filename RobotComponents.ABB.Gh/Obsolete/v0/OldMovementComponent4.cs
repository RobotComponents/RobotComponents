﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;
using RobotComponents.ABB.Gh.Parameters.Actions.Instructions;
using RobotComponents.ABB.Gh.Parameters.Definitions;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Action : Movement component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldMovementComponent4 : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, subcategory the panel. 
        /// If you use non-existing tab or panel names new tabs/panels will automatically be created.
        /// </summary>
        public OldMovementComponent4()
          : base("Move", "M",
              "Defines a linear or joint movement instruction."
               + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
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
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Target(), "Target", "T", "Target of the movement as Target", GH_ParamAccess.list);
            pManager.AddParameter(new Param_SpeedData(), "Speed Data", "SD", "Speed Data as Speed Data or as a number (vTCP)", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Movement Type", "MT", "Movement Type as integer. Use 0 for MoveAbsJ, 1 for MoveL and 2 for MoveJ", GH_ParamAccess.list, 0);
            pManager.AddParameter(new Param_ZoneData(), "Zone Data", "ZD", "Zone Data as Zone Data or as a number (path zone TCP)", GH_ParamAccess.list);

            pManager[3].Optional = true;
        }

        // Register the number of fixed input parameters
        private readonly int fixedParamNumInput = 4;

        // Create an array with the variable input parameters
        readonly IGH_Param[] variableInputParameters = new IGH_Param[3]
        {
            new Param_RobotTool() { Name = "Robot Tool", NickName = "RT", Description = "Robot Tool as list", Access = GH_ParamAccess.list, Optional = true},
            new Param_WorkObject() { Name = "Work Object", NickName = "WO", Description = "Work Object as list", Access = GH_ParamAccess.list, Optional = true },
            new Param_SetDigitalOutput() { Name = "Digital Output", NickName = "DO", Description = "Digital Output as list. For creation of MoveLDO and MoveJDO", Access = GH_ParamAccess.list, Optional = true }
        };

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Movement(), "Movement", "M", "Resulting Move instruction");
        }

        // Fields
        private bool _expire = false;
        private bool _overrideRobotTool = false;
        private bool _overrideWorkObject = false;
        private bool _setDigitalOutput = false;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Creates the input value list and attachs it to the input parameter
            CreateValueList();

            // Expire solution of this component
            if (_expire == true)
            {
                _expire = false;
                this.ExpireSolution(true);
            }

            // Input variables
            List<ITarget> targets = new List<ITarget>();
            List<SpeedData> speedDatas = new List<SpeedData>();
            List<int> movementTypes = new List<int>();
            List<ZoneData> zoneDatas = new List<ZoneData>();
            List<RobotTool> robotTools = new List<RobotTool>();
            List<WorkObject> workObjects = new List<WorkObject>();
            List<SetDigitalOutput> digitalOutputs = new List<SetDigitalOutput>();

            // Catch the input data from the fixed parameters
            if (!DA.GetDataList(0, targets)) { return; }
            if (!DA.GetDataList(1, speedDatas)) { return; }
            if (!DA.GetDataList(2, movementTypes)) { return; }
            if (!DA.GetDataList(3, zoneDatas)) { zoneDatas = new List<ZoneData>() { new ZoneData(0) }; }

            // Catch the input data from the variable parameteres
            if (Params.Input.Any(x => x.Name == variableInputParameters[0].Name))
            {
                if (!DA.GetDataList(variableInputParameters[0].Name, robotTools))
                {
                    robotTools = new List<RobotTool>() { RobotTool.GetEmptyRobotTool() };
                }
            }
            if (Params.Input.Any(x => x.Name == variableInputParameters[1].Name))
            {
                if (!DA.GetDataList(variableInputParameters[1].Name, workObjects))
                {
                    workObjects = new List<WorkObject>() { new WorkObject() };
                }
            }
            if (Params.Input.Any(x => x.Name == variableInputParameters[2].Name))
            {
                if (!DA.GetDataList(variableInputParameters[2].Name, digitalOutputs))
                {
                    digitalOutputs = new List<SetDigitalOutput>() { new SetDigitalOutput() };
                }
            }

            // Make sure variable input parameters have a default value
            if (robotTools.Count == 0)
            {
                robotTools.Add(RobotTool.GetEmptyRobotTool()); // Empty Robot Tool
            }
            if (workObjects.Count == 0)
            {
                workObjects.Add(new WorkObject()); // Makes a default WorkObject (wobj0)
            }
            if (digitalOutputs.Count == 0)
            {
                digitalOutputs.Add(new SetDigitalOutput()); // InValid / empty DO
            }

            // Get longest Input List
            int[] sizeValues = new int[7];
            sizeValues[0] = targets.Count;
            sizeValues[1] = speedDatas.Count;
            sizeValues[2] = movementTypes.Count;
            sizeValues[3] = zoneDatas.Count;
            sizeValues[4] = robotTools.Count;
            sizeValues[5] = workObjects.Count;
            sizeValues[6] = digitalOutputs.Count;

            int biggestSize = sizeValues.Max();

            // Keeps track of used indicies
            int targetGooCounter = -1;
            int speedDataCounter = -1;
            int movementTypeCounter = -1;
            int zoneDataCounter = -1;
            int robotToolGooCounter = -1;
            int workObjectGooCounter = -1;
            int digitalOutputGooCounter = -1;

            // Creates movements
            List<Movement> movements = new List<Movement>();

            for (int i = 0; i < biggestSize; i++)
            {
                ITarget target;
                SpeedData speedData;
                int movementType;
                ZoneData zoneData;
                RobotTool robotTool;
                WorkObject workObject;
                SetDigitalOutput digitalOutput;

                // Target counter
                if (i < sizeValues[0])
                {
                    target = targets[i];
                    targetGooCounter++;
                }
                else
                {
                    target = targets[targetGooCounter];
                }

                // Workobject counter
                if (i < sizeValues[1])
                {
                    speedData = speedDatas[i];
                    speedDataCounter++;
                }
                else
                {
                    speedData = speedDatas[speedDataCounter];
                }

                // Movement type counter
                if (i < sizeValues[2])
                {
                    movementType = movementTypes[i];
                    movementTypeCounter++;
                }
                else
                {
                    movementType = movementTypes[movementTypeCounter];
                }

                // Precision counter
                if (i < sizeValues[3])
                {
                    zoneData = zoneDatas[i];
                    zoneDataCounter++;
                }
                else
                {
                    zoneData = zoneDatas[zoneDataCounter];
                }

                // Robot tool counter
                if (i < sizeValues[4])
                {
                    robotTool = robotTools[i];
                    robotToolGooCounter++;
                }
                else
                {
                    robotTool = robotTools[robotToolGooCounter];
                }

                // Work Object counter
                if (i < sizeValues[5])
                {
                    workObject = workObjects[i];
                    workObjectGooCounter++;
                }
                else
                {
                    workObject = workObjects[workObjectGooCounter];
                }

                // Digital Output counter
                if (i < sizeValues[6])
                {
                    digitalOutput = digitalOutputs[i];
                    digitalOutputGooCounter++;
                }
                else
                {
                    digitalOutput = digitalOutputs[digitalOutputGooCounter];
                }

                // Movement constructor
                Movement movement = new Movement((MovementType)movementType, target, speedData, zoneData, robotTool, workObject, digitalOutput);
                movements.Add(movement);
            }

            // Check if a right value is used for the movement type
            for (int i = 0; i < movementTypes.Count; i++)
            {
                if (movementTypes[i] != 0 && movementTypes[i] != 1 && movementTypes[i] != 2)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Movement type value <" + i + "> is invalid. " +
                        "In can only be set to 0, 1 and 2. Use 0 for MoveAbsJ, 1 for MoveL and 2 for MoveJ.");
                    break;
                }
            }

            // Check if an exact predefined zonedata value is used
            for (int i = 0; i < zoneDatas.Count; i++)
            {
                if (zoneDatas[i].IsExactPredefinedValue == false & zoneDatas[i].IsPreDefined == true)
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
                if (speedDatas[i].IsExactPredefinedValue == false & speedDatas[i].IsPreDefined == true)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Predefined speeddata value <" + i + "> is invalid. " +
                        "The nearest valid predefined speed data value is used. Valid predefined speeddata values are 5, 10, " +
                        "20, 30, 40, 50, 60, 80, 100, 150, 200, 300, 400, 500, 600, 800, 1000, 1500, 2000, 2500, 3000, 4000, " +
                        "5000, 6000 and 7000.");
                    break;
                }
            }

            // Check target and movement combination
            for (int i = 0; i < movements.Count; i++)
            {
                if (movements[i].MovementType == MovementType.MoveAbsJ && movements[i].Target is RobotTarget)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "An Absolute Joint Movement instruction is combined with " +
                        "a Robot Target. The Robot Target will be converted to a Joint Target.");
                    break;
                }
            }

            // Output
            DA.SetDataList(0, movements);
        }

        // Method for creating the value list with movement types
        #region valuelist
        /// <summary>
        /// Creates the value list for the motion type and connects it the input parameter is other source is connected
        /// </summary>
        private void CreateValueList()
        {
            if (this.Params.Input[2].SourceCount == 0)
            {
                // Gets the input parameter
                var parameter = Params.Input[2];

                // Creates the empty value list
                GH_ValueList obj = new GH_ValueList();
                obj.CreateAttributes();
                obj.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;
                obj.ListItems.Clear();

                // Add the items to the value list
                // Add the items to the value list
                string[] names = Enum.GetNames(typeof(MovementType));
                int[] values = (int[])Enum.GetValues(typeof(MovementType));

                for (int i = 0; i < names.Length; i++)
                {
                    obj.ListItems.Add(new GH_ValueListItem(names[i], values[i].ToString()));
                }

                // Make point where the valuelist should be created on the canvas
                obj.Attributes.Pivot = new PointF(parameter.Attributes.InputGrip.X - 120, parameter.Attributes.InputGrip.Y - 11);

                // Add the value list to the active canvas
                Instances.ActiveCanvas.Document.AddObject(obj, false);

                // Connect the value list to the input parameter
                parameter.AddSource(obj);

                // Collect data
                parameter.CollectData();

                // Set bool for expire solution of this component
                _expire = true;

                // First expire the solution of the value list
                obj.ExpireSolution(true);
            }
        }
        #endregion

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
        /// Boolean that indicates if the custom menu item for overriding the Work Object is checked
        /// </summary>
        public bool OverrideWorkObject
        {
            get { return _overrideWorkObject; }
            set { _overrideWorkObject = value; }
        }

        /// <summary>
        /// Boolean that indicates if the custom menu item for setting a Digital Output is is checked
        /// </summary>
        public bool SetDigitalOutput
        {
            get { return _setDigitalOutput; }
            set { _setDigitalOutput = value; }
        }

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Override Robot Tool", OverrideRobotTool);
            writer.SetBoolean("Override Work Object", OverrideWorkObject);
            writer.SetBoolean("Set Digital Output", SetDigitalOutput);
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
            OverrideWorkObject = reader.GetBoolean("Override Work Object");
            SetDigitalOutput = reader.GetBoolean("Set Digital Output");
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
            Menu_AppendItem(menu, "Override Work Object", MenuItemClickWorkObject, true, OverrideWorkObject);
            Menu_AppendItem(menu, "Set Digital Output", MenuItemClickDigitalOutput, true, SetDigitalOutput);
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
        /// Handles the event when the custom menu item "Work Object" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickWorkObject(object sender, EventArgs e)
        {
            RecordUndoEvent("Override Work Object");
            OverrideWorkObject = !OverrideWorkObject;
            AddParameter(1);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Digital Output" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickDigitalOutput(object sender, EventArgs e)
        {
            RecordUndoEvent("Set Digital Output");
            SetDigitalOutput = !SetDigitalOutput;
            AddParameter(2);
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
            get { return Properties.Resources.Movement_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("AB744C95-E6A0-4ABD-B62D-14B558BEEDF8"); }
        }

    }

}