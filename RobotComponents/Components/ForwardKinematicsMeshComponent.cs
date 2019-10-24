using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace RobotComponents
{

    public class ForwardKinematicsMeshComponent : GH_Component 
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ForwardKinematicsMeshComponent()
          : base("Forward Kinematics with Mesh", "FK",
              "Calculates Robot and End Plane Position based on given Axis Values.",
              "RobotComponents", "Simulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("RobotInfo", "RI", "Contains Robot Data", GH_ParamAccess.item);
            pManager.AddNumberParameter("AxisValues", "AV", "Axis Values as Double List", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //pManager.Register_MeshParam("RobotMesh", "RM", "Posed Robot Mesh");  //Todo: beef this up to be more informative.
            pManager.Register_PlaneParam("EndPlane", "EP", "Robot EndEffector Plane placed on Target");
            pManager.Register_MeshParam("RobotMesh", "RM", "Robot Mesh in Final Kinematic");
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
            List<double> axisValues = new List<double>();
            List<Mesh> robotMeshes = new List<Mesh>();

            //inputs
            if (!DA.GetData(0, ref robotInfo)) { return; }
            if (!DA.GetDataList(1, axisValues)) { return; }

            //calculations
            ForwardKinematics forwardKinematics = new ForwardKinematics(robotInfo.Value, axisValues);
            forwardKinematics.Calculate();

            //valuesCheck
            for (int i = 0; i < forwardKinematics.ErrorText.Count; i++)
            {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, forwardKinematics.ErrorText[i]);
            }

            //output
            _fk = forwardKinematics;
            DA.SetData(0, forwardKinematics.TCPPlane);
            DA.SetDataList(1, forwardKinematics.PosedMeshes);
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
            get { return new Guid("83958ECE-B1A8-424A-8BFE-E5A2976772E1"); }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            if (_fk.PosedMeshes != null)
            {
                bool AxisAreValid = true;

                for (int i = 0; i < _fk.AxisInLimit.Count; i++)
                {
                    if (_fk.AxisInLimit[i] == false)
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
