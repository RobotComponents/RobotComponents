// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Gh.Parameters.Actions;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.18.000
// It is replaced with a new component. 

namespace RobotComponents.ABB.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Action : Ext Joint Position component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldExternalJointPositionComponent : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldExternalJointPositionComponent()
          : base("External Joint Position", "EJ",
              "Defines an External Joint Position for a Robot Target or Joint Target declaration."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.ABB.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
            // Create the component label with a message
            Message = "EXTENDABLE";
        }

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
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter(externalAxisParameters[0].Name, externalAxisParameters[0].NickName, externalAxisParameters[0].Description, GH_ParamAccess.item, 9e9);
            pManager.AddNumberParameter(externalAxisParameters[1].Name, externalAxisParameters[1].NickName, externalAxisParameters[1].Description, GH_ParamAccess.item, 9e9);

            pManager[0].Optional = externalAxisParameters[0].Optional;
            pManager[1].Optional = externalAxisParameters[1].Optional;
        }

        // Create an array with the parameters for the positions of the external logical axes
        readonly IGH_Param[] externalAxisParameters = new IGH_Param[6]
        {
            new Param_Number() { Name = "External axis position A", NickName = "EAa", Description = "Defines the position of external logical axis A", Access = GH_ParamAccess.item, Optional = true }, // fixed
            new Param_Number() { Name = "External axis position B", NickName = "EAb", Description = "Defines the position of external logical axis B", Access = GH_ParamAccess.item, Optional = true }, // fixed
            new Param_Number() { Name = "External axis position C", NickName = "EAc", Description = "Defines the position of external logical axis C", Access = GH_ParamAccess.item, Optional = true }, // variable
            new Param_Number() { Name = "External axis position D", NickName = "EAd", Description = "Defines the position of external logical axis D", Access = GH_ParamAccess.item, Optional = true }, // variable
            new Param_Number() { Name = "External axis position E", NickName = "EAe", Description = "Defines the position of external logical axis E", Access = GH_ParamAccess.item, Optional = true }, // variable
            new Param_Number() { Name = "External axis position F", NickName = "EAf", Description = "Defines the position of external logical axis F", Access = GH_ParamAccess.item, Optional = true } // variable
        };

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_ExternalJointPosition(), "External Joint Position", "EJ", "The resulting external joint position");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            double externalAxisValueA = 9e9;
            double externalAxisValueB = 9e9;
            double externalAxisValueC = 9e9;
            double externalAxisValueD = 9e9;
            double externalAxisValueE = 9e9;
            double externalAxisValueF = 9e9;

            // External axis A
            if (Params.Input.Any(x => x.Name == externalAxisParameters[0].Name))
            {
                if (!DA.GetData(externalAxisParameters[0].Name, ref externalAxisValueA)) { externalAxisValueA = 9e9; }
            }
            // External axis B
            if (Params.Input.Any(x => x.Name == externalAxisParameters[1].Name))
            {
                if (!DA.GetData(externalAxisParameters[1].Name, ref externalAxisValueB)) { externalAxisValueB = 9e9; }
            }
            // External axis C
            if (Params.Input.Any(x => x.Name == externalAxisParameters[2].Name))
            {
                if (!DA.GetData(externalAxisParameters[2].Name, ref externalAxisValueC)) { externalAxisValueC = 9e9; }
            }
            // External axis D
            if (Params.Input.Any(x => x.Name == externalAxisParameters[3].Name))
            {
                if (!DA.GetData(externalAxisParameters[3].Name, ref externalAxisValueD)) { externalAxisValueD = 9e9; }
            }
            // External axis E
            if (Params.Input.Any(x => x.Name == externalAxisParameters[4].Name))
            {
                if (!DA.GetData(externalAxisParameters[4].Name, ref externalAxisValueE)) { externalAxisValueE = 9e9; }
            }
            // External axis F
            if (Params.Input.Any(x => x.Name == externalAxisParameters[5].Name))
            {
                if (!DA.GetData(externalAxisParameters[5].Name, ref externalAxisValueF)) { externalAxisValueF = 9e9; }
            }

            // Create external joint position
            ExternalJointPosition extJointPosition = new ExternalJointPosition(externalAxisValueA, externalAxisValueB, externalAxisValueC, externalAxisValueD, externalAxisValueE, externalAxisValueF);

            // Sets Output
            DA.SetData(0, extJointPosition);
        }

        // Methods of variable parameter interface which handles (de)serialization of the variable input parameters
        #region variable input parameters
        /// <summary>
        /// This function needs to be called to add an input parameter to override the external axis value. 
        /// </summary>
        /// <param name="index"> The index number of the variable input parameter that needs to be added.
        /// In this case the index number of the array with variable input parameters. </param>
        private void AddExternalAxisValueParameter(int index)
        {
            // Pick the parameter that needs to be added
            IGH_Param parameter = externalAxisParameters[index];

            // Register the input parameter
            Params.RegisterInputParam(parameter, index);

            // Refresh parameters since they changed
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        /// <summary>
        /// This function will get called before an attempt is made to insert a parameter. 
        /// Since this method is potentially called on Canvas redraws, it must be fast.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if your component supports a variable parameter at the given location </returns>
        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            // Don't allow for insert before or in between the fixed input parameters
            if (side == GH_ParameterSide.Input && index < 2)
            {
                return false;
            }
            // Don't allow for insert if all variable input parameters are already added
            else if (side == GH_ParameterSide.Input && index == (externalAxisParameters.Length))
            {
                return false;
            }
            // Allow insert after the last input parameters
            else if (side == GH_ParameterSide.Input && index == Params.Input.Count)
            {
                return true;
            }
            // Don't allow for inserting new output parameters
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This function will get called before an attempt is made to insert a parameter. 
        /// Since this method is potentially called on Canvas redraws, it must be fast.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if your component supports a variable parameter at the given location. </returns>
        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            // If the first external axis override parameter is added it is allowed to remove parameters
            if (Params.Input.Any(x => x.Name == externalAxisParameters[0].Name))
            {
                // Makes it impossible to remove the fixed input parameters
                if (side == GH_ParameterSide.Input && index < 2)
                {
                    return false;
                }
                // Makes it possible to remove the last variable input parameter
                else if (side == GH_ParameterSide.Input && index == Params.Input.Count - 1)
                {
                    return true;
                }
                // Makes it impossible to remove all the other input and output parameters
                else
                {
                    return false;
                }
            }

            else
            {
                return false;
            }
        }

        /// <summary>
        /// This function will be called when a new parameter is about to be inserted. 
        /// You must provide a valid parameter or insertion will be skipped. 
        /// You do not, repeat not, need to insert the parameter yourself.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> A valid IGH_Param instance to be inserted. In our case a null value. </returns>
        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
        {
            // Add input parameter
            AddExternalAxisValueParameter(index);

            // This method always returns a null value
            return null;
        }

        /// <summary>
        /// This function will be called when a parameter is about to be removed. 
        /// You do not need to do anything, but this would be a good time to remove 
        /// any event handlers that might be attached to the parameter in question.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if the parameter in question can indeed be removed. Note, this is only in emergencies, 
        /// typically the CanRemoveParameter function should return false if the parameter in question is not removable. </returns>
        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
        {
            // If the first external axis is added it is allowed to destroy input parameters
            if (Params.Input.Any(x => x.Name == externalAxisParameters[0].Name))
            {
                // Makes it impossible to detroy the fixed input parameters
                if (side == GH_ParameterSide.Input && index < 2)
                {
                    return false;
                }

                // Makes it impossible to destroy the output parameters
                else if (side == GH_ParameterSide.Output)
                {
                    return false;
                }

                // Makes it possible to destroy all the other parameters
                else
                {
                    return true;
                }
            }

            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method will be called when a closely related set of variable parameter operations completes. 
        /// This would be a good time to ensure all Nicknames and parameter properties are correct. 
        /// This method will also be called upon IO operations such as Open, Paste, Undo and Redo.
        /// </summary>
        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {

        }
        #endregion

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.ExternalJointPosition_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4013FF60-6EBB-4882-A7D4-48A0ADF9170B"); }
        }

    }
}
