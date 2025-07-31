// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;

namespace RobotComponents.ABB.Gh.Components.Deconstruct.CodeGeneration
{
    /// <summary>
    /// RobotComponents Deconstruct External Joint Position component. 
    /// </summary>
    public class DeconstructExternalJointPositionComponent : GH_RobotComponent
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructExternalJointPosition class.
        /// </summary>
        public DeconstructExternalJointPositionComponent() : base("Deconstruct External Joint Position", "DeConExtJoint", "Deconstruct",
              "Deconstructs an External Joint Position Component into its parameters.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_ExternalJointPosition(), "External Joint Position", "EJ", "The External Joint Position.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "External joint position variable name as text");
            pManager.Register_DoubleParam("External joint position A", "EJa", "Defines the position of the external logical axis A");
            pManager.Register_DoubleParam("External joint position B", "EJb", "Defines the position of the external logical axis B");
            pManager.Register_DoubleParam("External joint position C", "EJc", "Defines the position of the external logical axis C");
            pManager.Register_DoubleParam("External joint position D", "EJd", "Defines the position of the external logical axis D");
            pManager.Register_DoubleParam("External joint position E", "EJe", "Defines the position of the external logical axis E");
            pManager.Register_DoubleParam("External joint position F", "EJf", "Defines the position of the external logical axis F");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            ExternalJointPosition extJointPosition = null;

            // Catch the input data
            if (!DA.GetData(0, ref extJointPosition)) { return; }

            if (extJointPosition != null)
            {
                // Check if the object is valid
                if (!extJointPosition.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The External Joint Position is not valid");
                }

                // Output
                DA.SetData(0, extJointPosition.Name);
                DA.SetData(1, extJointPosition[0]);
                DA.SetData(2, extJointPosition[1]);
                DA.SetData(3, extJointPosition[2]);
                DA.SetData(4, extJointPosition[3]);
                DA.SetData(5, extJointPosition[4]);
                DA.SetData(6, extJointPosition[5]);
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructExternalJointPosition_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("DF0B0707-E0A1-4978-8FFD-FF7FE916AF6E"); }
        }
        #endregion
    }
}