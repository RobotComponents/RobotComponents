// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Enumerations;

namespace RobotComponents.ABB.Definitions
{
    /// <summary>
    /// Represents the interface for External Axes. 
    /// </summary>
    public interface IExternalAxis
    {
        #region fields

        #endregion

        #region constructors
        /// <summary>
        /// Returns an exact duplicate of this External Axis.
        /// </summary>
        /// <returns> 
        /// A deep copy of the External Axis. 
        /// </returns>
        IExternalAxis DuplicateExternalAxis();

        /// <summary>
        /// Returns an exact duplicate of this External Axis without meshes.
        /// </summary>
        /// <returns> 
        /// A deep copy of the External Axis without meshes. 
        /// </returns>
        IExternalAxis DuplicateExternalAxisWithoutMesh();

        /// <summary>
        /// Returns an exact duplicate of this Mechanical Unit.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Mechanical Unit. 
        /// </returns>
        IMechanicalUnit DuplicateMechanicalUnit();

        /// <summary>
        /// Returns an exact duplicate of this Mechanical Unit without meshes.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Mechanical Unit without meshes. 
        /// </returns>
        IMechanicalUnit DuplicateMechanicalUnitWithoutMesh();
        #endregion

        #region methods
        /// <summary>
        /// Calculates the position of the attachment plane for a given External Joint Position.
        /// </summary>
        /// <remarks>
        /// This calculation does not take into account the axis limits. 
        /// </remarks>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <param name="isInLimits"> Specifies whether the External Joint Position is inside its limits. </param>
        /// <returns> 
        /// The posed attachement plane. 
        /// </returns>
        Plane CalculatePosition(ExternalJointPosition externalJointPosition, out bool isInLimits);

        /// <summary>
        /// Calculates the the transformation matrix for a given External Joint Position.. 
        /// </summary>
        /// <remarks>
        /// This calculation does not take into account the axis limits. 
        /// </remarks>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <param name="isInLimits"> Specifies whether the External Joint Position is inside its limits. </param>
        /// <returns> 
        /// The transformation matrix.
        /// </returns>
        Transform CalculateTransformationMatrix(ExternalJointPosition externalJointPosition, out bool isInLimits);

        /// <summary>
        /// Calculates the position of the attachment plane for a given External Joint Position..  
        /// </summary>
        /// <remarks>
        /// This calculations takes into account the external axis limits. 
        /// If the defined External Joint Posiiton is outside its limits the closest valid external axis value will be used. 
        /// </remarks>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <returns> 
        /// The posed attachement plane. 
        /// </returns>
        Plane CalculatePositionSave(ExternalJointPosition externalJointPosition);

        /// <summary>
        /// Calculates the the transformation matrix for a given External Joint Position.
        /// </summary>
        /// <remarks>
        /// This calculations takes into account the external axis limits. 
        /// If the defined External Joint Posiiton is outside its limits the closest valid external axis value will be used. 
        /// </remarks>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <returns> 
        /// The transformation matrix. 
        /// </returns>
        Transform CalculateTransformationMatrixSave(ExternalJointPosition externalJointPosition);

        /// <summary>
        /// Reinitializes the fields and properties to construct valid External Axis instance. 
        /// </summary>
        void ReInitialize();

        /// <summary>
        /// Calculates and returns the position of the meshes for a given Joint Target.
        /// </summary>
        /// <param name="jointTarget"> The Joint Target. </param>
        /// <returns> 
        /// The posed meshes. 
        /// </returns>
        List<Mesh> PoseMeshes(JointTarget jointTarget);

        /// <summary>
        /// Calculates the position of the external axis meshes for a given External Joint Position.
        /// </summary>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <returns> 
        /// The posed meshes. 
        /// </returns>
        List<Mesh> PoseMeshes(ExternalJointPosition externalJointPosition);

        /// <summary>
        /// Transforms the external axis spatial properties (planes and meshes). 
        /// </summary>
        /// <param name="xform"> The spatial deform. </param>
        void Transform(Transform xform);

        /// <summary>
        /// Returns the Bounding Box of the object.
        /// </summary>
        /// <param name="accurate"> If true, a physically accurate bounding box will be computed. If not, a bounding box estimate will be computed. </param>
        /// <returns> 
        /// The Bounding Box. 
        /// </returns>
        BoundingBox GetBoundingBox(bool accurate);
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets or sets the external axis name. 
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the axis limits.
        /// </summary>
        Interval AxisLimits { get; set; }

        /// <summary>
        /// Gets the Axis Type.
        /// </summary>
        AxisType AxisType { get; }

        /// <summary>
        /// Gets or sets the attachment plane to attach a robot or work object.
        /// </summary>
        Plane AttachmentPlane { get; set; }

        /// <summary>
        /// Gets or sets the axis plane.
        /// </summary>
        /// <remarks>
        /// In case of a rotational axis the z-axis of the plane defines the rotation center. 
        /// In case of linear axis the z-axis of the plane defines the movement direction.
        /// </remarks>
        Plane AxisPlane { get; set; }

        /// <summary>
        /// Gets or sets the axis logic as a number (-1, 0, 1, 2, 3, 4, 5).
        /// </summary>
        int AxisNumber { get; set; }

        /// <summary>
        /// Gets the axis logic as a char (-, A, B, C, E, E, F).
        /// </summary>
        char AxisLogic { get; set; }

        /// <summary>
        /// Gets or sets the fixed base mesh of the external axis. 
        /// </summary>
        Mesh BaseMesh { get; set; }

        /// <summary>
        /// Gets or sets the movable link mesh of the external axis posed for external axis value set to 0. 
        /// </summary>
        Mesh LinkMesh { get; set; }

        /// <summary>
        /// Gets latest calculated posed axis meshes.
        /// </summary>
        List<Mesh> PosedMeshes { get; }

        /// <summary>
        /// Gets a value indicating whether or not this External Axis moves the Robot.
        /// </summary>
        bool MovesRobot { get; set; }

        /// <summary>
        /// Gets the number of axes for the mechanical unit.
        /// </summary>
        int NumberOfAxes { get; }
        #endregion
    }
}