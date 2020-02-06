using Rhino.Geometry;
using System.Collections.Generic;

namespace RobotComponents.BaseClasses.Definitions
{
    /// <summary>
    /// Defines if the axis moves linear or rotational
    /// </summary>
    public enum AxisType { LINEAR, ROTATIONAL };

    /// <summary>
    /// External axis class, main Class for external axis.
    /// An external axis (linear and rotational) can be attached to a robot or a workobject
    /// </summary>
    public abstract class ExternalAxis
    {
        #region fields

        #endregion

        #region constructors
        /// <summary>
        /// A method to duplicate the object as an ExternalAxis type
        /// </summary>
        /// <returns> Returns a deep copy of the ExternalAxis object. </returns>
        public abstract ExternalAxis DuplicateExternalAxis();
        #endregion

        #region methods
        /// <summary>
        /// Calculates the position of the attachment plane for a defined external axis value.
        /// This method does not take into account the axis limits. 
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the position of the attachment plane for. </param>
        /// <param name="inLimits"> A boolean that indicates if the defined exernal axis value is inside its limits. </param>
        /// <returns> The posed attachement plane. </returns>
        public abstract Plane CalculatePosition(double axisValue, out bool inLimits);

        /// <summary>
        /// Calculates the position of the attachment plane for a defined external axis value.
        /// This method takes into account the external axis limits. If the defined external
        /// axis value is outside its limits the closest external axis limit will be used. 
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the position of the attachment plane for. </param>
        /// <returns> The posed attachement plane. </returns>
        public abstract Plane CalculatePositionSave(double axisValue);

        /// <summary>
        /// A method that can be called to reinitialize all the data that is needed to construct a valid external axis. 
        /// </summary>
        public abstract void ReInitialize();

        /// <summary>
        /// Calculates the position of the external axis mesh for a defined external axis value.
        /// </summary>
        /// <param name="axisValue"> The external axis value to calculate the position of the meshes for. </param>
        public abstract void PoseMeshes(double axisValue);

        /// <summary>
        /// Transforms the linear axis spatial properties (planes and meshes). 
        /// </summary>
        /// <param name="xform"> Spatial deform. </param>
        public abstract void Transform(Transform xform);
        #endregion

        #region properties
        /// <summary>
        /// Boolearn that indicates if the External Axis instance is valid.
        /// </summary>
        public abstract bool IsValid { get; }

        /// <summary>
        /// The name of the external axis
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Defines the axis limits as an interval
        /// </summary>
        public abstract Interval AxisLimits { get; set; }

        /// <summary>
        /// Defines the axis type (linear or rotational)
        /// </summary>
        public abstract AxisType AxisType { get; }

        /// <summary>
        /// Defines the plane where the robot or the work object is attached. 
        /// </summary>
        public abstract Plane AttachmentPlane { get; set; }

        /// <summary>
        /// Defines the axis plane. In case of a rotational axis the z-axis of the plane
        /// defines the rotation center. In case of linear axis the z-axis of the plane defines 
        /// the movement direction.
        /// </summary>
        public abstract Plane AxisPlane { get; set; }

        /// <summary>
        /// The axis logic as number (0, 1, 2, 3, 4 or 5)
        /// </summary>
        public abstract int? AxisNumber { get; set; } //TODO: changes int? to int after implementation

        /// <summary>
        /// The fixed base mesh of the external axis. 
        /// </summary>
        public abstract Mesh BaseMesh { get; set; }

        /// <summary>
        /// The movable link mesh of the external axis posed for external axis value 0. 
        /// </summary>
        public abstract Mesh LinkMesh { get; set; }

        /// <summary>
        /// The external axis mesh posed in a certain external axis value.
        /// </summary>
        public abstract List<Mesh> PosedMeshes { get; }
        #endregion

    }
}