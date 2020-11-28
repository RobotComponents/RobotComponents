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

namespace RobotComponents.Gh.Components.Deconstruct
{
    /// <summary>
    /// RobotComponents Deconstruct External Linear Axis Component. An inherent from the GH_Component Class.
    /// </summary>
    public class OldDeconstructExternalLinearAxisComponent2 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructRobotTool class.
        /// </summary>
        public OldDeconstructExternalLinearAxisComponent2()
          : base("Deconstruct External Linear Axis", "DeConELA", 
              "Deconstructs an External Linear Axis component into its parameters."
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
            pManager.AddParameter(new ExternalLinearAxisParameter(), "External Linear Axis", "ELA", "External Linear Axis as External Linear Axis", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Axis Name as a Text", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Attachment Plane", "AP", "Attachment Plane as Plane", GH_ParamAccess.item);
            pManager.AddVectorParameter("Axis", "A", "Axis as Vector", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Axis Limits", "AL", "Axis Limits as Domain", GH_ParamAccess.item);
            pManager.AddMeshParameter("Base Mesh", "BM", "Base Mesh as Mesh", GH_ParamAccess.item);
            pManager.AddMeshParameter("Link Mesh", "LM", "Link Mesh as Mesh", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            ExternalLinearAxis externalLinearAxis = null;

            // Catch the input data
            if (!DA.GetData(0, ref externalLinearAxis)) { return; }

            // Check if the object is valid
            if (!externalLinearAxis.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The External Linear Axis is not valid");
            }

            // Output
            DA.SetData(0, externalLinearAxis.Name);
            DA.SetData(1, externalLinearAxis.AttachmentPlane);
            DA.SetData(2, externalLinearAxis.AxisPlane.ZAxis);
            DA.SetData(3, externalLinearAxis.AxisLimits);
            DA.SetData(4, externalLinearAxis.BaseMesh);
            DA.SetData(5, externalLinearAxis.LinkMesh);
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructExternalLinearAxis_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2CB287BA-4FD1-44E6-B540-8C423B6CC4B6"); }
        }
    }

}