// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Interfaces;
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Gh.Parameters.Actions;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Action : Movement component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class MovementComponent_OBSOLETE : GH_Component, IGH_VariableParameterComponent
    {
        #region fields
        private bool _expire = false;
        private bool _overrideRobotTool = false;
        private bool _overrideWorkObject = false;
        private bool _setDigitalOutput = false;
        private readonly int _fixedParamNumInput = 4;
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, subcategory the panel. 
        /// If you use non-existing tab or panel names new tabs/panels will automatically be created.
        /// </summary>
        public MovementComponent_OBSOLETE()
          : base("Move", "M",
              "Defines a linear or joint movement instruction."
               + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Code Generation")

        {
            // Create the component label with a message
            Message = "EXTENDABLE";
        }

        /// <summary>
        /// Stores the variable input parameters in an array.
        /// </summary>
        private readonly IGH_Param[] variableInputParameters = new IGH_Param[3]
        {
            new Param_RobotTool() { Name = "Robot Tool", NickName = "RT", Description = "Robot Tool as list", Access = GH_ParamAccess.item, Optional = true},
            new Param_WorkObject() { Name = "Work Object", NickName = "WO", Description = "Work Object as list", Access = GH_ParamAccess.item, Optional = true },
            new Param_DigitalOutput() { Name = "Digital Output", NickName = "DO", Description = "Digital Output as list. For creation of MoveLDO and MoveJDO", Access = GH_ParamAccess.item, Optional = true }
        };

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Target(), "Target", "T", "Target of the movement as Target", GH_ParamAccess.item);
            pManager.AddParameter(new Param_SpeedData(), "Speed Data", "SD", "Speed Data as Speed Data or as a number (vTCP)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Movement Type", "MT", "Movement Type as integer. Use 0 for MoveAbsJ, 1 for MoveL and 2 for MoveJ", GH_ParamAccess.item, 0);
            pManager.AddParameter(new Param_ZoneData(), "Zone Data", "ZD", "Zone Data as Zone Data or as a number (path zone TCP)", GH_ParamAccess.item);

            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Movement(), "Movement", "M", "Resulting Move instruction");   
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Creates the input value list and attachs it to the input parameter
            if (this.Params.Input[2].SourceCount == 0)
            {
                _expire = true;
                HelperMethods.CreateValueList(this, typeof(MovementType), 2);
            }

            // Expire solution of this component
            if (_expire == true)
            {
                _expire = false;
                this.ExpireSolution(true);
            }

            // Input variables
            ITarget target = new RobotTarget();
            SpeedData speedData = new SpeedData();
            int movementType = 0;
            ZoneData zoneData = new ZoneData();
            RobotTool robotTool = RobotTool.GetEmptyRobotTool();
            WorkObject workObject = new WorkObject();
            DigitalOutput digitalOutput = new DigitalOutput();

            // Catch the input data from the fixed parameters
            if (!DA.GetData(0, ref target)) { return; }
            if (!DA.GetData(1, ref speedData)) { return; }
            if (!DA.GetData(2, ref movementType)) { return; }
            if (!DA.GetData(3, ref zoneData)) { zoneData = new ZoneData(0); }

            // Catch the input data from the variable parameteres
            if (Params.Input.Any(x => x.Name == variableInputParameters[0].Name))
            {
                if (!DA.GetData(variableInputParameters[0].Name, ref robotTool))
                {
                    robotTool = RobotTool.GetEmptyRobotTool();
                }
            }
            if (Params.Input.Any(x => x.Name == variableInputParameters[1].Name))
            {
                if (!DA.GetData(variableInputParameters[1].Name, ref workObject))
                {
                    workObject = new WorkObject();
                }
            }
            if (Params.Input.Any(x => x.Name == variableInputParameters[2].Name))
            {
                if (!DA.GetData(variableInputParameters[2].Name, ref digitalOutput))
                {
                    digitalOutput = new DigitalOutput();
                }
            }

            // Movement constructor
                Movement movement = new Movement((MovementType)movementType, target, speedData, zoneData, robotTool, workObject, digitalOutput);

            // Check if a right value is used for the movement type
            if (movementType != 0 && movementType != 1 && movementType != 2)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Movement type value is invalid. " +
                    "In can only be set to 0, 1 and 2. Use 0 for MoveAbsJ, 1 for MoveL and 2 for MoveJ.");
            }

            // Check if an exact predefined zonedata value is used
            if (zoneData.ExactPredefinedValue == false & zoneData.PreDefined == true)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Predefined zonedata value is invalid. " +
                    "The nearest valid predefined speeddata value is used. Valid predefined zonedata values are -1, " +
                    "0, 1, 5, 10, 15, 20, 30, 40, 50, 60, 80, 100, 150 or 200. " +
                    "A value of -1 will be interpreted as fine movement in RAPID Code.");
            }

            // Check if an exact predefined speeddata value is used
            if (speedData.ExactPredefinedValue == false & speedData.PreDefined == true)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Predefined speeddata value is invalid. " +
                    "The nearest valid predefined speed data value is used. Valid predefined speeddata values are 5, 10, " +
                    "20, 30, 40, 50, 60, 80, 100, 150, 200, 300, 400, 500, 600, 800, 1000, 1500, 2000, 2500, 3000, 4000, " +
                    "5000, 6000 and 7000.");
            }

            // Check target and movement combination
            if (movement.MovementType == MovementType.MoveAbsJ && movement.Target is RobotTarget)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "An Absolute Joint Movement instruction is combined with " +
                    "a Robot Target. The Robot Target will be converted to a Joint Target.");
            }

            // Output
            DA.SetData(0, movement);
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
            get { return RobotComponents.ABB.Gh.Properties.Resources.Movement_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("A478CB0C-5AAB-4AD5-8259-062B844A7006"); }
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
            Menu_AppendItem(menu, "Override Robot Tool", MenuItemClickRobotTool, true, _overrideRobotTool);
            Menu_AppendItem(menu, "Override Work Object", MenuItemClickWorkObject, true, _overrideWorkObject);
            Menu_AppendItem(menu, "Set Digital Output", MenuItemClickDigitalOutput, true, _setDigitalOutput);
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
        /// Handles the event when the custom menu item "Robot Tool" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickRobotTool(object sender, EventArgs e)
        {
            RecordUndoEvent("Override Robot Tool");
            _overrideRobotTool = !_overrideRobotTool;
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
            _overrideWorkObject = !_overrideWorkObject;
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
            _setDigitalOutput = !_setDigitalOutput;
            AddParameter(2);
        }

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Override Robot Tool", _overrideRobotTool);
            writer.SetBoolean("Override Work Object", _overrideWorkObject);
            writer.SetBoolean("Set Digital Output", _setDigitalOutput);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            _overrideRobotTool = reader.GetBoolean("Override Robot Tool");
            _overrideWorkObject = reader.GetBoolean("Override Work Object");
            _setDigitalOutput = reader.GetBoolean("Set Digital Output");
            return base.Read(reader);
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
                int insertIndex = _fixedParamNumInput;

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
    }
}