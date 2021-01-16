// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Definitions;
using RobotComponents.Gh.Parameters.Definitions;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.13.000
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Deconstruct Work Object component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldDeconstructWorkObjectComponent2 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructWorkObject class.
        /// </summary>
        public OldDeconstructWorkObjectComponent2()
          : base("Deconstruct Work Object", "DeConTar", 
              "Deconstructs a Work Object into its parameters."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Deconstruct")
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
            pManager.AddParameter(new WorkObjectParameter(), "Work Object", "WO", "Work Object as Work Object", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Name as string");
            pManager.Register_PlaneParam("Plane", "WP", "Work Object Plane as a Plane");
            pManager.RegisterParam(new ExternalRotationalAxisParameter(), "External Rotational Axis", "ERA", "External Rotational Axis as External Rotational Axis");
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

            // Check if the object is valid
            if (!workObject.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Work Object is not valid");
            }

            // Output
            DA.SetData(0, workObject.Name);
            DA.SetData(1, workObject.Plane);
            DA.SetData(2, workObject.ExternalAxis);
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
            get { return new Guid("2EF73719-3E07-431E-8729-AB0C99848D0A"); }
        }
    }
}