// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Gh.Parameters.Actions;
using RobotComponents.ABB.Actions;

namespace RobotComponents.ABB.Gh.Components.Utilities
{
    /// <summary>
    /// RobotComponents Ungroup Actions component.
    /// </summary>
    public class UngroupActionsComponent : GH_RobotComponent
    {
        /// <summary>
        /// Initializes a new instance of the FlipPlaneComponent class.
        /// </summary>
        public UngroupActionsComponent() : base("Ungroup Actions", "Ungroup", "Utility",
              "Ungroup a set of Actions.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_ActionGroup(), "Group", "G", "Grouped Actions", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Action(), "Actions", "A", "Actions inside Group.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            ActionGroup group = new ActionGroup();

            // Catch the input data
            if (!DA.GetData(0, ref group)) { return; }

            // Output
            DA.SetDataList(0, group.Actions);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.ActionUnGroup_Icon; ; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("91B0E8F6-BA1F-4480-8E85-1315E01CB625"); }
        }
        #endregion
    }
}