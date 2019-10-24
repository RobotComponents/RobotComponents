using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using RobotComponents.BaseClasses;
using RobotComponents.Goos;

namespace RobotComponents.Components
{

    public class ForwardKinematicsComponent : GH_Component 
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ForwardKinematicsComponent()
          : base("Forward Kinematics", "FK",
              "Computes the position of the end-effector of a defined ABB robot based on a set of given axis values."
                + System.Environment.NewLine +
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Simulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot Info", "RI", "Contains Robot Data", GH_ParamAccess.item);
            pManager.AddNumberParameter("Internal Axis Values", "IAV", "Internal Axis Values as List", GH_ParamAccess.list, new List<double> { 0, 0, 0, 0, 0, 0 } );
            pManager.AddNumberParameter("External Axis Values", "EAV", "External Axis Values as List", GH_ParamAccess.list, new List<double> { 0, 0, 0, 0, 0, 0 });
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_MeshParam("Robot Mesh", "RM", "Posed Robot Mesh");  //Todo: beef this up to be more informative.
            pManager.Register_PlaneParam("End Plane", "EP", "Robot EndEffector Plane placed on Target");
        }

        ForwardKinematics _fk = new ForwardKinematics();
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //variables
            RobotInfoGoo robotInfo = null;
            List<double> internalAxisValues = new List<double>();
            List<double> externalAxisValues = new List<double>();
            List<Mesh> robotMeshes = new List<Mesh>();

            //inputs
            if (!DA.GetData(0, ref robotInfo)) { return; }
            if (!DA.GetDataList(1, internalAxisValues)) { return; }
            if (!DA.GetDataList(2, externalAxisValues)) { return; }

            //add up missing internal axisValues
            for (int i = internalAxisValues.Count; i <6; i++)
            {
                internalAxisValues.Add(0);
            }

            //add up missing external axisValues
            for (int i = externalAxisValues.Count; i < 6; i++)
            {
                externalAxisValues.Add(0);
            }

            //calculations
            ForwardKinematics forwardKinematics = new ForwardKinematics(robotInfo.Value, internalAxisValues, externalAxisValues);
            forwardKinematics.Calculate();

            //valuesCheck
            for (int i = 0; i < forwardKinematics.ErrorText.Count; i++)
            {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, forwardKinematics.ErrorText[i]);
            }

            //output
            _fk = forwardKinematics;
            DA.SetDataList(0, forwardKinematics.PosedMeshes); // Output the Meshe of the Robot in Pose ( toggle this ? )
            DA.SetData(1, forwardKinematics.TCPPlane); // Outputs the TCP as a plane
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ForwardKinematics_Icon;
            }
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

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            if (_fk.PosedMeshes != null)
            {
                bool AxisAreValid = true;

                for (int i = 0; i < _fk.InternalAxisInLimit.Count; i++)
                {
                    if (_fk.InternalAxisInLimit[i] == false)
                    {
                        AxisAreValid = false;
                    }
                }

                for (int i = 0; i < _fk.ExternalAxisInLimit.Count; i++)
                {
                    if (_fk.ExternalAxisInLimit[i] == false)
                    {
                        AxisAreValid = false;
                    }
                }

                if (AxisAreValid == true)
                {
                    for (int i = 0; i != _fk.PosedMeshes.Count; i++)
                    {
                        args.Display.DrawMeshShaded(_fk.PosedMeshes[i], new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
                    }
                }
                else if (AxisAreValid == false)
                {
                    for (int i = 0; i != _fk.PosedMeshes.Count; i++)
                    {
                        args.Display.DrawMeshShaded(_fk.PosedMeshes[i], new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(150, 0, 0), 0.5));
                    }
                }
            }
        }
    }


}
