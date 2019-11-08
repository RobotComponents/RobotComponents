using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses;

namespace RobotComponents.Goos
{
    /// <summary>
    /// RobotInfo Goo wrapper class, makes sure RobotInfo can be used in Grasshopper.
    /// </summary>
    public class RobotInfoGoo : GH_GeometricGoo<RobotInfo>, IGH_PreviewData
    {
        #region constructors
        public RobotInfoGoo()
        {
            this.Value = new RobotInfo();
        }

        public RobotInfoGoo(RobotInfo robotInfo)
        {
            if (robotInfo == null)
                robotInfo = new RobotInfo();
            this.Value = robotInfo;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateRobotInfo();
        }

        public RobotInfoGoo DuplicateRobotInfo()
        {
            return new RobotInfoGoo(Value == null ? new RobotInfo() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return Value.IsValid;
            }
        }
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal RobotInfo instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid RobotInfo instance: Did you define an AxisValues, AxisLimits, BasePlane and MountingFrame?"; //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null RobotInfo";
            else
                return "Robot Info";
        }
        public override string TypeName
        {
            get { return ("RobotInfo"); }
        }
        public override string TypeDescription
        {
            get { return ("Defines a single RobotInfo"); }
        }

        public override BoundingBox Boundingbox
        {
            get
            {
                if (Value == null) { return BoundingBox.Empty; }
                if (Value.Meshes == null) { return BoundingBox.Empty; }
                else
                {
                    // Make the bounding box at the base plane
                    BoundingBox MeshBoundingBox = BoundingBox.Empty;
                    for (int i = 0; i != Value.Meshes.Count; i++)
                    {
                        MeshBoundingBox.Union(Value.Meshes[i].GetBoundingBox(true));
                    }
                    
                    Transform orientNow;
                    orientNow = Rhino.Geometry.Transform.ChangeBasis(Value.BasePlane, Plane.WorldXY);
                    MeshBoundingBox.Transform(orientNow);

                    return MeshBoundingBox;
                }
            }
        }
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            return Boundingbox;
        }
        #endregion

        #region casting methods
        public override bool CastTo<Q>(out Q target)
        {
            //Cast to RobotInfo.
            if (typeof(Q).IsAssignableFrom(typeof(RobotInfo)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            //Cast to Mesh.
            if (typeof(Q).IsAssignableFrom(typeof(List<Mesh>)))
            {
                if (Value == null)
                    target = default(Q);
                else if (Value.Meshes == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value.Meshes;
                return true;
            }

            //Todo: cast to point, number, mesh, curve?

            target = default(Q);
            return false;
        }
        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            //Cast from RobotInfo
            if (typeof(RobotInfo).IsAssignableFrom(source.GetType()))
            {
                Value = (RobotInfo)source;
                return true;
            }

            return false;
        }
        #endregion

        #region transformation methods
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            return null;
        }
        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            return null;
        }
        #endregion

        #region drawing methods
        public BoundingBox ClippingBox
        {
            get { return Boundingbox; }
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            if (Value == null) { return; }
            if (Value.Meshes != null)
            {
                // Used in DrawViewportMeshes to show Robot correct in viewport
                ForwardKinematics forwardKinematics = new ForwardKinematics(this.Value, new List<double> { 0, 0, 0, 0, 0, 0 }, new List<double> { 0, 0, 0, 0, 0, 0 });
                forwardKinematics.Calculate();

                if (forwardKinematics.PosedMeshes != null)
                {
                    for (int i = 0; i != forwardKinematics.PosedMeshes.Count; i++)
                    {
                        args.Pipeline.DrawMeshShaded(forwardKinematics.PosedMeshes[i], new Rhino.Display.DisplayMaterial(System.Drawing.Color.FromArgb(225, 225, 225), 0));
                    }
                }
            }
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value == null) { return; }

            //Draw hull shape.
            if (Value.Meshes != null)
            {
                ForwardKinematics forwardKinematics = new ForwardKinematics(this.Value, new List<double> { 0, 0, 0, 0, 0, 0 }, new List<double> { 0, 0, 0, 0, 0, 0 });
                forwardKinematics.Calculate();

                if (forwardKinematics.PosedMeshes != null)
                {
                    for (int i = 0; i != forwardKinematics.PosedMeshes.Count; i++)
                    {
                        args.Pipeline.DrawMeshWires(forwardKinematics.PosedMeshes[i], args.Color, -1);
                    }
                }
            }
        }
        #endregion
    }
}
