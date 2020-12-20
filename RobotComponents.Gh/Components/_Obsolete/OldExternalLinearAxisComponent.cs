// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Definitions;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.06.000 (February 2020)
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Definitions
{
    /// <summary>
    /// RobotComponents External Linear Axis component. An inherent from the GH_Component Class.
    /// </summary>
    public class OldExternalLinearAxisComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldExternalLinearAxisComponent()
          : base("External Linear Axis", "External Linear Axis",
              "Defines an External Linear Axis for any Robot."
                + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Definitions")
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
            pManager.AddPlaneParameter("Attachment plane", "AP", "Attachement plane of robot. Overrides robot position plane.", GH_ParamAccess.item);
            pManager.AddVectorParameter("Axis", "A", "Axis as Vector", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Axis Limits", "AL", "Axis Limits as Domain", GH_ParamAccess.item);
            pManager.AddMeshParameter("Base Mesh", "BM", "Base Mesh as Mesh", GH_ParamAccess.list);
            pManager.AddMeshParameter("Link Mesh", "LM", "Link Mesh as Mesh", GH_ParamAccess.list);

            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new ExternalLinearAxisParameter(), "External Linear Axis", "ELA", "Resulting External Linear Axis");  //Todo: beef this up to be more informative.
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed " +
                "in the future. Remove this component from your canvas and replace it by picking the new component " +
                "from the ribbon.");

            // Input variables
            Plane attachmentPlane = Plane.WorldXY;
            Vector3d axis = new Vector3d(0,0,0);
            Interval limits = new Interval(0, 0);
            List<Mesh> baseMeshes = new List<Mesh>();
            List<Mesh> linkMeshes = new List<Mesh>();
            
            // Catch the input data
            if (!DA.GetData(0, ref attachmentPlane)) { return; }
            if (!DA.GetData(1, ref axis)) { return; }
            if (!DA.GetData(2, ref limits)) { return; }
            if (!DA.GetDataList(3, baseMeshes)) {  }
            if (!DA.GetDataList(4, linkMeshes)) {  }

            // Make variables needed to join the base and link to one mesh
            Mesh baseMesh = new Mesh();
            Mesh linkMesh = new Mesh();

            // Join the base meshes to one mesh
            for (int i = 0; i < baseMeshes.Count; i++)
            {
                baseMesh.Append(baseMeshes[i]);
            }

            // Join the link meshes to one mesh
            for (int i = 0; i < linkMeshes.Count; i++)
            {
                linkMesh.Append(linkMeshes[i]);
            }

            // Create the external linear axis
            ExternalLinearAxis externalLinearAxis = new ExternalLinearAxis(attachmentPlane, axis, limits, baseMesh, linkMesh);

            // Output
            DA.SetData(0, externalLinearAxis);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.ExternalLinearAxis_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("C9916C52-8351-4883-9CC8-790C313A942E"); }
        }
    }
}
