using System;
using System.Collections.Generic;

using Rhino.Geometry;
using Grasshopper.Kernel;

using RobotComponents.BaseClasses.Actions;
using RobotComponents.BaseClasses.Kinematics;
using RobotComponentsABB.Goos;
using RobotComponentsABB.Parameters;

namespace RobotComponentsABB.Components.Simulation
{
    /// <summary>
    /// RobotComponents Path Generator component. An inherent from the GH_Component Class.
    /// </summary>
    public class PathGeneratorComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public PathGeneratorComponent()
          : base("Path Generator", "PG",
              "EXPERIMENTAL: Generates and displays an approximation of the movement path for a defined ABB robot based on a list of Actions."
                + System.Environment.NewLine +
                "RobotComponents : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Simulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new RobotInfoParameter(), "Robot Info", "RI", "Robot Info as Robot Info", GH_ParamAccess.item);
            pManager.AddGenericParameter("Actions", "A", "Actions as Actions", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Interpolations", "I", "Interpolations as Int", GH_ParamAccess.item, 5);
            pManager.AddNumberParameter("Animation Slider", "IS", "Animation Slider as double (0.0 - 1.0)", GH_ParamAccess.item, 0.0);
            pManager.AddBooleanParameter("Display Path", "CP", "Display Path Path if set to true.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Update", "U", "If set to true, path will be constantly recalculated.", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new TargetParameter(), "Target", "T", "Current Target Data");  //Todo: beef this up to be more informative.
            pManager.Register_DoubleParam("Internal Axis Values", "IAV", "Contains Internal Axis Values");  //Todo: beef this up to be more informative.
            pManager.Register_DoubleParam("External Axis Values", "EAV", "Contains External Axis Values");  //Todo: beef this up to be more informative.
            pManager.Register_CurveParam("Movement Paths", "P", "Movement Paths as Curves");
        }

        // Fields
        private PathGenerator _pathGenerator = new PathGenerator();
        private List<Target> _targets = new List<Target>();
        private List<Curve> _paths = new List<Curve>();
        private List<List<double>> _internalAxisValues = new List<List<double>>();
        private List<List<double>> _externalAxisValues = new List<List<double>>();
        private int _lastInterpolations = 0;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            RobotInfoGoo robotInfoGoo = new RobotInfoGoo();
            List<RobotComponents.BaseClasses.Actions.Action> actions = new List<RobotComponents.BaseClasses.Actions.Action>();
            int interpolations = 0;
            double interpolationSlider = 0;
            bool displayPath = false;
            bool update = false;

            // Catch the input data
            if (!DA.GetData(0, ref robotInfoGoo)) { return; }
            if (!DA.GetDataList(1, actions)) { return; }
            if (!DA.GetData(2, ref interpolations)) { return; }
            if (!DA.GetData(3, ref interpolationSlider)) { return; }
            if (!DA.GetData(4, ref displayPath)) { return; }
            if (!DA.GetData(5, ref update)) { return; }

            // Create the path generator
            _pathGenerator = new PathGenerator(robotInfoGoo.Value);

            // Update the path
            if (update == true || _lastInterpolations != interpolations)
            {
                // Re-calculate the path
                _pathGenerator.Calculate(actions, interpolations);

                // Get all the targets
                _targets.Clear();
                _targets = _pathGenerator.Targets;

                // Get the new path curve
                _paths.Clear();
                _paths = _pathGenerator.GeneratePathCurves();

                // Clear the lists with the internal and external axis values
                ClearAxisValuesLists();

                // Re-calculate all the internal and external axis values
                for (int i = 0; i < _targets.Count; i++)
                {
                    InverseKinematics IK = new InverseKinematics(_targets[i], robotInfoGoo.Value);
                    IK.Calculate();

                    _internalAxisValues.Add(IK.InternalAxisValues);
                    _externalAxisValues.Add(IK.ExternalAxisValues);
                }

                // Store the number of interpolations that are used, to check if this value is changed. 
                _lastInterpolations = interpolations;
            }

            // Get the index number of the current target
            int targetIndex = (int)(((_targets.Count - 1) * interpolationSlider));
          
            // Output
            DA.SetData(0, _targets[targetIndex]);
            DA.SetDataList(1, _internalAxisValues[targetIndex]);
            DA.SetDataList(2, _externalAxisValues[targetIndex]);

            if (displayPath == true)
            {
                DA.SetDataList(3, _paths);
            }
            else
            {
                DA.SetDataList(3, null);
            }
        }

        /// <summary>
        /// This method clears the two lists with internal and external values
        /// </summary>
        private void ClearAxisValuesLists()
        {
            // Clear the all the individual lists with internal axis values
            for (int i = 0; i < _internalAxisValues.Count; i++)
            {
                _internalAxisValues[i].Clear();
            }

            // Clear the all the individual lists with external axis values
            for (int i = 0; i < _externalAxisValues.Count; i++)
            {
                _externalAxisValues.Clear();
            }

            // Clear both primary lists
            _internalAxisValues.Clear();
            _externalAxisValues.Clear();
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.PathGen_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("C55D9FE7-7059-42E7-9383-2FE031BF788C"); }
        }
    }
}
