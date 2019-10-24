using System;
using System.Collections.Generic;
using System.Windows.Forms;
// ----- Grasshopper Libs -----
using Grasshopper.Kernel;
// ----- ABB Robotic Libs -----
using RobotComponents.Resources;
using ABB.Robotics.Controllers;
using RobotComponents.Goos;

namespace RobotComponents.Components
{
    public class GetDigitalInputComponent : GH_Component
    {
        // private Global Variables
        public int pickedIndex = 0;
        public static List<SignalGoo> signalGooList = new List<SignalGoo>();
        ABB.Robotics.Controllers.Controller controller = null;

        /// <summary>
        /// Initializes a new instance of the GetAxisValues class.
        /// </summary>
        public GetDigitalInputComponent()
          : base("Get Digital Input", "GetDI",
              "Gets a digital input from a defined ABB robot controller."
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
            pManager.AddGenericParameter("Robot Controller", "RC", "Controller to be connected to", GH_ParamAccess.item);
            pManager.AddGenericParameter("DI Name", "N", "Digital Input Name as string", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Signal", "S", "The Digital Input Signal", GH_ParamAccess.item);
            pManager.AddBooleanParameter("State", "S", "The State of the Digital Input.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declair Variables
            ControllerGoo controllerGoo = null;
            string nameIO = "";

            // retrieve data from inputs
            if (!DA.GetData(0, ref controllerGoo)) { return; }
            if (!DA.GetData(1, ref nameIO))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "IOName: No Signal Selected");
                nameIO = "";
            }

            controller = controllerGoo.Value;
            controller.Logon(UserInfo.DefaultUser);

            SignalGoo signalGoo;

            // check for null returns !!!
            if (nameIO == null || nameIO == "")
            {
                signalGoo = PickSignal();
                CreatePanel(signalGoo.Value.Name.ToString());
            }
            else
            {
                signalGoo = GetSignal(nameIO);
            }

            // Declair Signal
            ABB.Robotics.Controllers.IOSystemDomain.DigitalSignal signal = signalGoo.Value;

            // Convert Signal in bool 
            bool signalValue = false;
            if (signal.Value == 1)
            {
                signalValue = true;
            }
            else
            {
                signalValue = false;
            }

            // Output
            DA.SetData(0, signalGoo);
            DA.SetData(1, signalValue);
        }

        //  ----- Additional Functions -----
        #region Additional_Functions


        private SignalGoo PickSignal()
        {
            signalGooList.Clear();
            ABB.Robotics.Controllers.IOSystemDomain.SignalCollection signalCollection;
            signalCollection = controller.IOSystem.GetSignals(ABB.Robotics.Controllers.IOSystemDomain.IOFilterTypes.Input);

            List<string> signalNames = new List<string>();

            for (int i = 0; i < signalCollection.Count; i++)
            {
                //Check if i has Write Access
                if (controller.Configuration.Read("EIO", "EIO_SIGNAL", signalCollection[i].Name, "Access") != "ReadOnly")
                {
                    signalNames.Add(signalCollection[i].Name);
                    signalGooList.Add(new SignalGoo(signalCollection[i] as ABB.Robotics.Controllers.IOSystemDomain.DigitalSignal));
                }
            }

            if (signalGooList.Count == 0 || signalGooList == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No signal found with write access!");
                return null;
            }

            pickedIndex = DisplayForm(signalNames);

            if (pickedIndex >= 0)
            {
                return signalGooList[pickedIndex];
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No signal selected in menu");
                return null;
            }

        }

        private SignalGoo GetSignal(string name)
        {
            if (!ValidSignal(name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The Signal " + name + " does not exist in the current Controller");
                return null;
            }

            ABB.Robotics.Controllers.IOSystemDomain.Signal signal = controller.IOSystem.GetSignal(name) as ABB.Robotics.Controllers.IOSystemDomain.Signal;

            if (signal != null)
            {
                if (controller.Configuration.Read("EIO", "EIO_SIGNAL", signal.Name, "Access") == "ReadOnly")
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Blank, "The Picked Signal is ReadOnly!");
                }
                return new SignalGoo(signal as ABB.Robotics.Controllers.IOSystemDomain.DigitalSignal);
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Blank, "The Picked Signal does not Exist!");
                return null;
            }

        }

        // Creates a Panel Component
        private void CreatePanel(string text)
        {
            Grasshopper.Kernel.Special.GH_Panel panel = new Grasshopper.Kernel.Special.GH_Panel();
            panel.SetUserText(text);
            panel.Properties.Colour = System.Drawing.Color.White;
            GH_Document doc = Grasshopper.Instances.ActiveCanvas.Document;
            doc.AddObject(panel, false);
            panel.Attributes.Bounds = new System.Drawing.RectangleF(0.0f, 0.0f, 80.0f, 25.0f);
            panel.Attributes.Pivot = new System.Drawing.PointF(
                (float)this.Attributes.DocObject.Attributes.Bounds.Left - panel.Attributes.Bounds.Width - 30,
                (float)this.Params.Input[1].Attributes.Bounds.Y - 2);

            //connects the panel with the Input
            Params.Input[1].AddSource(panel);
        }

        // Check if Signal exists
        private bool ValidSignal(string signalName)
        {
            bool result = false;

            ABB.Robotics.Controllers.IOSystemDomain.SignalCollection signalCollection;
            signalCollection = controller.IOSystem.GetSignals(ABB.Robotics.Controllers.IOSystemDomain.IOFilterTypes.Input);

            List<string> signalNames = new List<string>();

            for (int i = 0; i < signalCollection.Count; i++)
            {
                //Check if i has Write Access
                if (controller.Configuration.Read("EIO", "EIO_SIGNAL", signalCollection[i].Name, "Access") != "ReadOnly")
                {
                    signalNames.Add(signalCollection[i].Name);
                }
            }

            result = signalNames.Contains(signalName);

            return result;
        }

        public ABB.Robotics.Controllers.IOSystemDomain.DigitalSignal GetSignalByIndex(int index)
        {
            return null;
        }

        private int DisplayForm(List<string> controllerNames)
        {
            PickDIForm frm = new PickDIForm(controllerNames);
            Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnEditor(frm, false);

            frm.ShowDialog();

            return PickDIForm.stationIndex;
        }
        #endregion Additional_Functions

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            // Additional Button
            Menu_AppendSeparator(menu);
            menu.Items.Add("Pick Signal", null, MenuItemClick);

            base.AppendAdditionalMenuItems(menu);
        }

        public void MenuItemClick(object sender, EventArgs e)
        {
            this.Params.Input[1].RemoveAllSources();
            ExpireSolution(true);
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
                return Properties.Resources.GetDigitalInput_Icon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("CB18B826-C66E-41AE-A102-B0C6BEB285B3"); }
        }
    }
}