using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data; 

using RobotComponents.BaseClasses;
using RobotComponents.Goos;
using RobotComponents.Parameters;
using RobotComponents.Utils;

namespace RobotComponents.Components
{
    public class MovementComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public MovementComponent()
          : base("Action: Movement", "M",
              "Defines a linear or nonlinear movement instruction for simulation and code generation."
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
            pManager.AddGenericParameter("Target", "T", "Target as Action: Target", GH_ParamAccess.list);
            pManager.AddGenericParameter("Speed Data", "SD", "Speed Data as Action: Speed Data or as a number (vTCP)", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Is Linear", "IL", "Movement Type as bool", GH_ParamAccess.list, false);
            pManager.AddIntegerParameter("Precision", "P", "Precision as int. If value is smaller than 0, precision will be set to fine.", GH_ParamAccess.list, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new MovementParameter(), "Movement", "JM", "Robot Movement Instructions");  //Todo: beef this up to be more informative.
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

            List<TargetGoo> targetGoos = new List<TargetGoo>();
            List<IGH_Goo> inputSpeedDatas = new List<IGH_Goo>();
            List<bool> isLinears = new List<bool>();
            List<int> precisions = new List<int>();

            if (!DA.GetDataList(0, targetGoos)) { return; }
            if (!DA.GetDataList(1, inputSpeedDatas)) { return; }
            if (!DA.GetDataList(2, isLinears)) { return; }
            if (!DA.GetDataList(3, precisions)) { return; }

            bool speedValueWarningRaised = false;
            List<SpeedData> speedDatas = new List<SpeedData>();

            // Check if input is speedata or a single tcp speed
            for (int i = 0; i < inputSpeedDatas.Count; i++)
            {
                // If input is a double: create pre-defined speed data
                if (inputSpeedDatas[i] is GH_Number)
                {
                    GH_Number speedNumber = inputSpeedDatas[i] as GH_Number;
                    double speedValue = speedNumber.Value;
                    SpeedData speedData = new SpeedData(speedValue);
                    speedDatas.Add(speedData);

                    // Check value
                    if (speedValueWarningRaised == false)
                    {
                        if (HelperMethods.PredefinedSpeedValueIsValid(speedValue) == false)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Pre-defined speed value <" + speedValue.ToString() +
                                "> is invalid. Use the speed data component to create custom speed data or use of one of the valid pre-defined speed datas. " +
                                "Pre-defined speed data can be set to 5, 10, 20, 30, 40, 50, 60, 80, 100, 150, 200, 300, " +
                                "400, 500, 600, 800, 1000, 1500, 2000, 2500, 3000, 4000, 5000, 6000 or 7000.");

                            speedValueWarningRaised = true;
                        }
                    }
                }
                // Else process the speeddata that is used as input
                else if (inputSpeedDatas[i] is SpeedDataGoo)
                {
                    SpeedDataGoo speedDataGoo = inputSpeedDatas[i] as SpeedDataGoo;
                    SpeedData speedData = speedDataGoo.Value;
                    speedDatas.Add(speedData);
                }
                // Wrong input is used
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The wrong datatype is used as an input for the speed data. " +
                        "Use a number or create custom speed data with the speed data component.");
                }
            }

            // Get longest Input List
            int[] sizeValues = new int[4];
            sizeValues[0] = targetGoos.Count;
            sizeValues[1] = speedDatas.Count;
            sizeValues[2] = isLinears.Count;
            sizeValues[3] = precisions.Count;
            int biggestSize = HelperMethods.GetBiggestValue(sizeValues);

            // Keeps track of used indicies
            int targetGooCounter = -1;
            int speedDataCounter = -1;
            int isLinearCounter = -1;
            int precisionCounter = -1;

            // Creates movements
            List<Movement> movements = new List<Movement>();

            for (int i = 0; i < biggestSize; i++)
            {
                TargetGoo targetGoo = null;
                SpeedData speedData = null;
                bool isLinear = false;
                int precision = 0;

                if (i < targetGoos.Count)
                {
                    targetGoo = targetGoos[i];
                    targetGooCounter++;
                }
                else
                {
                    targetGoo = targetGoos[targetGooCounter];
                }

                if (i < speedDatas.Count)
                {
                    speedData = speedDatas[i];
                    speedDataCounter++;
                }
                else
                {
                    speedData = speedDatas[speedDataCounter];
                }

                if (i < isLinears.Count)
                {
                    isLinear = isLinears[i];
                    isLinearCounter++;
                }
                else
                {
                    isLinear = isLinears[isLinearCounter];
                }

                if (i < precisions.Count)
                {
                    precision = precisions[i];
                    precisionCounter++;
                }
                else
                {
                    precision = precisions[precisionCounter];
                }

                Movement movement = new Movement(targetGoo.Value, speedData, isLinear, precision, documentGUID);
                movements.Add(movement);
            }

            for (int i = 0; i < precisions.Count; i++)
            {
                if (HelperMethods.PrecisionValueIsValid(precisions[i]) == false)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Precision value <" + i + "> is invalid. " +
                        "In can only be set to -1, 0, 1, 5, 10, 15, 20, 30, 40, 50, 60, 80, 100, 150 or 200. " +
                        "A value of -1 will be interpreted as fine movement in RAPID Code.");
                    break;
                }
            }

            DA.SetDataList(0, movements);
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
                //return Resources.IconForThisComponen;
                return Properties.Resources.Movement_Icon;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F2BBBB2D-96F7-4D65-9031-A6C08D14A448"); }
        }
    }

}
