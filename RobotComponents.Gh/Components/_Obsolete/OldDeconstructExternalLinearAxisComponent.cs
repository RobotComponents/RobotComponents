// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Gh.Goos.Definitions;
using RobotComponents.Gh.Parameters.Definitions;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.06.000 (February 2020)
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Deconstruct External Linear Axis Component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldDeconstructExternalLinearAxisComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructRobotTool class.
        /// </summary>
        public OldDeconstructExternalLinearAxisComponent()
          : base("Deconstruct External Linear Axis", "DeConELA",
              "Deconstructs an External Linear Axis into its parameters."
                + System.Environment.NewLine +
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
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed " +
                "in the future. Remove this component from your canvas and replace it by picking the new component " +
                "from the ribbon.");

            // Input variables
            GH_ExternalLinearAxis externalLinearAxisGoo = null;

            // Catch the input data
            if (!DA.GetData(0, ref externalLinearAxisGoo)) { return; }

            // Output
            DA.SetData(0, externalLinearAxisGoo.Value.AttachmentPlane);
            DA.SetData(1, externalLinearAxisGoo.Value.AxisPlane.ZAxis);
            DA.SetData(2, externalLinearAxisGoo.Value.AxisLimits);
            DA.SetData(3, externalLinearAxisGoo.Value.BaseMesh);
            DA.SetData(4, externalLinearAxisGoo.Value.LinkMesh);
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
            get { return new Guid("4E61CB2C-A7FE-43F8-9C61-616830FF57A1"); }
        }
    }

}