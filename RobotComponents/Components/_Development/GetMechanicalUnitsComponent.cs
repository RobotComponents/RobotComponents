using System;
using System.Collections.Generic;

using Grasshopper.Kernel;

using RobotComponents.Goos;

namespace RobotComponents.Components
{
    public class GetMechanicalUnitsComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetAxisValues class.
        /// </summary>
        public GetMechanicalUnitsComponent()
          : base("Get Mechanical Units", "GMU",
              "Gets the current mechanical units from a defined ABB robot controller."
                + System.Environment.NewLine +
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "_Development")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot Controller", "RC", "Controller to extract Axis values from", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Mechanical units", "MU", "The connected mechanical units", GH_ParamAccess.list);
            pManager.AddNumberParameter("Number of axes", "N", "Number of axes", GH_ParamAccess.list);
            pManager.AddTextParameter("Model", "M", "Name of the model", GH_ParamAccess.list);
            pManager.AddTextParameter("Task", "T", "Name of the task", GH_ParamAccess.list);
            pManager.AddTextParameter("Work Object", "WO", "Name of the work object", GH_ParamAccess.list);
            pManager.AddTextParameter("Type", "T", "Type", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declair Variables
            ControllerGoo controllerGoo = null;
            List<string> names = new List<string>();
            List<double> number = new List<double>();
            List<string> model = new List<string>();
            List<string> task = new List<string>();
            List<string> workobject = new List<string>();
            List<string> types = new List<string>();

            // retrieve data from inputs
            if (!DA.GetData(0, ref controllerGoo)) { return; }

            var mechanicalUnits = controllerGoo.Value.MotionSystem.MechanicalUnits;

            for (int i = 0; i < mechanicalUnits.Count; i++)
            {
                names.Add(mechanicalUnits[i].Name);
                number.Add(mechanicalUnits[i].NumberOfAxes);
                model.Add(mechanicalUnits[i].Model);
                task.Add(mechanicalUnits[i].Task.Name);
                workobject.Add(mechanicalUnits[i].WorkObject.Name);
                types.Add(mechanicalUnits[i].Type.ToString());
            }

            // Output
            DA.SetDataList(0, names);
            DA.SetDataList(1, number);
            DA.SetDataList(2, model);
            DA.SetDataList(3, task);
            DA.SetDataList(4, workobject);
            DA.SetDataList(5, types);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("A3CBC509-9465-46E1-9216-B858D05123C0"); }
        }
    }
}