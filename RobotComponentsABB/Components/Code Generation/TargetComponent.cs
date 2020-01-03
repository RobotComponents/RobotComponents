using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;

using GH_IO.Serialization;

using Rhino.Geometry;

using RobotComponents.BaseClasses;

using RobotComponentsABB.Parameters;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components
{
    /// <summary>
    /// RobotComponents Action : Target component. An inherent from the GH_Component Class.
    /// </summary>
    public class TargetComponent : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public TargetComponent()
          : base("Action: Target", "T",
              "Defines a target for an Action: Movement or Inverse Kinematics component."
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
            pManager.AddTextParameter("Name", "N", "Name as string", GH_ParamAccess.list, "defaultTar");
            pManager.AddPlaneParameter("Plane", "P", "Plane as Plane", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Axis Configuration", "AC", "Axis Configuration as int. This will modify the fourth value of the Robot Configuration Data in the RAPID Movement code line.", GH_ParamAccess.list, 0);
        }

        // Register the number of fixed input parameters
        private readonly int fixedParamNumInput = 3;

        // Create an array with the external axis value override parameters
        readonly IGH_Param[] externalAxisParameters = new IGH_Param[6]
        {
            new Param_Number() { Name = "External Axis Value A", NickName = "EAa", Description = "Overrides the first external axis value", Access = GH_ParamAccess.list, Optional = true },
            new Param_Number() { Name = "External Axis Value B", NickName = "EAb", Description = "Overrides the second external axis value", Access = GH_ParamAccess.list, Optional = true },
            new Param_Number() { Name = "External Axis Value C", NickName = "EAc", Description = "Overrides the third external axis value", Access = GH_ParamAccess.list, Optional = true },
            new Param_Number() { Name = "External Axis Value D", NickName = "EAd", Description = "Overrides the fourth external axis value", Access = GH_ParamAccess.list, Optional = true },
            new Param_Number() { Name = "External Axis Value E", NickName = "EAe", Description = "Overrides the fifth external axis value", Access = GH_ParamAccess.list, Optional = true },
            new Param_Number() { Name = "External Axis Value F", NickName = "EAf", Description = "Overrides the sixth external axis value", Access = GH_ParamAccess.list, Optional = true }
        };

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new TargetParameter(), "Target", "T", "Resulting Target");  //Todo: beef this up to be more informative.
        }

        // fields
        public List<string> targetNames = new List<string>();
        private string lastName = "";
        private bool namesUnique;
        private ObjectManager objectManager;
        
        private bool _setReferencePlane = false;
        private bool _overrideExternalAxisValues = false;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Add volatiledata
            CreateVolatileData();
            
            // Gets Document ID
            string documentGUID = DocumentManager.GetRobotComponentsDocumentID(this.OnPingDocument());

            // Checks if ObjectManager for this document already exists. If not it creates a new ObjectManager in DocumentManger Dictionary
            if (!DocumentManager.ObjectManagers.ContainsKey(documentGUID)) {
                DocumentManager.ObjectManagers.Add(documentGUID, new ObjectManager());
            }

            // Gets ObjectManager of this document
            objectManager = DocumentManager.ObjectManagers[documentGUID];

            // Clears targetNames
            for (int i = 0; i < targetNames.Count; i++)
            {
                objectManager.TargetNames.Remove(targetNames[i]);
            }
            targetNames.Clear();

            // Adds Component to TargetByGuid Dictionary
            if (!objectManager.TargetsByGuid.ContainsKey(this.InstanceGuid))
            {
                objectManager.TargetsByGuid.Add(this.InstanceGuid, this);
            }

            // Removes lastName from targetNameList
            if (objectManager.TargetNames.Contains(lastName))
            {
                objectManager.TargetNames.Remove(lastName);
            }

            // Sets inputs and creates target
            Guid instanceGUID = this.InstanceGuid;
            List<string> names = new List<string>();
            List<Plane> planes = new List<Plane>();
            List<Plane> referencePlanes = new List<Plane>();
            List<int> axisConfigs = new List<int>();

            // Override external axis values input variables
            List<double> externalAxisValueA = new List<double>();
            List<double> externalAxisValueB = new List<double>();
            List<double> externalAxisValueC = new List<double>();
            List<double> externalAxisValueD = new List<double>();
            List<double> externalAxisValueE = new List<double>();
            List<double> externalAxisValueF = new List<double>();

            // Catch name input
            if (!DA.GetDataList(0, names)) { return; } // Fixed index
            
            // Catch target planes input
            if (!DA.GetDataList(1, planes)) { return; } // Fixed index

            // Catch reference plane input
            if (Params.Input.Any(x => x.Name == "Reference Plane"))
            {
                if (!DA.GetDataList("Reference Plane", referencePlanes)) return;
            }

            // Catch axis configuration input
            if (Params.Input.Any(x => x.Name == "Axis Configuration"))
            {
                if (!DA.GetDataList("Axis Configuration", axisConfigs)) return;
            }

            // Catch input data for overriding external axis values
            if (Params.Input.Any(x => x.Name == externalAxisParameters[0].Name))
            {
                // External axis A
                if (!DA.GetDataList(externalAxisParameters[0].Name, externalAxisValueA)) return;

                // External axis B
                if (Params.Input.Any(x => x.Name == externalAxisParameters[1].Name))
                {
                    if (!DA.GetDataList(externalAxisParameters[1].Name, externalAxisValueB)) return;

                    // External axis C
                    if (Params.Input.Any(x => x.Name == externalAxisParameters[2].Name))
                    {
                        if (!DA.GetDataList(externalAxisParameters[2].Name, externalAxisValueC)) return;

                        // External axis D
                        if (Params.Input.Any(x => x.Name == externalAxisParameters[3].Name))
                        {
                            if (!DA.GetDataList(externalAxisParameters[3].Name, externalAxisValueD)) return;

                            // External axis E
                            if (Params.Input.Any(x => x.Name == externalAxisParameters[4].Name))
                            {
                                if (!DA.GetDataList(externalAxisParameters[4].Name, externalAxisValueE)) return;

                                // External axis F
                                if (Params.Input.Any(x => x.Name == externalAxisParameters[5].Name))
                                {
                                    if (!DA.GetDataList(externalAxisParameters[5].Name, externalAxisValueF)) return;
                                }
                            }
                        }
                    }
                }
            }

            // Make sure variable input has a default value
            if (referencePlanes.Count == 0) { referencePlanes.Add(Plane.WorldXY); }
            if (externalAxisValueA.Count == 0) { externalAxisValueA.Add(9e9); }
            if (externalAxisValueB.Count == 0) { externalAxisValueB.Add(9e9); }
            if (externalAxisValueC.Count == 0) { externalAxisValueC.Add(9e9); }
            if (externalAxisValueD.Count == 0) { externalAxisValueD.Add(9e9); }
            if (externalAxisValueE.Count == 0) { externalAxisValueE.Add(9e9); }
            if (externalAxisValueF.Count == 0) { externalAxisValueF.Add(9e9); }

            // Get longest Input List
            int[] sizeValues = new int[10];
            sizeValues[0] = names.Count;
            sizeValues[1] = planes.Count;
            sizeValues[2] = referencePlanes.Count;
            sizeValues[3] = axisConfigs.Count;

            sizeValues[4] = externalAxisValueA.Count;
            sizeValues[5] = externalAxisValueB.Count;
            sizeValues[6] = externalAxisValueC.Count;
            sizeValues[7] = externalAxisValueD.Count;
            sizeValues[8] = externalAxisValueE.Count;
            sizeValues[9] = externalAxisValueF.Count;

            int biggestSize = HelperMethods.GetBiggestValue(sizeValues);

            // Keeps track of used indicies
            int nameCounter = -1;
            int planesCounter = -1;
            int referencePlaneCounter = -1;
            int axisConfigCounter = -1;

            int externalAxisValueCounterA = -1;
            int externalAxisValueCounterB= -1;
            int externalAxisValueCounterC = -1;
            int externalAxisValueCounterD = -1;
            int externalAxisValueCounterE = -1;
            int externalAxisValueCounterF = -1;

            // Creates targets
            List<Target> targets = new List<Target>();
            for (int i = 0; i < biggestSize; i++)
            {
                string name = "";
                Plane plane = new Plane();
                Plane referencePlane = new Plane();
                int axisConfig = 0;

                double[] axisValues = new double[6];

                // Names counter
                if (i < names.Count)
                {
                    name = names[i];
                    nameCounter++;
                }
                else
                {
                    name = names[nameCounter] + "_" + (i - nameCounter);
                }

                // Target planes counter
                if (i < planes.Count)
                {
                    plane = planes[i];
                    planesCounter++;
                }
                else
                {
                    plane = planes[planesCounter];
                }

                // Reference plane counter
                if (i < referencePlanes.Count)
                {
                    referencePlane = referencePlanes[i];
                    referencePlaneCounter++;
                }
                else
                {
                    referencePlane = referencePlanes[referencePlaneCounter];
                }

                // Axis configuration counter
                if (i < axisConfigs.Count)
                {
                    axisConfig = axisConfigs[i];
                    axisConfigCounter++;
                }
                else
                {
                    axisConfig = axisConfigs[axisConfigCounter];
                }

                // External Axis Value A
                if (i < externalAxisValueA.Count)
                {
                    axisValues[0] = externalAxisValueA[i];
                    externalAxisValueCounterA++;
                }
                else
                {
                    axisValues[0] = externalAxisValueA[externalAxisValueCounterA];
                }

                // External Axis Value B
                if (i < externalAxisValueB.Count)
                {
                    axisValues[1] = externalAxisValueB[i];
                    externalAxisValueCounterB++;
                }
                else
                {
                    axisValues[1] = externalAxisValueB[externalAxisValueCounterB];
                }

                // External Axis Value C
                if (i < externalAxisValueC.Count)
                {
                    axisValues[2] = externalAxisValueC[i];
                    externalAxisValueCounterC++;
                }
                else
                {
                    axisValues[2] = externalAxisValueC[externalAxisValueCounterC];
                }

                // External Axis Value D
                if (i < externalAxisValueD.Count)
                {
                    axisValues[3] = externalAxisValueD[i];
                    externalAxisValueCounterD++;
                }
                else
                {
                    axisValues[3] = externalAxisValueD[externalAxisValueCounterD];
                }

                // External Axis Value E
                if (i < externalAxisValueE.Count)
                {
                    axisValues[4] = externalAxisValueE[i];
                    externalAxisValueCounterE++;
                }
                else
                {
                    axisValues[4] = externalAxisValueE[externalAxisValueCounterE];
                }

                // External Axis Value F
                if (i < externalAxisValueF.Count)
                {
                    axisValues[5] = externalAxisValueF[i];
                    externalAxisValueCounterF++;
                }
                else
                {
                    axisValues[5] = externalAxisValueF[externalAxisValueCounterF];
                }

                Target target = new Target(name, plane, referencePlane, axisConfig, axisValues);
                targets.Add(target);
            }

            // Checks if target name is already in use and counts duplicates
            #region NameCheck
            namesUnique = true;
            for (int i = 0; i < names.Count; i++)
            {
                if (objectManager.TargetNames.Contains(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Target Name already in use.");
                    namesUnique = false;
                    lastName = "";
                    break;
                }
                else
                {
                    // Adds Target Name to list
                    targetNames.Add(names[i]);
                    objectManager.TargetNames.Add(names[i]);

                    // Run SolveInstance on other Targets with no unique Name to check if their name is now available
                    foreach (KeyValuePair<Guid, TargetComponent> entry in objectManager.TargetsByGuid)
                    {
                        if (entry.Value.lastName == "")
                        {
                            entry.Value.ExpireSolution(true);
                        }
                    }

                    lastName = names[i];
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

            // Sets Output
            DA.SetDataList(0, targets);

            // Recognizes if Component is Deleted and removes it from Object Managers target and name list
            GH_Document doc = this.OnPingDocument();
            if (doc != null)
            {
                doc.ObjectsDeleted += DocumentObjectsDeleted;
            }
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
                if (namesUnique == true)
                {
                    for (int i = 0; i < targetNames.Count; i++)
                    {
                        objectManager.TargetNames.Remove(targetNames[i]);
                    }
                }
                objectManager.TargetsByGuid.Remove(this.InstanceGuid);

                // Run SolveInstance on other Targets with no unique Name to check if their name is now available
                foreach (KeyValuePair<Guid, TargetComponent> entry in objectManager.TargetsByGuid)
                {
                    if (entry.Value.lastName == "")
                    {
                        entry.Value.ExpireSolution(true);
                    }
                }
            }
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
        /// Boolean that indicates if the custom menu item for overriding the External Axis Values Wis checked
        /// </summary>
        public bool OverrideExternalAxisValues
        {
            get { return _overrideExternalAxisValues ; }
            set { _overrideExternalAxisValues = value; }
        }

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            // Add our own fields
            writer.SetBoolean("Set Reference Plane", SetReferencePlane);
            writer.SetBoolean("Override External Axis Values", OverrideExternalAxisValues);

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
            SetReferencePlane = reader.GetBoolean("Set Reference Plane");
            OverrideExternalAxisValues= reader.GetBoolean("Override External Axis Values");

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
            Menu_AppendItem(menu, "Reference Plane", MenuItemClickReferencePlane, true, SetReferencePlane);
            Menu_AppendItem(menu, "External Axis Values", MenuItemClickExternalAxisValue, true, OverrideExternalAxisValues);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Reference Plane" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        public void MenuItemClickReferencePlane(object sender, EventArgs e)
        {
            // Change bool
            RecordUndoEvent("Set Reference Plane");
            SetReferencePlane = !SetReferencePlane;

            // Input parameter name
            string name = "Reference Plane";

            // If the parameter already exist: unregister it
            if (Params.Input.Any(x => x.Name == name))
            {
                // Unregister the parameter
                Params.UnregisterInputParameter(Params.Input.First(x => x.Name == name), true);
            }

            // Else add the reference plane parameter
            else
            {
                // Create the parameter
                IGH_Param parameter = new Param_Plane()
                {
                    Name = "Reference Plane",
                    NickName = "RP",
                    Description = "Reference Plane as a Plane",
                    Access = GH_ParamAccess.list,
                    Optional = true
                };

                // The index where the parameter should be added
                int index = 2;

                // Register the input parameter
                Params.RegisterInputParam(parameter, index);

                // Add volatile data
                CreateVolatileData();
            }

            // Expire solution and refresh parameters since they changed
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        /// <summary>
        /// Registers the event when the custom menu item "External Axis Values" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        public void MenuItemClickExternalAxisValue(object sender, EventArgs e)
        {
            // Change bool
            RecordUndoEvent("Override External Axis Values");
            OverrideExternalAxisValues = !OverrideExternalAxisValues;

            // Register if a parameter was unregistered
            bool deleted = false;

            // If the parameter already exists: unregister it
            for (int i = 0; i < externalAxisParameters.Length; i++)
            {
                if (Params.Input.Any(x => x.Name == externalAxisParameters[i].Name))
                {
                    Params.UnregisterInputParameter(Params.Input.First(x => x.Name == externalAxisParameters[i].Name), true);
                    deleted = true;
                }
            }

            // Else add the firs external axis parameter
            if (deleted == false)
            {
                AddExternalAxisValueParameter(0);
            }

            // Expire solution
            ExpireSolution(true);
        }

        /// <summary>
        /// This function needs to be called to add an input parameter to override the external axis value. 
        /// </summary>
        /// <param name="index"> The index number of the variable input parameter that needs to be added.
        /// In this case the index number is the external axis number (0, 1, 2, 3, 4, or 5). </param>
        private void AddExternalAxisValueParameter(int index)
        {
            // Pick the parameter that needs to be added
            IGH_Param parameter = externalAxisParameters[index];

            // Correction for the index number if the reference place was already added
            if (Params.Input.Any(x => x.Name == "Reference Plane"))
            {
                index += 1;
            }

            // Register the input parameter
            Params.RegisterInputParam(parameter, fixedParamNumInput + index);

            // Add volatile data
            CreateVolatileData();

            // Refresh parameters since they changed
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

            // Reference plane
            name = "Reference Plane";
            if (Params.Input.Any(x => x.Name == name))
            {
                index = Params.Input.FindIndex(x => x.Name == name);
                if (Params.Input[index].VolatileData.DataCount == 0)
                {
                    Params.Input[index].AddVolatileData(new GH_Path(0), 0, Plane.WorldXY);
                    changed = true;
                }
            }

            // Work Object
            for (int i = 0; i < externalAxisParameters.Length; i++)
            {
                name = externalAxisParameters[i].Name;
                if (Params.Input.Any(x => x.Name == name))
                {
                    index = Params.Input.FindIndex(x => x.Name == name);
                    if (Params.Input[index].VolatileData.DataCount == 0)
                    {
                        Params.Input[index].AddVolatileData(new GH_Path(0), 0, 9e9);
                        changed = true;
                    }
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
            // If the first external axis override parameter is added it is allowed to insert more parameters
            if (Params.Input.Any(x => x.Name == externalAxisParameters[0].Name))
            {
                // Correction for the index number if the reference place was already added
                int correct = 0;
                if (Params.Input.Any(x => x.Name == "Reference Plane"))
                {
                    correct += 1;
                }

                // Don't allow for insert before or in between the fixed input parameters
                if (side == GH_ParameterSide.Input && index < fixedParamNumInput + correct)
                {
                    return false;
                }
                // Don't allow for insert if all variable input parameters are already added
                else if (side == GH_ParameterSide.Input && index == (externalAxisParameters.Length + fixedParamNumInput + correct))
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
                // Correction for the index number if the reference place was already added
                int correct = 0;
                if (Params.Input.Any(x => x.Name == "Reference Plane"))
                {
                    correct += 1;
                }

                // Makes it impossible to remove the fixed input parameters
                if (side == GH_ParameterSide.Input && index < fixedParamNumInput + correct)
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
            // If the first external axis override parameter is added it is allowed to add more parameters
            if (Params.Input.Any(x => x.Name == externalAxisParameters[0].Name))
            {
                // Correction for the index number if the reference place was already added
                int correct = 0;
                if (Params.Input.Any(x => x.Name == "Reference Plane"))
                {
                    correct += 1;
                }

                AddExternalAxisValueParameter(index - fixedParamNumInput - correct);
            }

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
            // If the first external axis override parameter is added it is allowed to destroy parameters
            if (Params.Input.Any(x => x.Name == externalAxisParameters[0].Name))
            {
                // Correction for the index number if the reference place was already added
                int correct = 0;
                if (Params.Input.Any(x => x.Name == "Reference Plane"))
                {
                    correct += 1;
                }

                // Makes it impossible to detroy the fixed input parameters
                if (side == GH_ParameterSide.Input && index < fixedParamNumInput + correct)
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
            CreateVolatileData();
        }
        #endregion

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.Target_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8D2E1FAC-E34A-4B4E-A6F0-D867128686DE"); }
        }

    }
}
