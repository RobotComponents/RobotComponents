// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
// Rhno Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.BaseClasses.Actions;

namespace RobotComponentsGoos.Actions
{
    /// <summary>
    /// ZoneData wrapper class, makes sure ZoneData can be used in Grasshopper.
    /// </summary>
    public class GH_ZoneData : GH_GeometricGoo<ZoneData>, IGH_PreviewData
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_ZoneData()
        {
            this.Value = new ZoneData();
        }

        /// <summary>
        /// Data constructor
        /// </summary>
        /// <param name="zoneData"> ZoneData Value to store inside this Goo instance. </param>
        public GH_ZoneData(ZoneData zoneData)
        {
            if (zoneData == null)
                zoneData = new ZoneData();
            this.Value = zoneData;
        }

        /// <summary>
        /// Data constructor
        /// </summary>
        /// <param name="zoneDataGoo"> ZoneDataGoo to store inside this Goo instance. </param>
        public GH_ZoneData(GH_ZoneData zoneDataGoo)
        {
            if (zoneDataGoo == null)
                zoneDataGoo = new GH_ZoneData();
            this.Value = zoneDataGoo.Value;
        }

        /// <summary>
        /// Data constructor fron pre-defined zonedata value
        /// </summary>
        /// <param name="zone"> The size (the radius) of the TCP zone in mm.  </param>
        public GH_ZoneData(GH_Number zone)
        {
            this.Value = new ZoneData(zone.Value);
        }

        /// <summary>
        /// Data constructor fron pre-defined zonedata value
        /// </summary>
        /// <param name="zone"> The size (the radius) of the TCP zone in mm. </param>
        public GH_ZoneData(GH_Integer zone)
        {
            this.Value = new ZoneData(zone.Value);
        }

        /// <summary>
        /// Data constructor fron pre-defined zonedata value
        /// </summary>
        /// <param name="zone"> The size (the radius) of the TCP zone in mm. </param>
        public GH_ZoneData(GH_String zone)
        {
            if (zone == null)
            {
                this.Value = new ZoneData();
            }
            else
            {
                string text = zone.Value;
                double val = Convert.ToDouble(text);
                this.Value = new ZoneData(val);
            }
        }

        /// <summary>
        /// Data constructor fron pre-defined zonedata value
        /// </summary>
        /// <param name="zone"> The size (the radius) of the TCP zone in mm. </param>
        public GH_ZoneData(double zone)
        {
            this.Value = new ZoneData(zone);
        }

        /// <summary>
        /// Data constructor fron pre-defined zonedata value
        /// </summary>
        /// <param name="zone"> The size (the radius) of the TCP zone in mm. </param>
        public GH_ZoneData(int zone)
        {
            this.Value = new ZoneData(zone);
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the ZoneDataGoo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateZoneDataGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the ZoneDataGoo. </returns>
        public GH_ZoneData DuplicateZoneDataGoo()
        {
            return new GH_ZoneData(Value == null ? new ZoneData() : Value.Duplicate());
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the current value is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return Value.IsValid;
            }
        }

        /// <summary>
        /// ets a string describing the state of "invalidness". 
        /// If the instance is valid, then this property should return Nothing or String.Empty.
        /// </summary>
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal ZoneData instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid ZoneData instance: Did you define a name and pzone_tcp?"; 
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns> Description of the the current instance value. </returns>
        public override string ToString()
        {
            if (Value == null)
                return "Null Zone Data";
            else
                return Value.ToString();
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("ZoneData"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return ("Defines a single ZoneData"); }
        }

        /// <summary>
        /// Gets the boundingbox for this geometry.
        /// </summary>
        public override BoundingBox Boundingbox
        {
            get
            {
                return BoundingBox.Empty; //Note: beef this up if needed
            }
        }

        /// <summary>
        /// Compute an aligned boundingbox.
        /// </summary>
        /// <param name="xform"> Transformation to apply to geometry for BoundingBox computation. </param>
        /// <returns> The world aligned boundingbox of the transformed geometry. </returns>
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            return BoundingBox.Empty;
        }
        #endregion

        #region casting methods
        /// <summary>
        /// Attempt a cast to type Q.
        /// </summary>
        /// <typeparam name="Q"> Type to cast to. </typeparam>
        /// <param name="target"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(out Q target)
        {
            //Cast to ZoneData
            if (typeof(Q).IsAssignableFrom(typeof(ZoneData)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            //Cast to ZoneDataGoo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ZoneData)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_ZoneData(Value);
                return true;
            }

            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(RobotComponents.BaseClasses.Actions.Action)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            //Cast to ActionGoo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Action(Value);
                return true;
            }

            target = default(Q);
            return false;
        }

        /// <summary>
        /// Attempt a cast from generic object.
        /// </summary>
        /// <param name="source"> Reference to source of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            //Cast from ZoneData: Custom ZoneData
            if (typeof(ZoneData).IsAssignableFrom(source.GetType()))
            {
                Value = (ZoneData)source;
                return true;
            }

            //Cast from number: Predefined ZoneData
            if (typeof(GH_Number).IsAssignableFrom(source.GetType()))
            {
                Value = new ZoneData((source as GH_Number).Value);
                return true;
            }

            //Cast from Action
            if (typeof(RobotComponents.BaseClasses.Actions.Action).IsAssignableFrom(source.GetType()))
            {
                if (source is ZoneData action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from ActionGoo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is ZoneData action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from text: Predefined ZoneData
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                string text = (source as GH_String).Value;

                if (ZoneData.ValidPredefinedNames.Contains(text))
                {
                    if (text == "fine")
                    {
                        Value = new ZoneData(-1);
                        return true;
                    }
                    else
                    {
                        try
                        {
                            text = text.Replace("z", String.Empty); // Changes z1 to 1, z5 to 5 etc. 
                            double number = System.Convert.ToDouble(text);
                            Value = new ZoneData(number);
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
            }

            return false;

        }
        #endregion

        #region transformation methods
        /// <summary>
        /// Transforms the object or a deformable representation of the object.
        /// </summary>
        /// <param name="xform"> Transformation matrix. </param>
        /// <returns> Returns a null item since this goo instance has no geometry. </returns>
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            return null;
        }

        /// <summary>
        /// Morph the object or a deformable representation of the object.
        /// </summary>
        /// <param name="xmorph"> Spatial deform. </param>
        /// <returns> Returns a null item since this goo instance has no geometry. </returns>
        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            return null;
        }
        #endregion

        #region drawing methods
        /// <summary>
        /// Gets the clipping box for this data. The clipping box is typically the same as the boundingbox.
        /// </summary>
        public BoundingBox ClippingBox
        {
            get { return Boundingbox; }
        }

        /// <summary>
        /// Implement this function to draw all shaded meshes. 
        /// If the viewport does not support shading, this function will not be called.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
        }

        /// <summary>
        /// Implement this function to draw all wire and point previews.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
        }
        #endregion
    }
}
