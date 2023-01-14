// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Controller Utility : Get the Axis Values from a defined controller. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class GetAxisValuesComponent_OBSOLETE : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetAxisValues class.
        /// </summary>
        public GetAxisValuesComponent_OBSOLETE()
          : base("Get Axis Values", "GA",
              "Gets the current robot axis values from an ABB IRC5 robot controller."
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
            pManager.AddGenericParameter("Robot Controller", "RC", "Robot Controller as Robot Controller", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_RobotJointPosition(), "Robot Joint Position", "RJ", "Extracted Robot Joint Position");
            pManager.AddNumberParameter("External Axis Values", "EAV", "Extracted external Axis Values", GH_ParamAccess.tree);
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
            get { return RobotComponents.ABB.Gh.Properties.Resources.GetAxisValues_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2C546F24-938B-4C8A-85D9-22927E51E1FD"); }
        }
        #endregion
    }
}