// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
//Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Definitions;
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Kinematics;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.10.000
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Inveser Kinematics component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldInverseKinematicsComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldInverseKinematicsComponent()
          : base("Inverse Kinematics", "IK",
              "Computes the axis values for a defined ABB robot based on an Action: Target."
      + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Simulation")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return true; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new RobotParameter(), "Robot", "R", "Robot as Robot", GH_ParamAccess.item);
            pManager.AddParameter(new MovementParameter(), "Movement", "M", "Movement or target input. A target will automatically be casted to a movement.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("Internal Axis Values", "IAV", "Internal Axis Values");
            pManager.Register_DoubleParam("External Axis Values", "EAV", "External Axis Values");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed " +
                "in the future. Remove this component from your canvas and replace it by picking the new component " +
                "from the ribbon.");

            // Input variables
            Robot robot = null;
            Movement Movement = null;

            // Catch the input data
            if (!DA.GetData(0, ref robot)) { return; }
            if (!DA.GetData(1, ref Movement)) { return; }

            // Calculate the robot pose
            InverseKinematics inverseKinematics = new InverseKinematics(Movement, robot);
            inverseKinematics.Calculate();

            // Check the values
            for (int i = 0; i < inverseKinematics.ErrorText.Count; i++)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, inverseKinematics.ErrorText[i]);
            }

            // Create list with externan axis values
            List<double> externalAxisValues = inverseKinematics.ExternalJointPosition.ToList();
            externalAxisValues.RemoveAll(val => val == 9e9);

            // Output
            DA.SetDataList(0, inverseKinematics.RobotJointPosition.ToList());
            DA.SetDataList(1, externalAxisValues);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.InverseKinematics_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0F1746B8-4E3D-4A22-8719-F7B42C2313AA"); }
        }
    }

}