// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Goos;
using RobotComponents.Gh.Utils;
// ABB Libs
using ABB.Robotics.Controllers.MotionDomain;

namespace RobotComponents.Gh.Components.ControllerUtility
{
    /// <summary>
    /// RobotComponents Controller Utility : Get the Axis Values from a defined controller. An inherent from the GH_Component Class.
    /// </summary>
    public class GetAxisValuesComponent : GH_Component
    {
        #region fields
        private readonly List<RobotJointPosition> _robotJointPositions = new List<RobotJointPosition>();
        private readonly GH_Structure<GH_Number> _externalAxisValues = new GH_Structure<GH_Number>();
        #endregion

        /// <summary>
        /// Initializes a new instance of the GetAxisValues class.
        /// </summary>
        public GetAxisValuesComponent()
          : base("Get Axis Values", "GA",
              "Gets the current robot axis values from an ABB IRC5 robot controller."
               + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Controller Utility")
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
            // Input variables
            GH_Controller controllerGoo = null;

            // Catch input data
            if (!DA.GetData(0, ref controllerGoo)) { return; }

            // Clear output variables 
            _robotJointPositions.Clear();
            _externalAxisValues.Clear();

            // Data needed for making the datatree with axis values
            MechanicalUnitCollection mechanicalUnits = controllerGoo.Value.MotionSystem.MechanicalUnits;
            int externalAxisValuesPath = 0;
            List<double> values;
            GH_Path path;

            // Make the output datatree with names with a branch for each mechanical unit
            for (int i = 0; i < mechanicalUnits.Count; i++)
            {
                // Get the ABB joint target of the mechanical unit
                MechanicalUnit mechanicalUnit = mechanicalUnits[i];
                ABB.Robotics.Controllers.RapidDomain.JointTarget jointTarget = mechanicalUnit.GetPosition();

                // For internal axis values
                if (mechanicalUnit.Type == MechanicalUnitType.TcpRobot)
                {
                    values = GetInternalAxisValuesAsList(jointTarget);
                    _robotJointPositions.Add(new RobotJointPosition(values));
                }

                // For external axis values
                else
                {
                    values = GetExternalAxisValuesAsList(jointTarget);
                    path = new GH_Path(externalAxisValuesPath);
                    _externalAxisValues.AppendRange(values.GetRange(0, mechanicalUnit.NumberOfAxes).ConvertAll(val => new GH_Number(val)), path);
                    externalAxisValuesPath += 1;
                }
            }

            // Output
            DA.SetDataList(0, _robotJointPositions);
            DA.SetDataTree(1, _externalAxisValues);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
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
            get { return RobotComponents.Gh.Properties.Resources.GetAxisValues_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2C546F24-938B-4C8A-85D9-22927E51E1FD"); }
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

        #region additional methods
        /// <summary>
        /// Get the internal axis values from a defined joint target
        /// </summary>
        /// <param name="jointTarget"> The joint target to get the internal axis values from. </param>
        /// <returns></returns>
        private List<double> GetInternalAxisValuesAsList(ABB.Robotics.Controllers.RapidDomain.JointTarget jointTarget)
        {
            // Initiate the list with internal axis values
            List<double> result = new List<double>() { };

            // Get the axis values from the joint target
            result.Add(jointTarget.RobAx.Rax_1);
            result.Add(jointTarget.RobAx.Rax_2);
            result.Add(jointTarget.RobAx.Rax_3);
            result.Add(jointTarget.RobAx.Rax_4);
            result.Add(jointTarget.RobAx.Rax_5);
            result.Add(jointTarget.RobAx.Rax_6);

            // Replace large numbers (the not connected axes) with an axis value equal to zero 
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] > 9.0e+8)
                {
                    result[i] = 0;
                }
            }

            // Return the list with axis values
            return result;
        }

        /// <summary>
        /// Get the external axis values from a defined joint target
        /// </summary>
        /// <param name="jointTarget"> The joint target to get the external axis values from. </param>
        /// <returns></returns>
        private List<double> GetExternalAxisValuesAsList(ABB.Robotics.Controllers.RapidDomain.JointTarget jointTarget)
        {
            // Initiate the list with external axis values
            List<double> result = new List<double>() { };

            // Get the axis values from the joint target
            result.Add(jointTarget.ExtAx.Eax_a);
            result.Add(jointTarget.ExtAx.Eax_b);
            result.Add(jointTarget.ExtAx.Eax_c);
            result.Add(jointTarget.ExtAx.Eax_d);
            result.Add(jointTarget.ExtAx.Eax_e);
            result.Add(jointTarget.ExtAx.Eax_f);

            // Replace large numbers (the not connected axes)
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] > 9.0e+8)
                {
                    result[i] = 9e9;
                }
            }

            // Return the list with axis values
            return result;
        }
        #endregion
    }
}