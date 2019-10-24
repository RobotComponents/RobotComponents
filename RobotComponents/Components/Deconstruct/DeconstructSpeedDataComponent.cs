using System;

using Rhino.Geometry;
using Grasshopper.Kernel;

using RobotComponents.Goos;

namespace RobotComponents.Components
{
    public class DeconstructSpeedDataComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructRobotTool class.
        /// </summary>
        public DeconstructSpeedDataComponent()
          : base("Deconstruct Speed Data", "DeConSpeed",
              "Deconstructs a Speed Data Component into its parameters."
                + System.Environment.NewLine +
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Deconstruct")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Speed Data", "SD", "Speed Data as Input", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Name as string",GH_ParamAccess.item);
            pManager.Register_DoubleParam("TCP Velocity", "vTCP", "tcp velocity in mm/s as a number");
            pManager.Register_DoubleParam("ORI Velocity", "vORI", "reorientation of the tool in degree/s as a number");
            pManager.Register_DoubleParam("LEAX Velocity", "vLEAX", "linear external axes velocity in mm/s as a number");
            pManager.Register_DoubleParam("REAX Velocity", "vREAX", "reorientation of the external rotational axes in degrees/s as a number");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Variables input
            SpeedDataGoo speedDataGoo = null;

            //Get the data from the input
            if (!DA.GetData(0, ref speedDataGoo)) { return; }

            //Output
            DA.SetData(0, speedDataGoo.Value.Name);
            DA.SetData(1, speedDataGoo.Value.V_TCP);
            DA.SetData(2, speedDataGoo.Value.V_ORI);
            DA.SetData(3, speedDataGoo.Value.V_LEAX);
            DA.SetData(4, speedDataGoo.Value.V_REAX);
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.DeconstructSpeedData_Icon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2A20F8ED-850F-4E01-9318-5548356F8A3A"); }
        }
    }
}