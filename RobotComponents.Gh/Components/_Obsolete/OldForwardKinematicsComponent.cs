// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Drawing;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Kinematics;
using RobotComponents.Gh.Goos.Definitions;
using RobotComponents.Gh.Parameters.Definitions;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.05.000 (January 2020)
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Forward Kinematics component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldForwardKinematicsComponent : GH_Component 
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldForwardKinematicsComponent()
          : base("Forward Kinematics", "FK",
              "OBSOLETE: Computes the position of the end-effector of a defined ABB robot based on a set of given axis values."
                + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Simulation")
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
            pManager.AddNumberParameter("Internal Axis Values", "IAV", "Internal Axis Values as List", GH_ParamAccess.list, new List<double> { 0, 0, 0, 0, 0, 0 } );
            pManager.AddNumberParameter("External Axis Values", "EAV", "External Axis Values as List", GH_ParamAccess.list, new List<double> { 0, 0, 0, 0, 0, 0 });
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_MeshParam("Posed Meshes", "PM", "Posed Robot and External Axis meshes");  //Todo: beef this up to be more informative.
            pManager.Register_PlaneParam("End Plane", "EP", "Robot EndEffector Plane placed on Target");
            pManager.Register_PlaneParam("External Axis Planes", "EAP", "Exernal Axis Planes as list of Planes");
        }

        // Fields
        ForwardKinematics _fk = new ForwardKinematics();

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed " +
                "in the future. Remove this component from your canvas and replace it by picking the new component " +
                "from the ribbon.");

            // Input variables
            GH_Robot robotGoo = null;
            List<double> internalAxisValues = new List<double>();
            List<double> externalAxisValues = new List<double>();

            // Catch input data
            if (!DA.GetData(0, ref robotGoo)) { return; }
            if (!DA.GetDataList(1, internalAxisValues)) { return; }
            if (!DA.GetDataList(2, externalAxisValues)) { return; }

            // Add up missing internal axisValues
            for (int i = internalAxisValues.Count; i <6; i++)
            {
                internalAxisValues.Add(0);
            }

            // Add up missing external axisValues
            for (int i = externalAxisValues.Count; i < 6; i++)
            {
                externalAxisValues.Add(0);
            }

            // Calcuate the robot pose
            ForwardKinematics forwardKinematics = new ForwardKinematics(robotGoo.Value, new RobotJointPosition(internalAxisValues), new ExternalJointPosition(externalAxisValues));
            forwardKinematics.Calculate();

            // Check the values
            for (int i = 0; i < forwardKinematics.ErrorText.Count; i++)
            {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, forwardKinematics.ErrorText[i]);
            }

            // Create data tree for output of all posed meshes
            GH_Structure<GH_Mesh> meshes = GetPosedMeshesDataTree(forwardKinematics);

            // Output
            _fk = forwardKinematics;
            DA.SetDataTree(0, meshes);
            DA.SetData(1, forwardKinematics.TCPPlane); // Outputs the TCP as a plane
            DA.SetDataList(2, forwardKinematics.PosedExternalAxisPlanes); // Outputs the External Axis Planes
        }

        /// <summary>
        /// Transform the posed meshes rom the forward kinematics to a datatree
        /// </summary>
        /// <param name="forwardKinematics"> The forward kinematics the posed meshes will be extracted from. </param>
        /// <returns> The data tree structure with all the posed meshes. </returns>
        public GH_Structure<GH_Mesh> GetPosedMeshesDataTree(ForwardKinematics forwardKinematics)
        {
            // Create data tree for output of alle posed meshes
            GH_Structure<GH_Mesh> meshes = new GH_Structure<GH_Mesh>();

            {
                // Robot pose meshes
                List<Mesh> posedInternalAxisMeshes = forwardKinematics.PosedInternalAxisMeshes;

                // Data tree path
                GH_Path path = new GH_Path(0);

                // Save the posed meshes
                for (int i = 0; i < posedInternalAxisMeshes.Count; i++)
                {
                    meshes.Append(new GH_Mesh(posedInternalAxisMeshes[i]), path);
                }

                // Extenal axis meshes
                List<List<Mesh>> posedExternalAxisMeshes = forwardKinematics.PosedExternalAxisMeshes;

                // Loop over all the external axes
                for (int i = 0; i < posedExternalAxisMeshes.Count; i++)
                {
                    // Data tree path
                    path = new GH_Path(i + 1);

                    // Save the posed meshes
                    for (int j = 0; j < posedExternalAxisMeshes[i].Count; j++)
                    {
                        meshes.Append(new GH_Mesh(posedExternalAxisMeshes[i][j]), path);
                    }
                }
            }

            // Return the data tree stucture
            return meshes;
        }

        /// <summary>
        /// This method displays the robot pose for the given axis values. 
        /// </summary>
        /// <param name="args"> Preview display arguments for IGH_PreviewObjects. </param>
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            // Check if there is a mesh available to display
            if (_fk.PosedInternalAxisMeshes != null)
            {
                // Initiate the display color and transparancy of the robot mesh
                Color color;
                double trans;

                // Set the display color and transparancy of the robot mesh
                if (_fk.InLimits == true)
                {
                    color = Color.FromArgb(225, 225, 225);
                    trans = 0.0;
                }
                else
                {
                    color = Color.FromArgb(150, 0, 0);
                    trans = 0.5;
                }

                // Display the internal axes of the robot
                for (int i = 0; i != _fk.PosedInternalAxisMeshes.Count; i++)
                {
                    args.Display.DrawMeshShaded(_fk.PosedInternalAxisMeshes[i], new Rhino.Display.DisplayMaterial(color, trans));
                }

                // Display the external axes
                for (int i = 0; i != _fk.PosedExternalAxisMeshes.Count; i++)
                {
                    for (int j = 0; j != _fk.PosedExternalAxisMeshes[i].Count; j++)
                    {
                        args.Display.DrawMeshShaded(_fk.PosedExternalAxisMeshes[i][j], new Rhino.Display.DisplayMaterial(color, trans));
                    }
                }
            }
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
            get { return new Guid("C1B950EA-B10E-4AD8-A676-8320DB465F14"); }
        }

    }
}
