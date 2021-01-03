// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Gh.Goos.Actions;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.08.000

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Target visualization component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class TargetVisualizerComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public TargetVisualizerComponent()
          : base("Target Visualizer", "TV",
              "Displays Target Information."
                + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Utility")
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
            pManager.AddGenericParameter("Actions", "A", "Input Actions here, all Targets will be extracted automatically.", GH_ParamAccess.tree);
            pManager.AddBooleanParameter("Display Names", "DN", "Display Target Names if set to true", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Display Points", "DP", "Display Target Origins if set to true.", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Display Direction", "DD", "Displays Target Plane Direction if set to true.", GH_ParamAccess.item, true);
            pManager.AddColourParameter("Color", "C", "Display Color", GH_ParamAccess.item, System.Drawing.Color.Black);
            pManager.AddIntegerParameter("Text Size", "TS", "Text size as int", GH_ParamAccess.item, 8);
            pManager.AddIntegerParameter("Point Size", "PS", "Point size as int", GH_ParamAccess.item, 3);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // This component has no output parameters.
        }

        // Fields
        private GH_Structure<GH_Target> _targetGoos = new GH_Structure<GH_Target>();
        private System.Drawing.Color _color = new System.Drawing.Color();
        private bool _displayNames = true;
        private bool _displayPoints = true;
        private bool _displayDirections = false;
        private int _textSize = 7;
        private int _pointSize = 2;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed in the future.");

            // Variable for catchint the datatree 
            GH_Structure<IGH_Goo> actions;

            // Clear the list with targets before catching new input data
            _targetGoos.Clear();

            // Catch the input data
            if (!DA.GetDataTree(0, out actions)) { return; }
            if (!DA.GetData(1, ref _displayNames)) { return; }
            if (!DA.GetData(2, ref _displayPoints)) { return; }
            if (!DA.GetData(3, ref _displayDirections)) { return; }
            if (!DA.GetData(4, ref _color)) { return; }
            if (!DA.GetData(5, ref _textSize)) { return; }
            if (!DA.GetData(6, ref _pointSize)) { return; }

            // Get the paths of the datatree with actions
            var paths = actions.Paths;

            // Check and concert the input data to the right datatype (target)
            for (int i = 0; i < actions.Branches.Count; i++)
            {
                var branches = actions.Branches[i];
                GH_Path iPath = paths[i];

                for (int j = 0; j < branches.Count; j++)
                {
                    // Get the target from the movement instance if the input data is a movement
                    if (actions.Branches[i][j] is GH_Movement)
                    {
                        GH_Movement movementGoo = actions.Branches[i][j] as GH_Movement;
                        GH_Target targetGoo = new GH_Target(movementGoo.Value.Target);
                        _targetGoos.Append(targetGoo, iPath);
                    }
                    // Get the target data directly if the input data is a target
                    else if (actions.Branches[i][j] is GH_Target)
                    {
                        GH_Target targetGoo = actions.Branches[i][j] as GH_Target;
                        _targetGoos.Append(targetGoo, iPath);
                    }
                    // Make a target from the input plane if the input data is a plane
                    else if (actions.Branches[i][j] is GH_Plane)
                    {
                        string targetName;
                        if (actions.Branches.Count == 1)
                        {
                            targetName = "plane" + "_" + j;
                        }
                        else
                        {
                            targetName = "plane" + "_" + i + "_" + j;
                        }

                        GH_Plane planeGoo = actions.Branches[i][j] as GH_Plane;
                        RobotTarget target = new RobotTarget(targetName, planeGoo.Value);
                        GH_Target targetGoo = new GH_Target(target);
                        _targetGoos.Append(targetGoo, iPath);
                    }
                    // Let all other data pass (raise no warning or error)
                    else
                    {
                        // empty
                    }
                }
            }
        }

        /// <summary>
        /// This method displays the data associated to the targets. 
        /// </summary>
        /// <param name="args"> Preview display arguments for IGH_PreviewObjects.</param>
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            // Loop over all the branches in the datatree structure with targets
            for (int i = 0; i < _targetGoos.Branches.Count; i++)
            {
                // Get the indvidual branch
                var branches = _targetGoos.Branches[i];

                // Loop over all the items in the individual branch
                for (int j = 0; j < branches.Count; j++)
                {
                    // Get the target
                    RobotTarget target = branches[j].Value as RobotTarget;

                    // Display the name of the target
                    if (_displayNames == true)
                    {
                        double pixelsPerUnit;
                        args.Viewport.GetWorldToScreenScale(target.Plane.Origin, out pixelsPerUnit);

                        Plane plane;
                        args.Viewport.GetCameraFrame(out plane);
                        plane.Origin = target.Plane.Origin + target.Plane.ZAxis * 2;

                        args.Display.Draw3dText(target.Name, _color, plane, _textSize / pixelsPerUnit, "Lucida Console");
                    }

                    // Display the origin of the target plane
                    if (_displayPoints == true)
                    {
                        args.Display.DrawPoint(target.Plane.Origin, Rhino.Display.PointStyle.Simple, _pointSize, _color);
                    }

                    // Display the direction / orientation of the target plane
                    if (_displayDirections == true)
                    {
                        Plane planeVisual = target.Plane;
                        args.Display.DrawDirectionArrow(planeVisual.Origin, planeVisual.ZAxis, System.Drawing.Color.Blue);
                        args.Display.DrawDirectionArrow(planeVisual.Origin, planeVisual.XAxis, System.Drawing.Color.Red);
                        args.Display.DrawDirectionArrow(planeVisual.Origin, planeVisual.YAxis, System.Drawing.Color.Green);
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
            get { return Properties.Resources.Target_Visualizer_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("EDFDCE2D-65BC-4B99-8BCA-C171D42CB89B"); }
        }
    }
}
