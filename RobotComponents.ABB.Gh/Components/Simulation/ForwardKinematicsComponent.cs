// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GH_IO.Serialization;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Kinematics;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Actions;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.Simulation
{
    /// <summary>
    /// RobotComponents Forward Kinematics component. An inherent from the GH_Component Class.
    /// </summary>
    public class ForwardKinematicsComponent : GH_Component 
    {
        #region fields
        private readonly List<ForwardKinematics> _forwardKinematics = new List<ForwardKinematics>();
        private bool _hideMesh = false;
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public ForwardKinematicsComponent()
          : base("Forward Kinematics", "FK",
              "Computes the position of the end-effector of a defined ABB robot based on a set of given axis values."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Simulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Robot(), "Robot", "R", "Robot as Robot", GH_ParamAccess.item);
            pManager.AddParameter(new Param_RobotJointPosition(), "Robot Joint Position", "RJ", "The positions of the Robot Axes as Robot Joint Position", GH_ParamAccess.item);
            pManager.AddParameter(new Param_ExternalJointPosition(), "External Joint Position", "EJ", "The positions of the external logical axes as External Joint Position", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_MeshParam("Posed Meshes", "PM", "Posed Robot and External Axis meshes");
            pManager.Register_PlaneParam("End Plane", "EP", "Robot TCP plane placed on Target");
            pManager.Register_PlaneParam("External Axis Planes", "EAP", "Exernal Axis Planes as list of Planes");
        }

        /// <summary>
        /// Override this method if you want to be called before the first call to SolveInstance.
        /// </summary>
        protected override void BeforeSolveInstance()
        {
            base.BeforeSolveInstance();

            _forwardKinematics.Clear();
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            Robot robot = null;
            RobotJointPosition robotJointPosition = new RobotJointPosition();
            ExternalJointPosition externalJointPosition = new ExternalJointPosition();

            // Catch input data
            if (!DA.GetData(0, ref robot)) { return; }
            if (!DA.GetData(1, ref robotJointPosition)) { robotJointPosition = new RobotJointPosition(); }
            if (!DA.GetData(2, ref externalJointPosition)) { externalJointPosition = new ExternalJointPosition(); }

            // Calcuate the robot pose
            ForwardKinematics forwardKinematics = new ForwardKinematics(robot, robotJointPosition, externalJointPosition, _hideMesh);
            forwardKinematics.Calculate();

            // Check the values
            for (int i = 0; i < forwardKinematics.ErrorText.Count; i++)
            {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, forwardKinematics.ErrorText[i]);
            }

            // Add to list with FK
            _forwardKinematics.Add(forwardKinematics);

            // Output variable
            GH_Structure<GH_Mesh> meshes = new GH_Structure<GH_Mesh>();

            // Fill data tree with meshes
            if (_hideMesh == false)
            {
                meshes = (this.GetPosedMeshesDataTree(DA.Iteration));
            }

            // Output
            DA.SetDataTree(0, meshes);
            DA.SetData(1, forwardKinematics.TCPPlane);
            DA.SetDataList(2, forwardKinematics.PosedExternalAxisPlanes);
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
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.ForwardKinematics_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("438679CC-2135-48B3-B818-F7E3A65503C2"); }
        }
        #endregion

        #region menu items
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Preview Robot Mesh", MenuItemClickHideMesh, true, !_hideMesh);
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

        /// <summary>
        /// Handles the event when the custom menu item "Hide Mesh" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickHideMesh(object sender, EventArgs e)
        {
            RecordUndoEvent("Preview Robot Mesh");
            _hideMesh = !_hideMesh;
            ExpireSolution(true);
        }

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Set Hide Mesh", _hideMesh);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            _hideMesh = reader.GetBoolean("Set Hide Mesh");
            return base.Read(reader);
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

            // Add bounding box of custom preview
            if (!_hideMesh)
            {
                for (int i = 0; i < _forwardKinematics.Count; i++)
                {
                    result.Union(_forwardKinematics[i].GetBoundingBox(false));
                }
            }

            return result;
        }

        /// <summary>
        /// This method displays the robot pose for the given axis values. 
        /// </summary>
        /// <param name="args"> Preview display arguments for IGH_PreviewObjects. </param>
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            // Default implementation (disabled)
            // base.DrawViewportMeshes(args);

            if (!_hideMesh)
            {
                for (int i = 0; i < _forwardKinematics.Count; i++)
                {
                    // Initiate the display color and transparancy of the robot mesh
                    Color color;
                    double trans;

                    // Set the display color and transparancy of the robot mesh
                    if (_forwardKinematics[i].InLimits == true)
                    {
                        color = Color.FromArgb(225, 225, 225);
                        trans = 0.0;
                    }
                    else
                    {
                        color = Color.FromArgb(150, 0, 0);
                        trans = 0.5;
                    }

                    // Display internal axis meshes
                    for (int j = 0; j < _forwardKinematics[i].PosedInternalAxisMeshes.Count; j++)
                    {
                        if (_forwardKinematics[i].PosedInternalAxisMeshes[j].IsValid)
                        {
                            args.Display.DrawMeshShaded(_forwardKinematics[i].PosedInternalAxisMeshes[j], new Rhino.Display.DisplayMaterial(color, trans));
                        }
                    }

                    // Display external axis meshes
                    for (int j = 0; j < _forwardKinematics[i].PosedExternalAxisMeshes.Count; j++)
                    {
                        for (int k = 0; k < _forwardKinematics[i].PosedExternalAxisMeshes[j].Count; k++)
                        {
                            if (_forwardKinematics[i].PosedExternalAxisMeshes[j][k].IsValid)
                            {
                                args.Display.DrawMeshShaded(_forwardKinematics[i].PosedExternalAxisMeshes[j][k], new Rhino.Display.DisplayMaterial(color, trans));
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region additional methods
        /// <summary>
        /// Transform the posed meshes rom the forward kinematics to a datatree
        /// </summary>
        /// <param name="iteration"> Iteration number of SolveInstance method. </param>
        /// <returns> The data tree structure with all the posed meshes. </returns>
        private GH_Structure<GH_Mesh> GetPosedMeshesDataTree(int iteration)
        {
            // Create data tree for output of alle posed meshes
            GH_Structure<GH_Mesh> meshes = new GH_Structure<GH_Mesh>();

            // Robot pose meshes
            List<Mesh> posedInternalAxisMeshes = _forwardKinematics[iteration].PosedInternalAxisMeshes;

            // Data tree path
            GH_Path path = new GH_Path(new int[2] { iteration, 0 });
            
            // Save the posed meshes
            for (int i = 0; i < posedInternalAxisMeshes.Count; i++)
            {
                meshes.Append(new GH_Mesh(posedInternalAxisMeshes[i]), path);
            }

            // Extenal axis meshes
            List<List<Mesh>> posedExternalAxisMeshes = _forwardKinematics[iteration].PosedExternalAxisMeshes;

            // Loop over all the external axes
            for (int i = 0; i < posedExternalAxisMeshes.Count; i++)
            {
                // Data tree path
                path = new GH_Path(new int[2] { iteration, i + 1 });

                // Save the posed meshes
                for (int j = 0; j < posedExternalAxisMeshes[i].Count; j++)
                {
                    meshes.Append(new GH_Mesh(posedExternalAxisMeshes[i][j]), path);
                }
            }

            // Return the data tree stucture
            return meshes;
        }
        #endregion
    }
}
