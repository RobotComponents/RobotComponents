// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2018-2020 EDEK Uni Kassel
// Copyright (c) 2020-2025 Arjen Deetman
//
// Authors:
//   - Gabriel Rumph (2018-2020)
//   - Benedikt Wannemacher (2018-2020)
//   - Arjen Deetman (2019-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Dynamic;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Gh.Parameters.Actions.Dynamic;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Comment component.
    /// </summary>
    public class CommentComponent : GH_RobotComponent
    {
        #region fields
        private bool _expire = false;
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public CommentComponent() : base("Comment", "C", "Code Generation", 
            "Defines a single comment line.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Text", "T", "Content of the Comment as text", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Type", "T", "Type of the Comment as integer. Use 0 for commenting on instructions, 1 for commenting on declarations", GH_ParamAccess.item, 0);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Comment(), "Comment", "C", "Resulting Comment", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Creates the input value list and attachs it to the input parameter
            if (this.Params.Input[1].SourceCount == 0)
            {
                _expire = true;
                HelperMethods.CreateValueList(this, typeof(CodeType), 1);
            }

            // Expire solution of this component
            if (_expire == true)
            {
                _expire = false;
                this.ExpireSolution(true);
            }

            // Input variables
            string text = null;
            int type = 0;

            // Catch the inut data
            if (!DA.GetData(0, ref text)) { return; }
            if (!DA.GetData(1, ref type)) { return; }

            // Check if a right value is used for the comment type
            if (type != 0 && type != 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Comment type value <" + type + "> is invalid. " +
                    "In can only be set to 0 or 1. Use 0 for commenting on instructions and 1 for commenting on declarations.");
            }

            // Split input if enter is used
            string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // Create output
            List<Comment> comments = new List<Comment>();

            for (int i = 0; i < lines.Length; i++)
            {
                comments.Add(new Comment(lines[i], (CodeType)type));
            }

            // Sets Output
            DA.SetDataList(0, comments);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
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
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.Comment_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("00C290AE-7946-4245-ABFD-EF71616B1C35"); }
        }
        #endregion
    }
}
