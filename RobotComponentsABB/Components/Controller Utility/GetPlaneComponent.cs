// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponentsABB.Goos;
using RobotComponentsABB.Utils;
// ABB Libs
using ABB.Robotics.Controllers.RapidDomain;
using ABB.Robotics.Controllers.MotionDomain;

namespace RobotComponentsABB.Components.ControllerUtility
{
    /// <summary>
    /// RobotComponents Controller Utility : Get Plane from a defined controller. An inherent from the GH_Component Class.
    /// </summary>
    public class GetPlaneComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetAxisValues class.
        /// </summary>
        public GetPlaneComponent()
          : base("Get Plane", "GP", "Controller Utility" + System.Environment.NewLine + System.Environment.NewLine +
              "Gets the position of a mechanical unit from the defined ABB IRC5 robot controller."
                + System.Environment.NewLine + System.Environment.NewLine +
                "RobotComponents : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Controller Utility")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // To do: replace generic parameter with an RobotComponents Parameter
            pManager.AddGenericParameter("Robot Controller", "RC", "Robot Controller to extract the position from as Robot Controller", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Coordinate System", "CS", "The coordinate system type", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Position Plane of the mechanical unit", GH_ParamAccess.list);
            pManager.AddTextParameter("Name", "N", "Names of the mechanical units", GH_ParamAccess.list);
        }

        // Fields
        private bool _expire = false;
        private Point3d _point = new Point3d(0, 0, 0);
        private Quaternion _quat = new Quaternion();

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Creates the input value list and attachs it to the input parameter
            CreateValueList();

            // Expire solution of this component (called after expire solution of the created value list)
            if (_expire == true)
            {
                _expire = false;
                this.ExpireSolution(true);
            }

            // Input variables
            GH_Controller controllerGoo = null;
            int coordinateSystem = 0;

            // Catch input data
            if (!DA.GetData(0, ref controllerGoo)) { return; }
            if (!DA.GetData(1, ref coordinateSystem)) { return; }

            // Output variables
            List<Plane> planes = new List<Plane>();

            //  Get the mechanical units of the controller
            MechanicalUnitCollection mechanicalUnits = controllerGoo.Value.MotionSystem.MechanicalUnits;
            List<string> mechanicalUnitnames = new List<string>();

            // ABB coordinate system type
            CoordinateSystemType coord = (CoordinateSystemType)coordinateSystem;

            // Get all the planes
            for (int i = 0; i < mechanicalUnits.Count; i++)
            {
                // Try to get the plane
                try
                {
                    // Get the ABB joint target of the mechanical unit
                    RobTarget robTarget = mechanicalUnits[i].GetPosition(coord);

                    int test = mechanicalUnits[i].DriveModule;

                    // Update Quaternion
                    _quat.A = robTarget.Rot.Q1;
                    _quat.B = robTarget.Rot.Q2;
                    _quat.C = robTarget.Rot.Q3;
                    _quat.D = robTarget.Rot.Q4;

                    // Update point
                    _point.X = robTarget.Trans.X;
                    _point.Y = robTarget.Trans.Y;
                    _point.Z = robTarget.Trans.Z;

                    // Convert to plane
                    _quat.GetRotation(out Plane plane);
                    plane = new Plane(_point, plane.XAxis, plane.YAxis);

                    // Add to list
                    planes.Add(plane);
                }

                // Set null plane if not
                catch
                {
                    // Raise a blank message
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Mechanical unit " + mechanicalUnits[i].Name + ": A position plane could not be defined for the selected coordinate system.");

                    // Add null plane
                    planes.Add(Plane.Unset);
                }

                // Add mechanical unit name to the list with names
                mechanicalUnitnames.Add(mechanicalUnits[i].Name);
            }

            // Output
            DA.SetDataList(0, planes);
            DA.SetDataList(1, mechanicalUnitnames);
        }

        // Method for creating the value list with coordinate system types
        #region valuelist
        /// <summary>
        /// Creates the value list for the motion type and connects it the input parameter is other source is connected
        /// </summary>
        private void CreateValueList()
        {
            if (this.Params.Input[1].SourceCount == 0)
            {
                // Gets the input parameter
                var parameter = Params.Input[1];

                // Creates the empty value list
                GH_ValueList obj = new GH_ValueList();
                obj.CreateAttributes();
                obj.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;
                obj.ListItems.Clear();

                // Add the items to the value list
                //obj.ListItems.Add(new GH_ValueListItem("Undefined", "0")); // doesn't return a robtarget
                obj.ListItems.Add(new GH_ValueListItem("World", "1"));
                obj.ListItems.Add(new GH_ValueListItem("Base", "2"));
                //obj.ListItems.Add(new GH_ValueListItem("Tool", "3")); // doesn't return a robtarget
                obj.ListItems.Add(new GH_ValueListItem("Work Object", "4"));

                // Make point where the valuelist should be created on the canvas
                obj.Attributes.Pivot = new PointF(parameter.Attributes.InputGrip.X - 120, parameter.Attributes.InputGrip.Y - 11);

                // Add the value list to the active canvas
                Instances.ActiveCanvas.Document.AddObject(obj, false);

                // Connect the value list to the input parameter
                parameter.AddSource(obj);

                // Collect data
                parameter.CollectData();

                // Set bool for expire solution of this component
                _expire = true;

                // First expire the solution of the value list
                obj.ExpireSolution(true);
            }
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

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return RobotComponentsABB.Properties.Resources.GetPlane_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3090E331-FF16-4D17-ACAC-513D288B37C4"); }
        }
    }
}