using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;

using GH_IO.Serialization;

using RobotComponents.BaseClasses;

using RobotComponentsABB.Goos;
using RobotComponentsABB.Parameters;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components
{
    /// <summary>
    /// RobotComponents Action : Movement component. An inherent from the GH_Component Class.
    /// </summary>
    public class MovementComponent : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, subcategory the panel. 
        /// If you use non-existing tab or panel names new tabs/panels will automatically be created.
        /// </summary>
        public MovementComponent()
          : base("Action: Movement", "M",
              "Defines a robot movement instruction for simulation and code generation."
                + System.Environment.NewLine +
                "RobotComponents: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
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
            pManager.AddParameter(new TargetParameter(), "Target", "T", "Target as Target", GH_ParamAccess.list);
            // To do: Something goes wrong with (de)serialization of the speed data parameter if it is used here. 
            // The problem occurs since it is a IGH_VariableParameterComponent. Without the IGH_VariableParameterComponent it works smooth. 
            pManager.AddParameter(new SpeedDataParameter(), "Speed Data", "SD", "Speed Data as Custom Speed Data or as a number (vTCP)", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Speed Data", "SD", "Speed Data as Custom Speed Data or as a number (vTCP)", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Movement Type", "MT", "Movement Type as integer. Use 0 for MoveAbsJ, 1 for MoveL and 2 for MoveJ", GH_ParamAccess.list, 0);
            pManager.AddIntegerParameter("Precision", "P", "Precision as int. If value is smaller than 0, precision will be set to fine.", GH_ParamAccess.list, 0);
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
        bool _expire = false;
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

            // Add volatiledata
            CreateVolatileData();

            // Input variables
            List<TargetGoo> targetGoos = new List<TargetGoo>();
            List<SpeedDataGoo> speedDataGoos = new List<SpeedDataGoo>();
            List<int> movementTypes = new List<int>();
            List<int> precisions = new List<int>();
            List<RobotToolGoo> robotToolGoos = new List<RobotToolGoo>();
            List<WorkObjectGoo> workObjectGoos = new List<WorkObjectGoo>();
            List<DigitalOutputGoo> digitalOutputGoos = new List<DigitalOutputGoo>();

            // Catch the input data from the fixed parameters
            if (!DA.GetDataList(0, targetGoos)) { return; }
            if (!DA.GetDataList(1, speedDataGoos)) { return; }
            if (!DA.GetDataList(2, movementTypes)) { return; }
            if (!DA.GetDataList(3, precisions)) { return; }

            // Catch the input data from the variable parameteres
            if (Params.Input.Any(x => x.Name == variableInputParameters[0].Name))
            {
                if (!DA.GetDataList(variableInputParameters[0].Name, robotToolGoos)) return;
            }
            if (Params.Input.Any(x => x.Name == variableInputParameters[1].Name))
            {
                if (!DA.GetDataList(variableInputParameters[1].Name, workObjectGoos)) return;
            }
            if (Params.Input.Any(x => x.Name == variableInputParameters[2].Name))
            {
                if (!DA.GetDataList(variableInputParameters[2].Name, digitalOutputGoos)) return;
            }

            // Make sure variable input parameters have a default value
            if (robotToolGoos.Count == 0)
            {
                robotToolGoos.Add(new RobotToolGoo()); // Makes a default RobotTool (tool0)
            }
            if (workObjectGoos.Count == 0)
            {
                workObjectGoos.Add(new WorkObjectGoo()); // Makes a default WorkObject (wobj0)
            }
            if (digitalOutputGoos.Count == 0)
            {
                digitalOutputGoos.Add(new DigitalOutputGoo()); // InValid DO
            }

            // Get longest Input List
            int[] sizeValues = new int[7];
            sizeValues[0] = targetGoos.Count;
            sizeValues[1] = speedDataGoos.Count;
            sizeValues[2] = movementTypes.Count;
            sizeValues[3] = precisions.Count;
            sizeValues[4] = robotToolGoos.Count;
            sizeValues[5] = workObjectGoos.Count;
            sizeValues[6] = digitalOutputGoos.Count;

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
                TargetGoo targetGoo;
                SpeedDataGoo speedDataGoo;
                int movementType;
                int precision;
                RobotToolGoo robotToolGoo;
                WorkObjectGoo workObjectGoo;
                DigitalOutputGoo digitalOutputGoo;

                // Target counter
                if (i < targetGoos.Count)
                {
                    targetGoo = targetGoos[i];
                    targetGooCounter++;
                }
                else
                {
                    targetGoo = targetGoos[targetGooCounter];
                }

                // Workobject counter
                if (i < speedDataGoos.Count)
                {
                    speedDataGoo = speedDataGoos[i];
                    speedDataGooCounter++;
                }
                else
                {
                    speedDataGoo = speedDataGoos[speedDataGooCounter];
                }

                // Movement type counter
                if (i < movementTypes.Count)
                {
                    movementType = movementTypes[i];
                    movementTypeCounter++;
                }
                else
                {
                    movementType = movementTypes[movementTypeCounter];
                }

                // Precision counter
                if (i < precisions.Count)
                {
                    precision = precisions[i];
                    precisionCounter++;
                }
                else
                {
                    precision = precisions[precisionCounter];
                }

                // Robot tool counter
                if (i < robotToolGoos.Count)
                {
                    robotToolGoo = robotToolGoos[i];
                    robotToolGooCounter++;
                }
                else
                {
                    robotToolGoo = robotToolGoos[robotToolGooCounter];
                }

                // Work Object counter
                if (i < workObjectGoos.Count)
                {
                    workObjectGoo = workObjectGoos[i];
                    workObjectGooCounter++;
                }
                else
                {
                    workObjectGoo = workObjectGoos[workObjectGooCounter];
                }

                // Digital Output counter
                if (i < digitalOutputGoos.Count)
                {
                    digitalOutputGoo = digitalOutputGoos[i];
                    digitalOutputGooCounter++;
                }
                else
                {
                    digitalOutputGoo = digitalOutputGoos[digitalOutputGooCounter];
                }

                // Movement constructor
                Movement movement = new Movement(targetGoo.Value, speedDataGoo.Value, movementType, precision, robotToolGoo.Value, workObjectGoo.Value, digitalOutputGoo.Value);
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

            // Check if a right predefined speeddata value is used
            for (int i = 0; i < speedDataGoos.Count; i++)
            {
                if (speedDataGoos[i].Value.PreDefinied == true)
                {
                    if (HelperMethods.PredefinedSpeedValueIsValid(speedDataGoos[i].Value.V_TCP) == false)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Pre-defined speed data <" + i +
                            "> is invalid. Use the speed data component to create custom speed data or use of one of the valid pre-defined speed datas. " +
                            "Pre-defined speed data can be set to 5, 10, 20, 30, 40, 50, 60, 80, 100, 150, 200, 300, " +
                            "400, 500, 600, 800, 1000, 1500, 2000, 2500, 3000, 4000, 5000, 6000 or 7000.");
                        break;
                    }
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
            // Add our own fields
            writer.SetBoolean("Override Robot Tool", OverrideRobotTool);
            writer.SetBoolean("Override Work Object", OverrideWorkObject);
            writer.SetBoolean("Set Digital Output", SetDigitalOutput);

            // Call the base class implementation.
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            // Read our own fields
            OverrideRobotTool = reader.GetBoolean("Override Robot Tool");
            OverrideWorkObject = reader.GetBoolean("Override Work Object");
            SetDigitalOutput = reader.GetBoolean("Set Digital Output");
            
            // Call the base class implementation.
            return base.Read(reader);
        }

        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            // Add menu separator
            Menu_AppendSeparator(menu);

            // Add custom menu items
            Menu_AppendItem(menu, "Override Robot Tool", MenuItemClickRobotTool, true, OverrideRobotTool);
            Menu_AppendItem(menu, "Override Work Object", MenuItemClickWorkObject, true, OverrideWorkObject);
            Menu_AppendItem(menu, "Set Digital Output", MenuItemClickDigitalOutput, true, SetDigitalOutput);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Robot Tool" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        public void MenuItemClickRobotTool(object sender, EventArgs e)
        {
            // Change bool
            RecordUndoEvent("Override Robot Tool");
            OverrideRobotTool = !OverrideRobotTool;

            // Add or remove the robot tool input parameter
            AddParameter(0);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Work Object" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        public void MenuItemClickWorkObject(object sender, EventArgs e)
        {
            // Change bool
            RecordUndoEvent("Override Work Object");
            OverrideWorkObject = !OverrideWorkObject;

            // Add or remove the work object input parameter
            AddParameter(1);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Digital Output" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        public void MenuItemClickDigitalOutput(object sender, EventArgs e)
        {
            // Change bool
            RecordUndoEvent("Set Digital Output");
            SetDigitalOutput = !SetDigitalOutput;

            // Add or remove the digital output parameter
            AddParameter(2);
        }

        /// <summary>
        /// Adds or destroys the input parameter to the component.
        /// </summary>
        /// <param name="index"> The index number of the parameter that needs to be added. </param>
        public void AddParameter(int index)
        {
            // Pick the parameter
            IGH_Param parameter = variableInputParameters[index];

            // Parameter name
            string name = variableInputParameters[index].Name;

            // If the parameter already exist: remove it
            if (Params.Input.Any(x => x.Name == name))
            {
                // Unregister the parameter
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

                // Add volatile data
                CreateVolatileData();
            }

        // Expire solution and refresh parameters since they changed
        Params.OnParametersChanged();
        ExpireSolution(true);

        }

        /// <summary>
        /// Adds the default data to the variable input parameters.
        /// </summary>
        public void CreateVolatileData()
        {
            // Variables
            bool changed = false;
            string name;
            int index;

            // Robot Tool
            name = variableInputParameters[0].Name;
            if (Params.Input.Any(x => x.Name == name))
            {
                index = Params.Input.FindIndex(x => x.Name == name);
                if (Params.Input[index].VolatileData.DataCount == 0)
                {
                    Params.Input[index].AddVolatileData(new GH_Path(0), 0, new RobotToolGoo()); // tool0
                    changed = true;
                }
            }

            // Work Object
            name = variableInputParameters[1].Name;
            if (Params.Input.Any(x => x.Name == name))
            {
                index = Params.Input.FindIndex(x => x.Name == name);
                if (Params.Input[index].VolatileData.DataCount == 0)
                {
                    Params.Input[index].AddVolatileData(new GH_Path(0), 0, new WorkObjectGoo()); // wobj0
                    changed = true;
                }
            }

            // Digital Output
            name = variableInputParameters[2].Name;
            if (Params.Input.Any(x => x.Name == name))
            {
                index = Params.Input.FindIndex(x => x.Name == name);
                if (Params.Input[index].VolatileData.DataCount == 0)
                {
                    Params.Input[index].AddVolatileData(new GH_Path(0), 0, new DigitalOutputGoo()); // InValid DO
                    changed = true;
                }
            }

            // Expire solution when default data was added
            if (changed == true)
            {
                Params.OnParametersChanged();
                ExpireSolution(true);
            }

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
            get { return RobotComponentsABB.Properties.Resources.Movement_Icon; }
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