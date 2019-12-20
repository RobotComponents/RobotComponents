using System;
using System.Collections.Generic;

using Rhino.Geometry;
using Grasshopper.Kernel;

using RobotComponents.BaseClasses;

using RobotComponentsABB.Utils;
using RobotComponentsABB.Parameters;

namespace RobotComponentsABB.Components
{
    public class RAPIDGeneratorComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public RAPIDGeneratorComponent()
          : base("RAPID Generator", "RG",
              "Generates the RAPID main and base code for the ABB IRC5 robot controller from a list of Actions."
                + System.Environment.NewLine +
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            // Always place the rapid generator in the last sub category
            get { return GH_Exposure.septenary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new RobotInfoParameter(), "Robot Info", "RI", "Robot Info as Robot Info", GH_ParamAccess.item);
            pManager.AddGenericParameter("Actions", "A", "Actions as Actions", GH_ParamAccess.list);
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

        // Global component variables
        string MAINCode = "";
        string BASECode = "";
        bool firstMovementIsMoveAbs = true;
        ObjectManager _objectManager;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Gets Document ID
            Guid documentGUID = this.OnPingDocument().DocumentID;

  
          // Checks if ObjectManager for this document already exists. If not it creates a new one
          if (!DocumentManager.ObjectManagers.ContainsKey(documentGUID))
          {
              DocumentManager.ObjectManagers.Add(documentGUID, new ObjectManager());
          }

          // Gets ObjectManager of this document
          _objectManager = DocumentManager.ObjectManagers[documentGUID];

            // Input variables
            RobotInfo robInfo = new RobotInfo();
            List<RobotComponents.BaseClasses.Action> actions = new List<RobotComponents.BaseClasses.Action>();
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

            // Sets the current Robot Tool in the object Manager
            _objectManager.CurrentTool = robInfo.Tool.Name;

            // Initiaties the rapidGenerator
            RAPIDGenerator rapidGenerator = new RAPIDGenerator(moduleName, actions, filePath, saveToFile, robInfo, documentGUID);

            // Updates the rapid BASE and MAIN code 
            if(update == true)
            {
                string toolBaseCode = GetToolBaseCode();
                rapidGenerator.CreateBaseCode(toolBaseCode);
                rapidGenerator.CreateRAPIDCode();
                MAINCode = rapidGenerator.RAPIDCode;
                BASECode = rapidGenerator.BASECode;
                firstMovementIsMoveAbs = rapidGenerator.FirstMovementIsMoveAbs;
            }

            // Checks if first Movement is MoveAbsJ
            if (firstMovementIsMoveAbs == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "First movement is defined as a linear movement.");
            }

            // Output
            DA.SetData(0, MAINCode);
            DA.SetData(1, BASECode);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return RobotComponentsABB.Properties.Resources.RAPID_Icon; }
        }

        /// <summary>
        /// Gets the Base Code for all RobotTools from the object manager.
        /// </summary>
        /// <returns></returns>
        private string GetToolBaseCode()
        {
            string BASECode = "";

            foreach (KeyValuePair<Guid, RobotToolFromDataEulerComponent> entry in _objectManager.ToolsEulerByGuid)
            {
                string toolData = "";
                double posX = entry.Value.robTool.AttachmentPlane.Origin.X + entry.Value.robTool.ToolPlane.Origin.X;
                double posY = entry.Value.robTool.AttachmentPlane.Origin.Y + entry.Value.robTool.ToolPlane.Origin.Y;
                double posZ = entry.Value.robTool.AttachmentPlane.Origin.Z + entry.Value.robTool.ToolPlane.Origin.Z;
                Point3d position = new Point3d(posX, posY, posZ);
                Quaternion orientation = entry.Value.robTool.Orientation;
                string name = entry.Value.robTool.Name;
                toolData += " PERS tooldata " + name + " := [TRUE, [["
                    + position.X.ToString("0.##") + ","
                    + position.Y.ToString("0.##") + ","
                    + position.Z.ToString("0.##") + "], ["
                    + orientation.A.ToString("0.######") + ","
                    + orientation.B.ToString("0.######") + ","
                    + orientation.C.ToString("0.######") + ","
                    + orientation.D.ToString("0.######") + "]],@";
                toolData += "\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "[0.001, [0, 0, 0.001],[1, 0, 0, 0], 0, 0, 0]];@@";

                BASECode += toolData;
            }

            foreach (KeyValuePair<Guid, RobotToolFromPlanesComponent> entry in _objectManager.ToolsPlanesByGuid)
            {
                string toolData = "";
                double posX = entry.Value.robTool.AttachmentPlane.Origin.X + entry.Value.robTool.ToolPlane.Origin.X;
                double posY = entry.Value.robTool.AttachmentPlane.Origin.Y + entry.Value.robTool.ToolPlane.Origin.Y;
                double posZ = entry.Value.robTool.AttachmentPlane.Origin.Z + entry.Value.robTool.ToolPlane.Origin.Z;
                Point3d position = new Point3d(posX, posY, posZ);
                Quaternion orientation = entry.Value.robTool.Orientation;
                string name = entry.Value.robTool.Name;
                toolData += " PERS tooldata " + name + " := [TRUE, [["
                    + position.X.ToString("0.##") + ","
                    + position.Y.ToString("0.##") + ","
                    + position.Z.ToString("0.##") + "], ["
                    + orientation.A.ToString("0.######") + ", "
                    + orientation.B.ToString("0.######") + ","
                    + orientation.C.ToString("0.######") + ","
                    + orientation.D.ToString("0.######") + "]],@";
                toolData += "\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "[0.001, [0, 0, 0.001],[1, 0, 0, 0], 0, 0, 0]];@@";

                BASECode += toolData;
            }

            return BASECode;
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
