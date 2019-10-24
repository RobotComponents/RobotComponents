using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;
using RobotComponents.Components;
using RobotComponents.Goos;
using RobotComponents.Parameters;
using RobotComponents.Utils;



namespace RobotComponents.Components
{

    public class DigitalOutputComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public DigitalOutputComponent()
          : base("Action: Digital Output", "DO",
              "Defines an instruction to change the state of a digital output of the robot controller for RAPID code generation."
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
            pManager.AddTextParameter("Name", "N", "Name as Sring", GH_ParamAccess.item);
            pManager.AddBooleanParameter("State", "S", "State as Bool", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new DigitalOutputParameter(), "Digital Output", "DO", "Digital Output");  //Todo: beef this up to be more informative.
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "";
            bool isActive = false;

            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref isActive)) { return; }

            // Checks if Digital Output Name exceeds max character limit for RAPID Code
            if (HelperMethods.VariableExeedsCharacterLimit32(name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Digital Output Name exceeds character limit of 32 characters.");
            }

            // Checks if variable name starts with a number
            if (HelperMethods.VariableStartsWithNumber(name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Digital Output Name starts with a number which is not allowed in RAPID Code.");
            }

            DigitalOutput digitalOutput = new DigitalOutput(name, isActive);
            DA.SetData(0, digitalOutput);
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
                return Properties.Resources.DigitalOutput_Icon;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("75014DB2-351F-4A79-9917-DEA7DAD89BEF"); }
        }
    }

}
