// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Kinematics;
using RobotComponents.Definitions;
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Gh.Parameters.Actions;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.08.000
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Simulation
{
    /// <summary>
    /// RobotComponents Path Generator component. An inherent from the GH_Component Class.
    /// </summary>
    public class OldPathGeneratorComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldPathGeneratorComponent()
          : base("Path Generator", "PG",
              "EXPERIMENTAL: Generates and displays an approximation of the movement path for a defined ABB robot based on a list of Actions."
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
            pManager.AddParameter(new ActionParameter(), "Actions", "A", "Actions as a list with Actions", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Interpolations", "I", "Interpolations as Int", GH_ParamAccess.item, 5);
            pManager.AddNumberParameter("Animation Slider", "AS", "Animation Slider as number (0.0 - 1.0)", GH_ParamAccess.item, 0.0);
            pManager.AddBooleanParameter("Display Path", "DP", "Display Path Path if set to true.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Update", "U", "If set to true, path will be constantly recalculated.", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_PlaneParam("Plane", "P", "Current Target Plane");  //Todo: beef this up to be more informative.
            pManager.Register_DoubleParam("Internal Axis Values", "IAV", "Contains Internal Axis Values");  //Todo: beef this up to be more informative.
            pManager.Register_DoubleParam("External Axis Values", "EAV", "Contains External Axis Values");  //Todo: beef this up to be more informative.
            pManager.Register_CurveParam("Movement Paths", "P", "Movement Paths as Curves");
        }

        // Fields
        private PathGenerator _pathGenerator = new PathGenerator();
        private List<Plane> _planes = new List<Plane>();
        private List<Curve> _paths = new List<Curve>();
        private List<RobotJointPosition> _robotJointPositions = new List<RobotJointPosition>();
        private List<ExternalJointPosition> _externalJointPositions = new List<ExternalJointPosition>();
        private int _lastInterpolations = 0;

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
            Robot robot = new Robot();
            List<RobotComponents.Actions.Action> actions = new List<RobotComponents.Actions.Action>();
            int interpolations = 0;
            double interpolationSlider = 0;
            bool displayPath = false;
            bool update = false;

            // Catch the input data
            if (!DA.GetData(0, ref robot)) { return; }
            if (!DA.GetDataList(1, actions)) { return; }
            if (!DA.GetData(2, ref interpolations)) { return; }
            if (!DA.GetData(3, ref interpolationSlider)) { return; }
            if (!DA.GetData(4, ref displayPath)) { return; }
            if (!DA.GetData(5, ref update)) { return; }


            // Update the path
            if (update == true || _lastInterpolations != interpolations)
            {
                // Create the path generator
                _pathGenerator = new PathGenerator(robot);

                // Re-calculate the path
                _pathGenerator.Calculate(actions, interpolations);

                // Get all the targets
                _planes.Clear();
                _planes = _pathGenerator.Planes;

                // Get the new path curve
                _paths.Clear();
                _paths = _pathGenerator.Paths;

                // Clear the lists with the internal and external axis values
                _robotJointPositions.Clear();
                _externalJointPositions.Clear();
                _robotJointPositions = _pathGenerator.RobotJointPositions;
                _externalJointPositions = _pathGenerator.ExternalJointPositions;

                // Store the number of interpolations that are used, to check if this value is changed. 
                _lastInterpolations = interpolations;
            }

            // Get the index number of the current target
            int index = (int)(((_planes.Count - 1) * interpolationSlider));

            List<double> externalAxisValues = _externalJointPositions[index].ToList();
            externalAxisValues.RemoveAll(val => val == 9e9);

            // Output
            DA.SetData(0, _planes[index]);
            DA.SetDataList(1, _robotJointPositions[index].ToList());
            DA.SetDataList(2, externalAxisValues);

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
