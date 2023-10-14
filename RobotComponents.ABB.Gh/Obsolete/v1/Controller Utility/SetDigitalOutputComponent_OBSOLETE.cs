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
    /// RobotComponents Controller Utility : Get and set the Digital Outputs on a defined controller. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class SetDigitalOutputComponent_OBSOLETE : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetDigitalOutput class.
        /// </summary>
        public SetDigitalOutputComponent_OBSOLETE()
          : base("Set Digital Output", "SetDO",
              "Changes the state of a defined digital output from an ABB IRC5 robot controller in Realtime."
               + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Controller Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot Controller", "RC", "Robot Controller to connected to as Robot Controller", GH_ParamAccess.item);
            pManager.AddTextParameter("DO Name", "N", "Name of the Digital Output as text", GH_ParamAccess.item);
            pManager.AddBooleanParameter("State", "S", "State of the Digital Output as bool", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Update", "U", "Updates the Digital Input as bool", GH_ParamAccess.item, false);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Signal", "S", "Signal of the Digital Output", GH_ParamAccess.item);
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
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.SetDigitalOutput_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8fbb97b5-cdc4-41de-a33d-df1d6f7bda21"); }
        }
        #endregion
    }
}