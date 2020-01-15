using System;

using Grasshopper.Kernel;

using RobotComponents.BaseClasses;

using RobotComponentsABB.Parameters;

namespace RobotComponentsABB.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Wait for Digital Input component. An inherent from the GH_Component Class.
    /// </summary>
    public class WaitDIComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public WaitDIComponent()
          : base("Action: Wait for Digital Input", "WDI",
              "Defines an instruction to wait for the signal of a Digital Input."
                + System.Environment.NewLine +
                "RobotComponents : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
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
            pManager.AddTextParameter("DI Name", "N", "Digital Input Name as string.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("State", "S", "Digital Input State as Boolean.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new WaitDIParameter(), "Wait DI", "WDI", "Resulting Wait DI");  //Todo: beef this up to be more informative.
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            string _DIName = "";
            bool _DIValue = false;

            // Catch the input data
            if (!DA.GetData(0, ref _DIName)) { return; }
            if (!DA.GetData(1, ref _DIValue)) { return; }

            // Create the action
            WaitDI waitDI = new WaitDI(_DIName, _DIValue);

            // Sets Output
            DA.SetData(0, waitDI);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.WaitDI_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F3472EF2-0B4A-4231-B716-105C4EB11AC6"); }
        }

    }

}
