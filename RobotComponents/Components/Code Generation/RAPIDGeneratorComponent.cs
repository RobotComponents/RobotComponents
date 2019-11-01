﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;

using RobotComponents.BaseClasses;
using RobotComponents.Utils;


namespace RobotComponents.Components
{

    public class RAPIDGeneratorComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
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
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot Info", "RI", "RobotInfo as Robot Info", GH_ParamAccess.item);
            pManager.AddGenericParameter("Actions", "A", "Actions as Actions", GH_ParamAccess.list);
            pManager.AddTextParameter("Module Name", "MN", "Name of the Module as String", GH_ParamAccess.item, "MainModule");
            pManager.AddTextParameter("File Path", "FP", "File Path as String", GH_ParamAccess.item, "null");
            pManager.AddBooleanParameter("Save To File", "S", "Saves RAPID Code to File", GH_ParamAccess.item, false);

            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Main Code", "Main", "Robot Instructions in RAPID Code");  //Todo: beef this up to be more informative.
            pManager.Register_StringParam("Base Code", "Base", "Basic defined system data in RAPID Code");  //Todo: beef this up to be more informative.
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Gets Document ID
            Guid documentGUID = this.OnPingDocument().DocumentID;

            RobotInfo robInfo = new RobotInfo();
            List<RobotComponents.BaseClasses.Action> actions = new List<RobotComponents.BaseClasses.Action>();
            string moduleName = "";
            string filePath = "";
            bool saveToFile = false;

            if (!DA.GetData(0, ref robInfo)) { return; }
            if (!DA.GetDataList(1, actions)) { return; }
            if (!DA.GetData(2, ref moduleName)) { moduleName = "MainModule"; }
            if (!DA.GetData(3, ref filePath)) { return; }
            if (!DA.GetData(4, ref saveToFile)) { return; }

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

            RAPIDGenerator rapidGenerator = new RAPIDGenerator(moduleName, actions, filePath, saveToFile, robInfo, documentGUID);

            // Checks if first Movement is MoveAbsJ
            if (rapidGenerator.FirstMovementIsMoveAbs == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "First movement is defined as a linear movement.");
            }

            DA.SetData(0, rapidGenerator.RAPIDCode);
            DA.SetData(1, rapidGenerator.BASECode);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.RAPID_Icon;
            }
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