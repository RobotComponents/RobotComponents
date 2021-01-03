// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Definitions;
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Deconstruct Robot Info component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldDeconstructrobotComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructrobotComponent class.
        /// </summary>
        public OldDeconstructrobotComponent()
          : base("Deconstruct Robot Info", "DeRobInfo",
              "Deconstructs a robot info definition into its constituent parts"
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
            pManager.AddParameter(new RobotParameter(), "Robot", "R", "Robot as Robot", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Robot Name as String", GH_ParamAccess.item);
            pManager.AddMeshParameter("Meshes", "M", "Robot Meshes as Mesh List", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Axis Planes", "AP", "Axis Planes as Plane List", GH_ParamAccess.list);
            pManager.AddIntervalParameter("Axis Limits", "AL", "Axis Limits as Interval List", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Position Plane", "PP", "Position Plane of the Robot as Plane", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Mounting Frame", "MF", "Mounting Frame as Frame", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Tool Plane", "TP", "Tool Plane (TCP) as Frame", GH_ParamAccess.item);
            pManager.RegisterParam(new RobotToolParameter(), "Robot Tool", "RT", "Robot Tool", GH_ParamAccess.item);
            pManager.RegisterParam(new ExternalAxisParameter(), "External Axes", "EA", "External Axes as External Axis Parameter", GH_ParamAccess.list);
        }

        // Meshes
        private List<Mesh> _meshes = new List<Mesh>() { };
        private GH_Document _doc;

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

            // Get the Grasshopper document
            _doc = this.OnPingDocument();

            // Input variables
            Robot robot = null;

            // Catch the input data
            if (!DA.GetData(0, ref robot)) { return; }

            // Check if the input is valid
            if (!robot.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Robot Info is not Valid");
            }

            // Output meshes (only link meshes, no robot tool)
            List<Mesh> meshes = new List<Mesh>();

            // Add display meshes
            _meshes.Clear();
            if (robot.Meshes != null)
            {
                for (int i = 0; i < 7; i++)
                {
                    _meshes.Add(robot.Meshes[i]);
                    meshes.Add(robot.Meshes[i]);
                }
            }

            if (robot.Tool.IsValid)
            {
                _meshes.Add(robot.Tool.Mesh);
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Robot Tool is not Valid");
            }

            for (int i = 0; i < robot.ExternalAxes.Count; i++)
            {
                if (robot.ExternalAxes[i].IsValid)
                {
                    _meshes.Add(robot.ExternalAxes[i].BaseMesh);
                    _meshes.Add(robot.ExternalAxes[i].LinkMesh);
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The External Axis is not Valid");
                }
            }

            // Output
            DA.SetData(0, robot.Name);
            DA.SetDataList(1, meshes);
            DA.SetDataList(2, robot.InternalAxisPlanes);
            DA.SetDataList(3, robot.InternalAxisLimits);
            DA.SetData(4, robot.BasePlane);
            DA.SetData(5, robot.MountingFrame);
            DA.SetData(6, robot.ToolPlane);
            DA.SetData(7, robot.Tool);
            DA.SetDataList(8, robot.ExternalAxes);
        }

        #region menu item
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Documentation", MenuItemClickComponentDoc, Properties.Resources.WikiPage_MenuItem_Icon);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Documentation" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickComponentDoc(object sender, EventArgs e)
        {
            string url = Documentation.ComponentWeblinks[this.GetType()];
            Documentation.OpenBrowser(url);
        }
        #endregion

        /// <summary>
        /// This method displays the meshes
        /// </summary>
        /// <param name="args"> Preview display arguments for IGH_PreviewObjects. </param>
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            // Initiate material
            Rhino.Display.DisplayMaterial material;

            // Selected document objects
            List<IGH_DocumentObject> selectedObjects = _doc.SelectedObjects();

            // Check if component is selected
            if (selectedObjects.Contains(this))
            {
                material = args.ShadeMaterial_Selected;
            }
            else
            {
                material = args.ShadeMaterial;
            }

            // Display the meshes
            for (int i = 0; i != _meshes.Count; i++)
            {
                args.Display.DrawMeshShaded(_meshes[i], material);
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructRobotInfoComponent_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8452629c-5da8-4e64-82f2-23f00c49ae4b"); }
        }
    }
}