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
    /// RobotComponents Action : Zone Data component. An inherent from the GH_Component Class.
    /// This is a dummy component that is hidden in Grasshopper. It is only called and used in
    /// the background to create a datatree structure that follows the Grasshopper logic.
    /// </summary>
    public class ZoneDataComponentDataTreeGenerator : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public ZoneDataComponentDataTreeGenerator()
          : base("Zone Data", "ZD",
              "Defines a zone data declaration for robot movements in RAPID program code generation."
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
            get { return GH_Exposure.hidden; } // This component is hidden. It is only used to create a datatree inside the real Zone Data component (according to the Grasshopper logic)
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name of the Zone Data as text", GH_ParamAccess.item, "default_zone");
            pManager.AddBooleanParameter("Fine Point", "FP", "Defines whether the movement is to terminate as a stop point (fine point) or as a fly-by point as a bool.", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Path Zone TCP", "pzTCP", "The size (the radius) of the TCP zone in mm as a number.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Path Zone Reorientation", "pzORI", "The zone size (the radius) for the tool reorientation in mm as a number. ", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Path Zone External Axes", "pzEA", "The zone size (the radius) for external axes in mm as a number.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Zone Reorientation", "zORI", "The zone size for the tool reorientation in degrees as a number.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Zone External Linear Axes", "zELA", "The zone size for linear external axes in mm as a number.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Zone External Rotational Axes", "zERA", "The zone size for rotating external axes in degrees as a number.", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new ZoneDataParameter(), "Zone Data", "ZD", "Resulting Zone Data declaration");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Sets inputs
            string name = "";
            bool finep = false;
            double pzone_tcp = 0;
            double pzone_ori = 0;
            double pzone_eax = 0;
            double zone_ori = 0;
            double zone_leax = 0;
            double zone_reax = 0;

            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref finep)) { return; }
            if (!DA.GetData(2, ref pzone_tcp)) { return; }
            if (!DA.GetData(3, ref pzone_ori)) { return; }
            if (!DA.GetData(4, ref pzone_eax)) { return; }
            if (!DA.GetData(5, ref zone_ori)) { return; }
            if (!DA.GetData(6, ref zone_leax)) { return; }
            if (!DA.GetData(7, ref zone_reax)) { return; }

            // Replace spaces
            name = HelperMethods.ReplaceSpacesAndRemoveNewLines(name);

            ZoneData zoneData = new ZoneData(name, finep, pzone_tcp, pzone_ori, pzone_eax, zone_ori, zone_leax, zone_reax);

            // Sets Output
            DA.SetData(0, zoneData);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            { return Properties.Resources.ZoneData_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("78239012-472E-4649-AE6C-6C42EC4024E7"); }
        }
    }
}

