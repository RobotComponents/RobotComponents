// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Speed Data component. An inherent from the GH_Component Class.
    /// This is a dummy component that is hidden in Grasshopper. It is only called and used in
    /// the background to create a datatree structure that follows the Grasshopper logic.
    /// </summary>
    public class SpeedDataComponentDataTreeGenerator : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public SpeedDataComponentDataTreeGenerator()
          : base("Speed Data 2", "SD2", 
              "Defines a speed data declaration for Move components."
               + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; } // This component is hidden. It is only used to create a datatree inside the real Speed Data component (according to the Grasshopper logic)
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name of the Speed Data as text", GH_ParamAccess.item, "default_speed");
            pManager.AddNumberParameter("TCP Velocity", "vTCP", "TCP Velocity in mm/s as number", GH_ParamAccess.item);
            pManager.AddNumberParameter("ORI Velocity", "vORI", "Reorientation Velocity of the tool in degree/s as number", GH_ParamAccess.item, 500);
            pManager.AddNumberParameter("LEAX Velocity", "vLEAX", "Linear External Axes Velocity in mm/s", GH_ParamAccess.item, 5000);
            pManager.AddNumberParameter("REAX Velocity", "vREAX", "Reorientation of the External Rotational Axes in degrees/s", GH_ParamAccess.item, 1000);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new SpeedDataParameter(), "Speed Data", "SD", "Resulting Speed Data declaration");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Sets inputs 
            string name = "";
            double v_tcp = 5;
            double v_ori = 500;
            double v_leax = 5000;
            double v_reax = 1000;

            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref v_tcp)) { return; }
            if (!DA.GetData(2, ref v_ori)) { return; }
            if (!DA.GetData(3, ref v_leax)) { return; }
            if (!DA.GetData(4, ref v_reax)) { return; }

            // Replace spaces
            name = HelperMethods.ReplaceSpacesAndRemoveNewLines(name);

            SpeedData speeddata = new SpeedData(name, v_tcp, v_ori, v_leax, v_reax);

            // Sets Output
            DA.SetData(0, speeddata);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.SpeedData_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4AAEE8A9-A683-4DE0-83D2-4C56C7ADE6DF"); }
        }
    }
}

