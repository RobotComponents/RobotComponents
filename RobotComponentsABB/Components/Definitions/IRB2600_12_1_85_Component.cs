// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.BaseClasses.Definitions;
using RobotComponentsGoos.Definitions;
using RobotComponentsABB.Parameters.Definitions;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components.Definitions
{
    /// <summary>
    /// RobotComponents IRB2600-12/1.85 preset component. An inherent from the GH_Component Class.
    /// </summary>
    public class IRB2600_12_1_85_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IRB2600_12_1_85_Component class.
        /// </summary>
        public IRB2600_12_1_85_Component()
          : base("ABB IRB2600-12/1.85", "IRB2600",
              "An ABB IRB2600-12/1.85 Robot Info preset component."
                + System.Environment.NewLine +
                "RobotComponents : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Definitions")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Position Plane", "PP", "Position Plane of the Robot as Plane", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddParameter(new RobotToolParameter(), "Robot Tool", "RT", "Robot Tool as Robot Tool Parameter", GH_ParamAccess.item);
            pManager.AddParameter(new ExternalAxisParameter(), "External Axis", "EA", "External Axis as External Axis Parameter", GH_ParamAccess.list);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new RobotInfoParameter(), "Robot Info", "RI", "Resulting Robot Info", GH_ParamAccess.item);  //Todo: beef this up to be more informative.
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            Plane positionPlane = Plane.WorldXY;
            GH_RobotTool toolGoo = null;
            List<ExternalAxis> externalAxis = new List<ExternalAxis>();

            if (!DA.GetData(0, ref positionPlane)) { return; }
            if (!DA.GetData(1, ref toolGoo))
            {
                toolGoo = new GH_RobotTool();
            }
            if (!DA.GetDataList(2, externalAxis))
            {
            }

            // Check the axis input: A maximum of one external linear axis is allow
            double count = 0;
            for (int i = 0; i < externalAxis.Count; i++)
            {
                if (externalAxis[i] is ExternalLinearAxis)
                {
                    count += 1;
                }
            }

            // Raise error if more than one external linear axis is used
            if (count > 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "At the moment RobotComponents supports one external linear axis.");
            }

            // Robot mesh
            List<Mesh> meshes = new List<Mesh>();
            // Base
            string linkString = RobotComponentsABB.Properties.Resources.IRB2600_shared_link_0;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            // Axis 1
            linkString = RobotComponentsABB.Properties.Resources.IRB2600_shared_link_1;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            // Axis 2
            linkString = RobotComponentsABB.Properties.Resources.IRB2600_shared_link_2;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            // Axis 3
            linkString = RobotComponentsABB.Properties.Resources.IRB2600_12_1_85_link_3;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            // Axis 4
            linkString = RobotComponentsABB.Properties.Resources.IRB2600_12_1_85_link_4;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            // Axis 5
            linkString = RobotComponentsABB.Properties.Resources.IRB2600_12_1_85_link_5;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));
            // Axis 6
            linkString = RobotComponentsABB.Properties.Resources.IRB2600_12_1_85_link_6;
            meshes.Add((Mesh)GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(linkString)));

            // Axis planes
            List<Plane> axisPlanes = new List<Plane>() { };
            // Axis 1
            axisPlanes.Add(new Plane(
                new Point3d(0.00, 0.00, 0.00),
                new Vector3d(0.00, 0.00, 1.00)));
            // Axis 2
            axisPlanes.Add(new Plane(
                new Point3d(150.0, 0.00, 445),
                new Vector3d(0.00, 1.00, 0.00)));
            // Axis 3
            axisPlanes.Add(new Plane(
                new Point3d(150.0, 0.00, 445.0 + 900.0),
                new Vector3d(0.00, 1.00, 0.00)));
            // Axis 4
            axisPlanes.Add(new Plane(
                new Point3d(437.5, 0.00, 445.0 + 900.0 + 115.0),
                new Vector3d(1.00, 0.00, 0.00)));
            // Axis 5
            axisPlanes.Add(new Plane(
                new Point3d(150.0 + 795.0, 0.00, 445.0 + 900.0 + 115.0),
                new Vector3d(0.00, 1.00, 0.00)));
            // Axis 6
            axisPlanes.Add(new Plane(
                new Point3d(150.0 + 795.0 + 85.0, 0.00, 445.0 + 900.0 + 115.0),
                new Vector3d(1.00, 0.00, 0.00)));

            // Robot axis limits
            List<Interval> axisLimits = new List<Interval>{
                new Interval(-180, 180),
                new Interval(-95, 155),
                new Interval(-180, 75),
                new Interval(-400, 400),
                new Interval(-120, 120),
                new Interval(-400, 400),
             };

            // External axis limits
            for (int i = 0; i < externalAxis.Count; i++)
            {
                axisLimits.Add(externalAxis[i].AxisLimits);
            }

            // Tool mounting frame
            Plane mountingFrame = new Plane(
                new Point3d(150.0 + 795.0 + 85.0, 0.00, 445.0 + 900.0 + 115.0),
                new Vector3d(1.00, 0.00, 0.00));
            mountingFrame.Rotate(Math.PI * -0.5, mountingFrame.Normal);

            RobotInfo robotInfo;

            // Override position plane when an external axis is coupled
            if (externalAxis.Count != 0)
            {
                for (int i = 0; i < externalAxis.Count; i++)
                {
                    if(externalAxis[i] is ExternalLinearAxis)
                    {
                        positionPlane = (externalAxis[i] as ExternalLinearAxis).AttachmentPlane;
                    }
                }

                robotInfo = new RobotInfo("IRB2600-12/1.85", meshes, axisPlanes, axisLimits, Plane.WorldXY, mountingFrame, toolGoo.Value, externalAxis);
                Transform trans = Transform.PlaneToPlane(Plane.WorldXY, positionPlane);
                robotInfo.Transfom(trans);
            }

            else
            {
                robotInfo = new RobotInfo("IRB2600-12/1.85", meshes, axisPlanes, axisLimits, Plane.WorldXY, mountingFrame, toolGoo.Value);
                Transform trans = Transform.PlaneToPlane(Plane.WorldXY, positionPlane);
                robotInfo.Transfom(trans);
            }

            // Output
            DA.SetData(0, robotInfo);
        }

        #region menu item
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            // Add menu separator
            Menu_AppendSeparator(menu);

            // Add custom menu items
            Menu_AppendItem(menu, "Documentation", MenuItemClickComponentDoc, Properties.Resources.WikiPage_MenuItem_Icon);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Documentation" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        public void MenuItemClickComponentDoc(object sender, EventArgs e)
        {
            string url = Documentation.ComponentWeblinks[this.GetType()];
            System.Diagnostics.Process.Start(url);
        }
        #endregion

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.IRB2600_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B13A1D2B-4C34-487C-83A5-754FDB8F75A1"); }
        }
    }
}