// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Definitions;

namespace RobotComponents.ABB.Gh.Components.Deconstruct.Definitions
{
    /// <summary>
    /// RobotComponents Deconstruct Work Object component.
    /// </summary>
    public class DeconstructWorkObjectComponent : GH_RobotComponent
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructWorkObject class.
        /// </summary>
        public DeconstructWorkObjectComponent() : base("Deconstruct Work Object", "DeConWobj", "Deconstruct",
              "Deconstructs a Work Object into its parameters.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_WorkObject(), "Work Object", "WO", "Work Object as Work Object", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Name as string");
            pManager.Register_PlaneParam("Plane", "WP", "Work Object Plane as a Plane");
            pManager.RegisterParam(new Param_ExternalAxis(), "ExternalAxis", "EA", "External Axis as External Axis");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            WorkObject workObject = null;

            // Catch the input data
            if (!DA.GetData(0, ref workObject)) { return; }

            if (workObject != null)
            {
                // Check if the object is valid
                if (!workObject.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Work Object is not valid");
                }

                // Output
                DA.SetData(0, workObject.Name);
                DA.SetData(1, workObject.ObjectFrame);
                DA.SetData(2, workObject.ExternalAxis);
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
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
            get { return Properties.Resources.DeconstructWorkObject_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1E29AF75-A85C-465C-AA44-561570CCD0AE"); }
        }
        #endregion
    }
}