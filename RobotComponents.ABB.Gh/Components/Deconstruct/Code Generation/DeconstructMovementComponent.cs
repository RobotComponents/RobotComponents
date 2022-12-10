// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Gh.Parameters.Actions;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.Deconstruct.CodeGeneration
{
    /// <summary>
    /// RobotComponents Deconstruct Movement component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructMovementComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructMovement class.
        /// </summary>
        public DeconstructMovementComponent()
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
            pManager.Register_IntegerParam("Type", "TY", "Type as Integer");
            pManager.RegisterParam(new Param_Target(), "Target", "TA", "Target as Target");
            pManager.RegisterParam(new Param_SpeedData(), "Speed Data", "SD", "Speed Data as Speed Data");
            pManager.Register_DoubleParam("Time", "TI", "The total movement time in seconds as Number");
            pManager.RegisterParam(new Param_ZoneData(), "Zone Data", "ZD", "Zone Data as Zone Data");
            pManager.RegisterParam(new Param_RobotTool(), "Robot Tool", "RT", "Robot Tool as Robot Tool");
            pManager.RegisterParam(new Param_WorkObject(), "Work Object", "WO", "Work Object as Work Object");
            pManager.RegisterParam(new Param_DigitalOutput(), "Digital Output", "DO", "Digital Output as Digital Output");
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
                DA.SetData(0, (int)movement.MovementType);
                DA.SetData(1, movement.Target);
                DA.SetData(2, movement.SpeedData);
                DA.SetData(3, movement.Time);
                DA.SetData(4, movement.ZoneData);
                DA.SetData(5, movement.RobotTool);
                DA.SetData(6, movement.WorkObject);
                DA.SetData(7, movement.DigitalOutput);
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
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
            get { return new Guid("3A5719AC-9CDB-4255-BD25-425144C36D8A"); }
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