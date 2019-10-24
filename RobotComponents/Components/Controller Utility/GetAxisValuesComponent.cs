using System;
using System.Collections.Generic;

using Grasshopper.Kernel;

using RobotComponents.Goos;

namespace RobotComponents.Components
{
    public class GetAxisValuesComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetAxisValues class.
        /// </summary>
        public GetAxisValuesComponent()
          : base("Get Axis Values", "GA",
              "Gets the current robot axis values from a defined ABB robot controller."
                + System.Environment.NewLine +
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Controller Utility")
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
            pManager.AddNumberParameter("InternalAxisValues", "IAV", "Extracted internal Axis Values", GH_ParamAccess.list);
            pManager.AddNumberParameter("ExternalAxisValues", "EAV", "Extracted external Axis Values", GH_ParamAccess.list);
            //pManager.AddTextParameter("debug", "D", "Debug Message", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declair Variables
            ControllerGoo controllerGoo = null;
            //ABB.Robotics.Controllers.Controller controller = null;
            List<double> internalAxisValues = new List<double>();
            List<double> externalAxisValues = new List<double>();

            // retrieve data from inputs
            if (!DA.GetData(0, ref controllerGoo)) { return; }

            // Interal axis values
            internalAxisValues = GetInternalAxisValuesAsList(controllerGoo.Value.MotionSystem.MechanicalUnits[0].GetPosition());

            // Try to get the external axis values: if there is no external axis connected the mechanical unit does not exist
            if (controllerGoo.Value.MotionSystem.MechanicalUnits.Count > 1)
            {
                externalAxisValues = GetExternalAxisValuesAsList(controllerGoo.Value.MotionSystem.MechanicalUnits[1].GetPosition());
            }
            // If there is not external axis connected set all the axis values equal to zero. 
            else
            {
                externalAxisValues = new List<double> {0, 0, 0, 0, 0, 0};
            }

            // Output
            DA.SetDataList(0, internalAxisValues);
            DA.SetDataList(1, externalAxisValues);
        }

        //  ----- Additional Functions -----
        #region Additional Functions

        /// <summary>
        /// Get Axis Values from Joint Target
        /// </summary>
        /// <param name="jointPosition"></param>
        /// <returns></returns>
        public List<double> GetInternalAxisValuesAsList(ABB.Robotics.Controllers.RapidDomain.JointTarget jointPosition)
        {
            List<double> result = new List<double>();

            result.Add(jointPosition.RobAx.Rax_1);
            result.Add(jointPosition.RobAx.Rax_2);
            result.Add(jointPosition.RobAx.Rax_3);
            result.Add(jointPosition.RobAx.Rax_4);
            result.Add(jointPosition.RobAx.Rax_5);
            result.Add(jointPosition.RobAx.Rax_6);

            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] > 9.0e+8)
                {
                    result[i] = 0;
                }
            }

            return result;
        }

        public List<double> GetExternalAxisValuesAsList(ABB.Robotics.Controllers.RapidDomain.JointTarget jointPosition)
        {
            List<double> result = new List<double>();

            result.Add(jointPosition.ExtAx.Eax_a);
            result.Add(jointPosition.ExtAx.Eax_b);
            result.Add(jointPosition.ExtAx.Eax_c);
            result.Add(jointPosition.ExtAx.Eax_d);
            result.Add(jointPosition.ExtAx.Eax_e);
            result.Add(jointPosition.ExtAx.Eax_f);

            for (int i = 0; i < result.Count; i++)
            {
                if(result[i] > 9.0e+8)
                {
                    result[i] = 0;
                }
            }

            return result;
        }
        #endregion

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.GetAxisValues_Icon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("691a3c83-114a-4c80-81b9-2e1407004a24"); }
        }
    }
}