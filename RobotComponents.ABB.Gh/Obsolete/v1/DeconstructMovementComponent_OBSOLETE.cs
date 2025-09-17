// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;
using RobotComponents.ABB.Gh.Parameters.Actions.Instructions;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Deconstruct Movement component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class DeconstructMovementComponent_OBSOLETE : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructMovement class.
        /// </summary>
        public DeconstructMovementComponent_OBSOLETE()
          : base("Deconstruct Move", "DeConMove",
              "Deconstructs a Move component into its parameters."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Deconstruct")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Movement(), "Movement", "M", "Movement as Movement", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Target(), "Target", "T", "Target as Target");
            pManager.RegisterParam(new Param_SpeedData(), "Speed Data", "SD", "Speed Data as Speed Data");
            pManager.Register_IntegerParam("Movement Type", "MT", "Movement Type as integer");
            pManager.RegisterParam(new Param_ZoneData(), "Zone Data", "ZD", "Zone Data as Zone Data");
            pManager.RegisterParam(new Param_RobotTool(), "Robot Tool", "RT", "Robot Tool as Robot Tool");
            pManager.RegisterParam(new Param_WorkObject(), "Work Object", "WO", "Work Object as Work Object");
            pManager.RegisterParam(new Param_SetDigitalOutput(), "Digital Output", "DO", "Digital Output as Digital Output");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            Movement movement = null;

            // Catch the input data
            if (!DA.GetData(0, ref movement)) { return; }

            if (movement != null)
            {
                // Check if the object is valid
                if (!movement.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Movement is not valid");
                }

                // Output
                DA.SetData(0, movement.Target);
                DA.SetData(1, movement.SpeedData);
                DA.SetData(2, (int)movement.MovementType);
                DA.SetData(3, movement.ZoneData);
                DA.SetData(4, movement.RobotTool);
                DA.SetData(5, movement.WorkObject);
                DA.SetData(6, movement.SetDigitalOutput);
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
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
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructMovement_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ACDB0D71-C805-4AEB-ADC9-99ACC740B096"); }
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
    }
}