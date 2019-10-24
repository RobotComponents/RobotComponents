using System;
using System.Collections.Generic;
using System.Windows.Forms;
// ----- Grasshopper Libs -----
using Grasshopper.Kernel;
// ----- ABB Robotic Libs -----
using RobotComponents.Resources;
using RobotComponents.Goos;

namespace RobotComponents.Components
{
    public class GetControllerComponent : GH_Component
    {
        // private Global Variables
        public int pickedIndex = 0;
        public static List<ABB.Robotics.Controllers.Controller> controllerInstance = new List<ABB.Robotics.Controllers.Controller>();
        public ControllerGoo controllerGoo;
        public string outputString = "";
        public bool fromMenu = false;

        /// <summary>
        /// Initializes a new instance of the GetController class.
        /// </summary>
        public GetControllerComponent()
          : base("Get Controller", "GC",
              "Connects to a virtual or real ABB controller to extract data from it."
                + System.Environment.NewLine +
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Controller Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Update", "U", "Update Controller", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot Controller", "RC", "Robotic Controller", GH_ParamAccess.item);
            //pManager.AddTextParameter("debug", "D", "Debug Message", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declair Variables
            bool update = false;

            // retrieve data from inputs
            if (!DA.GetData(0, ref update)) { return; }

            if (update || fromMenu)
            {
                Rhino.RhinoApp.WriteLine("GetController call component");

                var controllerNow = GetController();
                if (controllerNow != null)
                {
                    controllerGoo = new ControllerGoo(controllerNow as ABB.Robotics.Controllers.Controller);
                    Rhino.RhinoApp.WriteLine("Controller");
                }
                else
                {
                    Rhino.RhinoApp.WriteLine("No controller");
                    return;
                }
            }
            else
            {
                Rhino.RhinoApp.WriteLine("Get Controller not call component in menu mode");
            }

            // Output
            DA.SetData(0, controllerGoo);
        }

        //  ----- Additional Functions -----
        #region Additional Functions

        private ABB.Robotics.Controllers.Controller GetController()
        {
            controllerInstance.Clear();
            ABB.Robotics.Controllers.ControllerInfo[] controllers;

            // Scann for Network
            ABB.Robotics.Controllers.Discovery.NetworkScanner scanner = new ABB.Robotics.Controllers.Discovery.NetworkScanner();
            scanner.Scan();

            try
            {
                controllers = scanner.GetControllers();
            }
            catch (Exception e)
            {
                Rhino.RhinoApp.WriteLine("Null Controller");
                controllers = null;
            }

            List<string> controllerNames = new List<string>();
            for (int i = 0; i < controllers.Length; i++)
            {
                controllerInstance.Add(ABB.Robotics.Controllers.ControllerFactory.CreateFrom(controllers[i]));
                controllerNames.Add(controllerInstance[i].Name);
            }

            if (controllerNames.Count == 1)
            {
                pickedIndex = 0;
            }
            else if (controllerNames.Count > 1)
            {
                pickedIndex = DisplayForm(controllerNames);
                if (pickedIndex < 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Station picked from menu!");
                    return null;
                }
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Station found!");
                return null;
            }

            return controllerInstance[pickedIndex];
        }

        private int DisplayForm(List<string> controllerNames)
        {
            PickControllerForm frm = new PickControllerForm(controllerNames);
            Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnEditor(frm, false);

            frm.ShowDialog();

            return PickControllerForm.stationIndex;
        }

        #endregion

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            // Additional Button
            Menu_AppendSeparator(menu);
            menu.Items.Add("Pick Controller", null, menuItemClick);

            base.AppendAdditionalMenuItems(menu);
        }

        public void menuItemClick(object sender, EventArgs e)
        {
            fromMenu = true;
            ExpireSolution(true);
            fromMenu = false;
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.GetController_Icon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6fd61c34-c262-4d10-b6e5-5c1762411aac"); }
        }
    }
}