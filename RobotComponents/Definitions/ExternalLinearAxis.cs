// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;

namespace RobotComponents.BaseClasses.Definitions
{
    /// <summary>
    /// External linear axis class, main class for external linear axis.
    /// </summary>
    public class ExternalLinearAxis : ExternalAxis
    {
        #region fields
        private string _name; // The name of the external axis
        private Plane _attachmentPlane; // The plane where the robot or the work object is attached
        private Plane _axisPlane; // Z-Axis of the _axisPlane is the linear axis
        private Interval _axisLimits; // The movement limits
        private int? _axisNumber; // TODO: The axis logic number
        private Mesh _baseMesh; // The base mesh (fixed)
        private Mesh _linkMesh; // The link mesh posed for axis value 0
        private Curve _axisCurve; // The axis curve
        private List<Mesh> _posedMeshes; // The mesh posed for a certain axis value
        #endregion

        #region constructors
        /// <summary>
        /// An empty constuctor that creates an empty external linear axis.
        /// </summary>
        public ExternalLinearAxis()
        {
            _name = "";
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();
        }

        /// <summary>
        /// Defines an external linear axis with empty meshes. 
        /// </summary>
        /// <param name="attachmentPlane"> The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction of the external linear axis as a vector. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis as an interval. </param>
        public ExternalLinearAxis(Plane attachmentPlane, Vector3d axis, Interval axisLimits)
        {
            axis.Unitize();

            _name = "";
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = null;
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Defines an external linear axis with a mesh geometry. 
        /// </summary>
        /// <param name="attachmentPlane"> The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction of the external linear axis as a vector. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis as an interval. </param>
        /// <param name="baseMesh"> The base mesh of the external linear axis. </param>
        /// <param name="linkMesh"> The link mesh of the external linear axis posed for external axis value 0. </param>
        public ExternalLinearAxis(Plane attachmentPlane, Vector3d axis, Interval axisLimits, Mesh baseMesh, Mesh linkMesh)
        {
            axis.Unitize();

            _name = "";
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = null; 
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Defines an external linear axis with a mesh geometry.
        /// </summary>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axisPlane"> The axis plane. The Z-axis defines the positive movement direction of the axis. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis as an interval. </param>
        /// <param name="baseMesh"> The base mesh of the external linear axis. </param>
        /// <param name="linkMesh"> The link mesh of the external linear axis posed for external axis value 0. </param>
        public ExternalLinearAxis(Plane attachmentPlane, Plane axisPlane, Interval axisLimits, Mesh baseMesh, Mesh linkMesh)
        {
            _name = "";
            _attachmentPlane = attachmentPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = null; 
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Defines an external linear axis with a mesh geometry and a name.
        /// </summary>
        /// <param name="name"> The axis name as a string. </param>
        /// <param name="attachmentPlane"> The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction of the external linear axis as a vector. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis as an interval. </param>
        /// <param name="baseMesh"> The base mesh of the external linear axis. </param>
        /// <param name="linkMesh"> The link mesh of the external linear axis posed for external axis value 0. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Vector3d axis, Interval axisLimits, Mesh baseMesh, Mesh linkMesh)
        {
            axis.Unitize();

            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = null; 
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Defines an external linear axis with a mesh geometry and a name.
        /// </summary>
        /// <param name="name"> The axis name as a string. </param>
        /// <param name="attachmentPlane"> The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axis"> The positive movement direction of the external linear axis as a vector. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis as an interval. </param>
        /// <param name="baseMeshes"> The base mesh of the external linear axis as a list with Meshes </param>
        /// <param name="linkMeshes"> The link mesh of the external linear axis posed for external axis value 0 as a list with Meshes. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Vector3d axis, Interval axisLimits, List<Mesh> baseMeshes, List<Mesh> linkMeshes)
        {
            axis.Unitize();

            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = new Plane(attachmentPlane.Origin, axis);
            _axisLimits = axisLimits;
            _axisNumber = null;
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            for (int i = 0; i < baseMeshes.Count; i++) { _baseMesh.Append(baseMeshes[i]); }
            for (int i = 0; i < linkMeshes.Count; i++) { _linkMesh.Append(linkMeshes[i]); }

            Initialize();
        }

        /// <summary>
        /// Defines an external linear axis with a mesh geometry and a name. 
        /// </summary>
        /// <param name="name"> The axis name as a string. </param>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axisPlane"> The axis plane. The Z-axis defines the positive movement direction of the axis. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis as an interval. </param>
        /// <param name="baseMesh"> The base mesh of the external linear axis. </param>
        /// <param name="linkMesh"> The link mesh of the external linear axis posed for external axis value 0. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Plane axisPlane, Interval axisLimits, Mesh baseMesh, Mesh linkMesh)
        {
            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = null;
            _baseMesh = baseMesh;
            _linkMesh = linkMesh;
            _posedMeshes = new List<Mesh>();

            Initialize();
        }

        /// <summary>
        /// Defines an external linear axis with a mesh geometry and a name. 
        /// </summary>
        /// <param name="name"> The axis name as a string. </param>
        /// <param name="attachmentPlane" > The attachment plane posed at the location for axis value 0. </param>
        /// <param name="axisPlane"> The axis plane. The Z-axis defines the positive movement direction of the axis. </param>
        /// <param name="axisLimits"> The movement limits of the external linear axis as an interval. </param>
        /// <param name="baseMeshes"> The base mesh of the external linear axis as list with meshes </param>
        /// <param name="linkMeshes"> The link mesh of the external linear axis posed for external axis value 0 as a list with meshes. </param>
        public ExternalLinearAxis(string name, Plane attachmentPlane, Plane axisPlane, Interval axisLimits, List<Mesh> baseMeshes, List<Mesh> linkMeshes)
        {
            _name = name;
            _attachmentPlane = attachmentPlane;
            _axisPlane = axisPlane;
            _axisLimits = axisLimits;
            _axisNumber = null; 
            _baseMesh = new Mesh();
            _linkMesh = new Mesh();
            _posedMeshes = new List<Mesh>();

            for (int i = 0; i < baseMeshes.Count; i++) { _baseMesh.Append(baseMeshes[i]); }
            for (int i = 0; i < linkMeshes.Count; i++) { _linkMesh.Append(linkMeshes[i]); }

            Initialize();
        }

        /// <summary>
        /// Creates a new external linear axis by duplicating an existing axis.
        /// This creates a deep copy of the existing axis.
        /// </summary>
        /// <param name="externalLinearAxis"> The external linear axis that should be duplicated. </param>
        /// <param name="duplicateMesh"> A boolean that indicates if the mesh should be duplicated. </param>
        public ExternalLinearAxis(ExternalLinearAxis externalLinearAxis, bool duplicateMesh = true)
        {
            _name = externalLinearAxis.Name;
            _axisPlane = new Plane(externalLinearAxis.AxisPlane);
            _attachmentPlane = new Plane(externalLinearAxis.AttachmentPlane);
            _axisLimits = new Interval(externalLinearAxis.AxisLimits);
            _axisNumber = externalLinearAxis.AxisNumber;
            _axisCurve = externalLinearAxis.AxisCurve;

            if (duplicateMesh == true)
            {
                _baseMesh = externalLinearAxis.BaseMesh.DuplicateMesh();
                _linkMesh = externalLinearAxis.LinkMesh.DuplicateMesh();
                _posedMeshes = externalLinearAxis.PosedMeshes.ConvertAll(mesh => mesh.DuplicateMesh());
            }
            else
            {
                _baseMesh = new Mesh();
                _linkMesh = new Mesh();
                _posedMeshes = new List<Mesh>();
            }
        }

        /// <summary>
        /// A method to duplicate the ExternalLinearAxis object. 
        /// </summary>
        /// <returns> Returns a deep copy for the ExternalLinearAxis object. </returns>
        public ExternalLinearAxis Duplicate()
        {
            return new ExternalLinearAxis(this);
        }

        /// <summary>
        /// A method to duplicate the ExternalLinearAxis object without duplicating the mesh. It will set an empty mesh. 
        /// </summary>
        /// <returns> Returns a deep copy for the ExternalLinearAxis object without a mesh. </returns>
        public ExternalLinearAxis DuplicateWithoutMesh()
        {
            return new ExternalLinearAxis(this, false);
        }

        /// <summary>
        /// A method to duplicate the ExternalLinearAxis object to an ExternalAxis object. 
        /// </summary>
        /// <returns> Returns a deep copy of the ExternalLinearAxis object as an ExternalAxis object. </returns>
        public override ExternalAxis DuplicateExternalAxis()
        {
            return new ExternalLinearAxis(this) as ExternalAxis;
        }

        /// <summary>
        /// A method to duplicate the ExternalLinearAxis object to an ExternalAxis object without the mesh. 
        /// </summary>
        /// <returns> Returns a deep copy of the ExternalLinearAxis object as an ExternalAxis object with empty meshes. </returns>
        public override ExternalAxis DuplicateExternalAxisWithoutMesh()
        {
            return new ExternalLinearAxis(this, false) as ExternalAxis;
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!this.IsValid)
            {
                return "Invalid External Linear Axis";
            }
            else
            {
                return "External Linear Axis (" + this.Name + ")";
            }
        }

        /// <summary>
        /// Defines the axis curve based on the axis limits, the momvement direction and the attachment plane origin.
        /// </summary>
        public void GetAxisCurve()
        {
            Line line = new Line(_attachmentPlane.Origin + _axisPlane.ZAxis * _axisLimits.Min, _attachmentPlane.Origin + _axisPlane.ZAxis * _axisLimits.Max);
            _axisCurve = line.ToNurbsCurve();
            _axisCurve.Domain = new Interval(0, 1);
        }

        /// <summary>
        /// Calculates the position of the attachment plane for a defined external axis value.
        /// This method does not take into account the axis limits. 
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the position of the attachment plane for. </param>
        /// <param name="inLimits"> A boolean that indicates if the defined exernal axis value is inside its limits. </param>
        /// <returns> The posed attachement plane. </returns>
        public override Plane CalculatePosition(double axisValue, out bool inLimits)
        {
            Transform translateNow = CalculateTransformationMatrix(axisValue, out bool isInLimits);
            Plane positionPlane = new Plane(AttachmentPlane);
            positionPlane.Transform(translateNow);

            inLimits = isInLimits;
            return positionPlane;
        }

        /// <summary>
        /// Calculates the the transformation matrix for a defined external axis value. 
        /// This method does not take into account the axis limits. 
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the position of the attachment plane for in mm. </param>
        /// <param name="inLimits"> A boolean that indicates if the defined exernal axis value is inside its limits. </param>
        /// <returns> The transformation matrix </returns>
        public override Transform CalculateTransformationMatrix(double axisValue, out bool inLimits)
        {
            bool isInLimits;

            if (axisValue < AxisLimits.Min)
            {
                isInLimits = false;
            }
            else if (axisValue > AxisLimits.Max)
            {
                isInLimits = false;
            }
            else
            {
                isInLimits = true;
            }

            Transform transform = Rhino.Geometry.Transform.Translation(_axisPlane.ZAxis * axisValue);
            inLimits = isInLimits;

            return transform;
        }

        /// <summary>
        /// Calculates the position of the attachment plane for a defined external axis value.
        /// This method takes into account the external axis limits. If the defined external
        /// axis value is outside its limits the closest external axis limit will be used. 
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the position of the attachment plane for in mm. </param>
        /// <returns> The posed attachement plane. </returns>
        public override Plane CalculatePositionSave(double axisValue)
        {
            Transform translateNow = CalculateTransformationMatrixSave(axisValue);
            Plane positionPlane = new Plane(AttachmentPlane);
            positionPlane.Transform(translateNow);

            return positionPlane;
        }

        /// <summary>
        /// Calculates the the transformation matrix for a defined external axis value. 
        /// This method takes into account the external axis limits. If the defined external
        /// axis value is outside its limits the closest external axis limit will be used. 
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the transformation matrix for in mm. </param>
        /// <returns> Returns the transformation matrix. </returns>
        public override Transform CalculateTransformationMatrixSave(double axisValue)
        {
            double value;

            if (axisValue < _axisLimits.Min)
            {
                value = _axisLimits.Min;
            }
            else if (axisValue > _axisLimits.Max)
            {
                value = _axisLimits.Max;
            }
            else
            {
                value = axisValue;
            }

            Transform transform = Rhino.Geometry.Transform.Translation(_axisPlane.ZAxis * value);

            return transform;
        }

        /// <summary>
        /// Calculates the position of the external axis mesh for a defined external axis value.
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the position of the meshes for in mm. </param>
        public override void PoseMeshes(double axisValue)
        {
            _posedMeshes.Clear();
            _posedMeshes.Add(_baseMesh.DuplicateMesh());
            _posedMeshes.Add(_linkMesh.DuplicateMesh());

            Transform translateNow = CalculateTransformationMatrix(axisValue, out bool isInLimits);
            _posedMeshes[1].Transform(translateNow);
        }

        /// <summary>
        /// A method that calls all the other methods that are needed to initialize the data that is needed to construct a valid external linear axis object. 
        /// </summary>
        private void Initialize()
        {
            GetAxisCurve();
        }

        /// <summary>
        /// A method that can be called to reinitialize all the data that is needed to construct a valid external linear axis. 
        /// </summary>
        public override void ReInitialize()
        {
            Initialize();
            _posedMeshes.Clear();
        }

        /// <summary>
        /// Transforms the external linear axis spatial properties (planes and meshes. 
        /// </summary>
        /// <param name="xform"> Spatial deform. </param>
        public override void Transform(Transform xform)
        {
            _attachmentPlane.Transform(xform);
            _axisPlane.Transform(xform);
            _baseMesh.Transform(xform);
            _linkMesh.Transform(xform);

            for (int i = 0; i < _posedMeshes.Count; i++)
            {
                _posedMeshes[i].Transform(xform);
            }

            GetAxisCurve();
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the External Linear Axis object is valid. 
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (AttachmentPlane == null) { return false; }
                if (AttachmentPlane == Plane.Unset) { return false; }
                if (AxisPlane == null) { return false; }
                if (AxisPlane == Plane.Unset) { return false; }
                if (AxisLimits == null) { return false; }
                return true;
            }
        }

        /// <summary>
        /// The name of the external axis
        /// </summary>
        public override string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The attachment plane of the axis. 
        /// </summary>
        public override Plane AttachmentPlane 
        {
            get 
            { 
                return _attachmentPlane; 
            }
            set 
            { 
                _attachmentPlane = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// The axis plane. The z-axis of the place defines the positive movement direction of the plane. 
        /// </summary>
        public override Plane AxisPlane 
        { 
            get 
            { 
                return _axisPlane; 
            }
            set 
            { 
                _axisPlane = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// The movement limits of the linear axis in meters. 
        /// </summary>
        public override Interval AxisLimits 
        { 
            get 
            { 
                return _axisLimits; 
            }
            set 
            {
                _axisLimits = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// The logic number of the external axis. 
        /// </summary>
        public override int? AxisNumber 
        { 
            get { return _axisNumber; }
            set { _axisNumber = value; }
        }

        /// <summary>
        /// The axis movement type.
        /// </summary>
        public override AxisType AxisType 
        { 
            get { return AxisType.LINEAR; }
        }

        /// <summary>
        /// The fixed base mesh of the external axis. 
        /// </summary>
        public override Mesh BaseMesh 
        { 
            get 
            { 
                return _baseMesh; 
            }
            set 
            { 
                _baseMesh = value;
                _posedMeshes = new List<Mesh>();
            }
        }

        /// <summary>
        /// The movable link mesh of the external axis posed for external axis value 0. 
        /// </summary>
        public override Mesh LinkMesh 
        { 
            get 
            { 
                return _linkMesh; 
            }
            set 
            { 
                _linkMesh = value;
                _posedMeshes = new List<Mesh>();
            }
        }

        /// <summary>
        /// The external linear axis curve. Defines the movement direction and the spatial
        /// limits of the attachement plane.
        /// </summary>
        public Curve AxisCurve 
        { 
            get { return _axisCurve; }
        }

        /// <summary>
        /// The external axis mesh posed in a certain external axis value.
        /// </summary>
        public override List<Mesh> PosedMeshes 
        { 
            get { return _posedMeshes; }
        }
        #endregion
    }
}
