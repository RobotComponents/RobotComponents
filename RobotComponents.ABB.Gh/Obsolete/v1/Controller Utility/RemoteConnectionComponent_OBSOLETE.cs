// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Controller Utility : Setup a remote connection. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class RemoteConnectionComponent_OBSOLETE : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RemoteConnection class.
        /// </summary>
        public RemoteConnectionComponent_OBSOLETE()
          : base("Remote Connection", "Remote Connection",
              "Establishes a remote connection with the controller to upload an run RAPID code directly on a virtual or real ABB IRC5 robot controller."
              + System.Environment.NewLine + System.Environment.NewLine +
              "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Controller Utility")
        {
            this.Message = "-";
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot Controller", "RC", "Robot Controller to connect to as Robot Controller", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Connect", "C", "Create an online connection with the Robot Controller as bool", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Upload", "U", "Upload the RAPID code to the Robot as bool", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Run", "R", "Run as bool", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Stop", "S", "Stop/Pause as bool", GH_ParamAccess.item, false);
            pManager.AddTextParameter("Program Module", "PM", "Program Module code as a list with code lines", GH_ParamAccess.list);
            pManager.AddTextParameter("System Module", "SM", "System Module code as as list with code lines", GH_ParamAccess.list);

            pManager[5].Optional = true;
            pManager[6].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Status", "S", "Status of the ABB IRC5 robot controller", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "This component is OBSOLETE. Pick the new component from the toolbar.");
        }

        #region properties
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
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8bfb75d4-9122-45a3-9f11-8d01fb7ea069"); }
        }
        #endregion
    }
}