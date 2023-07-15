// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.Deconstruct.Definitions
{
    /// <summary>
    /// RobotComponents Deconstruct Robot Info component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructRobotComponent : GH_Component
    {
        #region fields
        private readonly List<Mesh> _meshes = new List<Mesh>() { };
        private GH_Document _doc;
        #endregion

        /// <summary>
        /// Initializes a new instance of the DeconstructrobotComponent class.
        /// </summary>
        public DeconstructRobotComponent()
          : base("Deconstruct Robot", "DeRob", 
              "Deconstructs a Robot component into its parameters"
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Deconstruct")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Robot(), "Robot", "R", "Robot as Robot", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Robot Name as String", GH_ParamAccess.item);
            pManager.AddMeshParameter("Meshes", "M", "Robot Meshes as Mesh List", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Axis Planes", "AP", "Axis Planes as Plane List", GH_ParamAccess.list);
            pManager.AddIntervalParameter("Axis Limits", "AL", "Axis Limits as Interval List", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Position Plane", "PP", "Position Plane of the Robot as Plane", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Mounting Frame", "MF", "Mounting Frame as Frame", GH_ParamAccess.item);
            pManager.RegisterParam(new Param_RobotTool(), "Robot Tool", "RT", "Robot Tool as Robot Tool", GH_ParamAccess.item);
            pManager.RegisterParam(new Param_ExternalAxis(), "External Axes", "EA", "External Axes as External Axis Parameter", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Get the Grasshopper document
            _doc = this.OnPingDocument();

            // Input variables
            Robot robot = null;

            // Catch the input data
            if (!DA.GetData(0, ref robot)) { return; }

            // Clear list with meshes on first iteration
            if (DA.Iteration == 0)
            {
                _meshes.Clear();
            }

            if (robot != null)
            {
                // Check if the input is valid
                if (!robot.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Robot is not valid");
                }

                // Output meshes (only link meshes, no robot tool)
                List<Mesh> meshes = new List<Mesh>();

                // Add display meshes
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
                DA.SetData(6, robot.Tool);
                DA.SetDataList(7, robot.ExternalAxes);
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
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
            get { return new Guid("E651BA0F-7CE3-40BC-A04F-C76EA0665D1A"); }
        }
        #endregion

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

        #region custom preview method
        /// <summary>
        /// Gets the clipping box for all preview geometry drawn by this component and all associated parameters.
        /// </summary>
        public override BoundingBox ClippingBox
        {
            get { return GetBoundingBox(); }
        }

        /// <summary>
        /// Returns the bounding box for all preview geometry drawn by this component.
        /// </summary>
        /// <returns></returns>
        private BoundingBox GetBoundingBox()
        {
            BoundingBox result = new BoundingBox();

            // Get bouding box of all the output parameters
            for (int i = 0; i < Params.Output.Count; i++)
            {
                if (Params.Output[i] is IGH_PreviewObject previewObject)
                {
                    result.Union(previewObject.ClippingBox);
                }
            }

            for (int i = 0; i < _meshes.Count; i++)
            {
                if (_meshes[i].IsValid)
                {
                    result.Union(_meshes[i].GetBoundingBox(false));
                }
            }

            return result;
        }

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
                if (_meshes[i].IsValid)
                {
                    args.Display.DrawMeshShaded(_meshes[i], material);
                }
            }
        }
        #endregion
    }
}