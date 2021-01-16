// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

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
using RobotComponents.Actions;
using RobotComponents.Definitions;
using RobotComponents.Enumerations;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Gh.Utils;
using RobotComponents.Gh.Goos.Actions;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.08.000
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Action : Movement component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldMovementComponent2 : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, subcategory the panel. 
        /// If you use non-existing tab or panel names new tabs/panels will automatically be created.
        /// </summary>
        public OldMovementComponent2()
          : base("Action: Movement", "M",
              "Defines a robot movement instruction for simulation and code generation."
                + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "RAPID Generation")

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
            pManager.AddParameter(new RobotTargetParameter(), "Target", "T", "Target as Target", GH_ParamAccess.list);
            pManager.AddParameter(new SpeedDataParameter(), "Speed Data", "SD", "Speed Data as Custom Speed Data or as a number (vTCP)", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Movement Type", "MT", "Movement Type as integer. Use 0 for MoveAbsJ, 1 for MoveL and 2 for MoveJ", GH_ParamAccess.list, 0);
            pManager.AddIntegerParameter("Zone Data", "Z", "The zone size for the TCP path as int. If the value is smaller than 0, zonedata will be set to fine.", GH_ParamAccess.list, 0);
        }

        // Register the number of fixed input parameters
        private readonly int fixedParamNumInput = 4;

        // Create an array with the variable input parameters
        readonly IGH_Param[] variableInputParameters = new IGH_Param[3]
        {
            new RobotToolParameter() { Name = "Robot Tool", NickName = "RT", Description = "Robot Tool as as list", Access = GH_ParamAccess.list, Optional = true},
            new WorkObjectParameter() { Name = "Work Object", NickName = "WO", Description = "Work Object as a list", Access = GH_ParamAccess.list, Optional = true },
            new DigitalOutputParameter() { Name = "Digital Output", NickName = "DO", Description = "Digital Output as a list. For creation of MoveLDO and MoveJDO", Access = GH_ParamAccess.list, Optional = true }
        };

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new MovementParameter(), "Movement", "M", "Resulting Movement");  //Todo: beef this up to be more informative.
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
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed " +
                "in the future. Remove this component from your canvas and replace it by picking the new component " +
                "from the ribbon.");

            // Creates the input value list and attachs it to the input parameter
            CreateValueList();

            // Expire solution of this component
            if (_expire == true)
            {
                _expire = false;
                this.ExpireSolution(true);
            }

            // Input variables
            List<RobotTarget> targets = new List<RobotTarget>();
            List<GH_SpeedData> speedDataGoos = new List<GH_SpeedData>();
            List<int> movementTypes = new List<int>();
            List<int> precisions = new List<int>();
            List<RobotTool> robotTools = new List<RobotTool>();
            List<WorkObject> workObjects = new List<WorkObject>();
            List<DigitalOutput> digitalOutputs = new List<DigitalOutput>();

            // Create an empty Robot Tool
            RobotTool emptyRobotTool = new RobotTool();
            emptyRobotTool.Clear();

            // Catch the input data from the fixed parameters
            if (!DA.GetDataList(0, targets)) { return; }
            if (!DA.GetDataList(1, speedDataGoos)) { return; }
            if (!DA.GetDataList(2, movementTypes)) { return; }
            if (!DA.GetDataList(3, precisions)) { return; }

            // Catch the input data from the variable parameteres
            if (Params.Input.Any(x => x.Name == variableInputParameters[0].Name))
            {
                if (!DA.GetDataList(variableInputParameters[0].Name, robotTools))
                {
                    robotTools = new List<RobotTool>() { new RobotTool(emptyRobotTool) };
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
                    digitalOutputs = new List<DigitalOutput>() { new DigitalOutput() };
                }
            }

            // Make sure variable input parameters have a default value
            if (robotTools.Count == 0)
            {
                robotTools.Add(new RobotTool(emptyRobotTool)); // Empty Robot Tool
            }
            if (workObjects.Count == 0)
            {
                workObjects.Add(new WorkObject()); // Makes a default WorkObject (wobj0)
            }
            if (digitalOutputs.Count == 0)
            {
                digitalOutputs.Add(new DigitalOutput()); // InValid / empty DO
            }

            // Get longest Input List
            int[] sizeValues = new int[7];
            sizeValues[0] = targets.Count;
            sizeValues[1] = speedDataGoos.Count;
            sizeValues[2] = movementTypes.Count;
            sizeValues[3] = precisions.Count;
            sizeValues[4] = robotTools.Count;
            sizeValues[5] = workObjects.Count;
            sizeValues[6] = digitalOutputs.Count;

            int biggestSize = HelperMethods.GetBiggestValue(sizeValues);

            // Keeps track of used indicies
            int targetGooCounter = -1;
            int speedDataGooCounter = -1;
            int movementTypeCounter = -1;
            int precisionCounter = -1;
            int robotToolGooCounter = -1;
            int workObjectGooCounter = -1;
            int digitalOutputGooCounter = -1;
            
            // Creates movements
            List<Movement> movements = new List<Movement>();

            for (int i = 0; i < biggestSize; i++)
            {
                RobotTarget target;
                SpeedData speedData;
                int movementType;
                int precision;
                RobotTool robotTool;
                WorkObject workObject;
                DigitalOutput digitalOutput;

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
                    speedData = speedDataGoos[i].Value;
                    speedDataGooCounter++;
                }
                else
                {
                    speedData = speedDataGoos[speedDataGooCounter].Value;
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
                    precision = precisions[i];
                    precisionCounter++;
                }
                else
                {
                    precision = precisions[precisionCounter];
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
                Movement movement = new Movement((MovementType)movementType, target, speedData, new ZoneData(precision), robotTool, workObject, digitalOutput);
                movements.Add(movement);
            }

            // Check if a right value is used for the movement type
            for (int i = 0; i < movementTypes.Count; i++)
            {
                if (movementTypes[i] != 0 && movementTypes[i] != 1 && movementTypes[i] != 2)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Movement type value <" + i + "> is invalid. " +
                        "In can only be set to 0, 1 and 2. Use 1 for MoveAbsJ, 2 for MoveL and 3 for MoveJ.");
                    break;
                }
            }

            // Check if a right value is used for the input of the precision
            for (int i = 0; i < precisions.Count; i++)
            {
                if (HelperMethods.PrecisionValueIsValid(precisions[i]) == false)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Precision value <" + i + "> is invalid. " +
                        "In can only be set to -1, 0, 1, 5, 10, 15, 20, 30, 40, 50, 60, 80, 100, 150 or 200. " +
                        "A value of -1 will be interpreted as fine movement in RAPID Code.");
                    break;
                }
            }

            // Check if an exact predefined speeddata value is used
            for (int i = 0; i < speedDataGoos.Count; i++)
            {
                if (speedDataGoos[i].Value.ExactPredefinedValue == false & speedDataGoos[i].Value.PreDefinied == true)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Predefined speeddata value <" + i + "> is invalid. " +
                        "The nearest valid predefined speed data value is used. Valid predefined speeddata values are 5, 10, " +
                        "20, 30, 40, 50, 60, 80, 100, 150, 200, 300, 400, 500, 600, 800, 1000, 1500, 2000, 2500, 3000, 4000, " +
                        "5000, 6000 and 7000.");
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
                obj.ListItems.Add(new GH_ValueListItem("MoveAbsJ", "0"));
                obj.ListItems.Add(new GH_ValueListItem("MoveL", "1"));
                obj.ListItems.Add(new GH_ValueListItem("MoveJ", "2"));

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
            get { return RobotComponents.Gh.Properties.Resources.Movement_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B1E7F4C2-2FDC-4E9B-8D8A-9F5FBBB5B64F"); }
        }

    }

}