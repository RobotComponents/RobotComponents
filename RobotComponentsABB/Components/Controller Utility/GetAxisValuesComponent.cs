using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
// RobotComponents Libs
using RobotComponentsABB.Goos;
using RobotComponentsABB.Parameters;

namespace RobotComponentsABB.Components
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
            // To do: replace generic parameter with an RobotComponents Parameter
            pManager.AddGenericParameter("Robot Controller", "RC", "Controller to extract Axis values from", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Internal Axis Values", "IAV", "Extracted internal Axis Values", GH_ParamAccess.list);
            pManager.AddNumberParameter("External Axis Values", "EAV", "Extracted external Axis Values", GH_ParamAccess.list);
            //pManager.AddNumberParameter("Axis Values", "AV", "All the axis values in a datatree structure", GH_ParamAccess.tree);
            //pManager.AddTextParameter("Mechanical Units", "MU", "The names of the mechanical units", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            ControllerGoo controllerGoo = null;
            List<double> internalAxisValues;
            List<double> externalAxisValues;

            // Catch input data
            if (!DA.GetData(0, ref controllerGoo)) { return; }

            // Internal axis values
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

            /**
            // Output variables         
            GH_Structure<GH_Number> axisValues = new GH_Structure<GH_Number>();
            List<string> mechanicalUnitnames = new List<string>() { };

            // Data needed for making the datatree with axis values
            ABB.Robotics.Controllers.MotionDomain.MechanicalUnitCollection mechanicalUnits = controllerGoo.Value.MotionSystem.MechanicalUnits;
            List<double> values;

            // Make the output datatree with names with a branch for each mechanical unit
            for (int i = 0; i < mechanicalUnits.Count; i++)
            {
                // Get the ABB joint target of the mechanical unit
                ABB.Robotics.Controllers.RapidDomain.JointTarget jointTarget = mechanicalUnits[i].GetPosition();

                // For internal axis values
                if (mechanicalUnits[i].Type.ToString() ==  "TcpRobot")
                {
                    values = GetInternalAxisValuesAsList(jointTarget);
                }

                // For external axis values
                else
                {
                    values = GetExternalAxisValuesAsList(jointTarget);
                }

                // Path number of the datatree for storing the axis values
                GH_Path path = new GH_Path(i);

                // Add mechanical unit name to the list with names
                mechanicalUnitnames.Add(mechanicalUnits[i].Name);

                // Save the axis values
                for (int j = 0; j < mechanicalUnits[i].NumberOfAxes; j++)
                {
                    axisValues.Append(new GH_Number(values[j]), path);
                }
            }
            **/

            // Output
            DA.SetDataList(0, internalAxisValues);
            DA.SetDataList(1, externalAxisValues);
            //DA.SetDataTree(2, axisValues);
            //DA.SetDataList(3, mechanicalUnitnames);
        }

        // Additional methods
        #region additional methods
        /// <summary>
        /// Get the internal axis values from a defined joint target
        /// </summary>
        /// <param name="jointTarget"> The joint target to get the internal axis values from. </param>
        /// <returns></returns>
        public List<double> GetInternalAxisValuesAsList(ABB.Robotics.Controllers.RapidDomain.JointTarget jointTarget)
        {
            // Initiate the list with internal axis values
            List<double> result = new List<double>() { };

            // Get the axis values from the joint target
            result.Add(jointTarget.RobAx.Rax_1);
            result.Add(jointTarget.RobAx.Rax_2);
            result.Add(jointTarget.RobAx.Rax_3);
            result.Add(jointTarget.RobAx.Rax_4);
            result.Add(jointTarget.RobAx.Rax_5);
            result.Add(jointTarget.RobAx.Rax_6);

            // Replace large numbers (the not connected axes) with an axis value equal to zero 
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] > 9.0e+8)
                {
                    result[i] = 0;
                }
            }

            // Return the list with axis values
            return result;
        }

        /// <summary>
        /// Get the external axis values from a defined joint target
        /// </summary>
        /// <param name="jointTarget"> The joint target to get the external axis values from. </param>
        /// <returns></returns>
        public List<double> GetExternalAxisValuesAsList(ABB.Robotics.Controllers.RapidDomain.JointTarget jointTarget)
        {
            // Initiate the list with external axis values
            List<double> result = new List<double>() { };

            // Get the axis values from the joint target
            result.Add(jointTarget.ExtAx.Eax_a);
            result.Add(jointTarget.ExtAx.Eax_b);
            result.Add(jointTarget.ExtAx.Eax_c);
            result.Add(jointTarget.ExtAx.Eax_d);
            result.Add(jointTarget.ExtAx.Eax_e);
            result.Add(jointTarget.ExtAx.Eax_f);

            // Replace large numbers (the not connected axes) with an axis value equal to zero 
            for (int i = 0; i < result.Count; i++)
            {
                if(result[i] > 9.0e+8)
                {
                    result[i] = 0;
                }
            }

            // Return the list with axis values
            return result;
        }
        #endregion

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return RobotComponentsABB.Properties.Resources.GetAxisValues_Icon; }
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