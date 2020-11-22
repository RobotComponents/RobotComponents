// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Definitions;
using RobotComponents.Gh.Utils;
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Gh.Parameters.Actions;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.07.000 (March 2020)
// It is replaced with a new movement component. 

namespace RobotComponents.Gh.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Rapid Generator component. An inherent from the GH_Component Class.
    /// </summary>
    public class OldRAPIDGeneratorComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldRAPIDGeneratorComponent()
          : base("RAPID Generator", "RG",
              "OBSOLETE: Generates the RAPID main and base code for the ABB IRC5 robot controller from a list of Actions."
                + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "RAPID Generation")
        {
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
            pManager.AddParameter(new RobotParameter(), "Robot", "R", "Robot as Robot", GH_ParamAccess.item);
            pManager.AddParameter(new ActionParameter(), "Actions", "A", "Actions as Actions", GH_ParamAccess.list);
            pManager.AddTextParameter("Module Name", "MN", "Name of the Module as String", GH_ParamAccess.item, "MainModule");
            pManager.AddTextParameter("File Path", "FP", "File Path as String", GH_ParamAccess.item, "null");
            pManager.AddBooleanParameter("Save To File", "S", "Saves RAPID Code to File", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Update", "U", "Updates RAPID Code", GH_ParamAccess.item, true);

            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Main Code", "Main", "Robot Instructions in RAPID Code");  //Todo: beef this up to be more informative.
            pManager.Register_StringParam("Base Code", "Base", "Basic defined system data in RAPID Code");  //Todo: beef this up to be more informative.
        }

        // Fields
        private RAPIDGenerator _rapidGenerator;
        private ObjectManager _objectManager;
        private bool _firstMovementIsMoveAbsJ = true;
        private string _BASECode = "";
        private string _MAINCode = "";

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed " +
                "in the future. Remove this component from your canvas and replace it by picking the new component " +
                "from the ribbon.");

            // Gets ObjectManager of this document
            _objectManager = DocumentManager.GetDocumentObjectManager(this.OnPingDocument());

            // Input variables
            Robot robInfo = new Robot();
            List<RobotComponents.Actions.Action> actions = new List<RobotComponents.Actions.Action>();
            string moduleName = "";
            string filePath = "";
            bool saveToFile = false;
            bool update = true;

            // Catch the input data
            if (!DA.GetData(0, ref robInfo)) { return; }
            if (!DA.GetDataList(1, actions)) { return; }
            if (!DA.GetData(2, ref moduleName)) { moduleName = "MainModule"; }
            if (!DA.GetData(3, ref filePath)) { return; }
            if (!DA.GetData(4, ref saveToFile)) { return; }
            if (!DA.GetData(5, ref update)) { return; }

            // Checks if module name exceeds max character limit for RAPID Code
            if (HelperMethods.VariableExeedsCharacterLimit32(moduleName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Module Name exceeds character limit of 32 characters.");
            }

            // Checks if module name starts with a number
            if (HelperMethods.VariableStartsWithNumber(moduleName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Module Name starts with a number which is not allowed in RAPID Code.");
            }

            // Saved file
            bool updated = false; // Avoids saving the file two times in one run
          
            // Updates the rapid BASE and MAIN code 
            if (update == true)
            {
                // Initiaties the rapidGenerator
                _rapidGenerator = new RAPIDGenerator(moduleName, "BASE", actions, filePath, saveToFile, robInfo.Duplicate());

                // Get tools data for system module
                List<RobotTool> robotTools = _objectManager.GetRobotTools(); // Gets all the robot tools from the object manager
                List<WorkObject> workObjects = _objectManager.GetWorkObjects(); // Gets all the work objects from the object manager
                List<string> customCode = new List<string>() { };

                // Generator code
                _rapidGenerator.CreateSystemCode(robotTools, workObjects, customCode);
                _rapidGenerator.CreateProgramCode();
                _MAINCode = _rapidGenerator.ProgramCode;
                _BASECode = _rapidGenerator.SystemCode;

                // Check if the first movement is an absolute joint movement. 
                _firstMovementIsMoveAbsJ = _rapidGenerator.FirstMovementIsMoveAbsJ;

                // Saved file
                updated = true; // Avoids saving the file two times in one run
            }

            // Save to file
            if (saveToFile == true && updated == false) // Avoids saving the file two times in one run
            {
                _rapidGenerator.FilePath = filePath;
                _rapidGenerator.WriteProgramCodeToFile();
                _rapidGenerator.WriteSystemCodeToFile();
            }

            // Checks if first Movement is MoveAbsJ
            if (_firstMovementIsMoveAbsJ == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The first movement is not set as an absolute joint movement.");
            }

            // Output
            DA.SetData(0, _MAINCode);
            DA.SetData(1, _BASECode);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return RobotComponents.Gh.Properties.Resources.RAPID_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("71E8E409-A998-4714-8145-FE2A81973970"); }
        }
    }
}
