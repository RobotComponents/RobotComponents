using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using RobotComponents.Goos;

namespace RobotComponents.Parameters
{

    public class AutoAxisConfigParameter : GH_PersistentGeometryParam<AutoAxisConfigGoo>, IGH_PreviewObject
    {
        public AutoAxisConfigParameter()
          : base(new GH_InstanceDescription("Auto Axis Configurator Parameter", "AACP", "Sets Auto Axis Configuration to True or False.", "RobotComponents", "Parameters"))
        {
        }

        public override string ToString()
        {
            return "Auto Axis Configurator";
        }


        public override string Name { get => "Auto Axis Configurator"; set => base.Name = value; }

        public override string Description { get => "Resulting Auto Axis Configurator"; set => base.Description = value; }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Auto_Parameter_Icon;
            }
        }
        public override GH_Exposure Exposure
        {
            get
            {
                // If you want to provide this parameter on the toolbars, use something other than hidden.
                return GH_Exposure.hidden;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("F413CBF2-C0C8-4FA4-A8A2-CC60D1F10343"); }
        }

        //We do not allow users to pick JointMovements, 
        //therefore the following 4 methods disable all this ui.
        protected override GH_GetterResult Prompt_Plural(ref List<AutoAxisConfigGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref AutoAxisConfigGoo value)
        {
            return GH_GetterResult.cancel;
        }
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
            item.Text = "Not available";
            item.Visible = false;
            return item;
        }
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
            item.Text = "Not available";
            item.Visible = false;
            return item;
        }

        #region preview methods
        public BoundingBox ClippingBox
        {
            get
            {
                return Preview_ComputeClippingBox();
            }
        }
        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            Preview_DrawMeshes(args);
        }
        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            //Use a standard method to draw wires, you don't have to specifically implement this.
            Preview_DrawWires(args);
        }

        private bool m_hidden = false;
        public bool Hidden
        {
            get { return m_hidden; }
            set { m_hidden = value; }
        }
        public bool IsPreviewCapable
        {
            get { return true; }
        }
        #endregion
    }


}
