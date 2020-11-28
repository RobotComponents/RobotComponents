// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Enumerations;

namespace RobotComponents.Definitions
{
    /// <summary>
    /// Represents an abstract class for External Axes. 
    /// </summary>
    [Serializable()]
    public abstract class ExternalAxis
    {
        #region fields

        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the External Axis class. 
        /// </summary>
        public ExternalAxis()
        {
        }

        /// <summary>
        /// Returns an exact duplicate of this External Axis.
        /// </summary>
        /// <returns> A deep copy of the External Axis. </returns>
        public abstract ExternalAxis DuplicateExternalAxis();

        /// <summary>
        /// Returns an exact duplicate of this External Axis without meshes.
        /// </summary>
        /// <returns> A deep copy of the External Axis without meshes. </returns>
        public abstract ExternalAxis DuplicateExternalAxisWithoutMesh();
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
                return "Invalid External Axis";
            }
            else if (this is ExternalLinearAxis externalLinearAxis)
            {
                return externalLinearAxis.ToString();
            }
            else if (this is ExternalRotationalAxis externalRotationalAxis)
            {
                return externalRotationalAxis.ToString();
            }
            else
            {
                return "External Axis (" + this.Name + ")";
            }
        }

        /// <summary>
        /// Calculates the position of the attachment plane for a given External Joint Position.
        /// This calculation does not take into account the axis limits. 
        /// </summary>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <param name="inLimits"> Specifies whether the External Joint Position is inside its limits. </param>
        /// <returns> The posed attachement plane. </returns>
        public abstract Plane CalculatePosition(ExternalJointPosition externalJointPosition, out bool inLimits);

        /// <summary>
        /// Calculates the the transformation matrix for a given External Joint Position.
        /// This calculation does not take into account the axis limits. 
        /// </summary>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <param name="inLimits"> Specifies whether the External Joint Position is inside its limits. </param>
        /// <returns> The transformation matrix. </returns>
        public abstract Transform CalculateTransformationMatrix(ExternalJointPosition externalJointPosition, out bool inLimits);

        /// <summary>
        /// Calculates the position of the attachment plane for a given External Joint Position.
        /// This calculations takes into account the external axis limits. 
        /// If the defined External Joint Posiiton is outside its limits the closest valid external axis value will be used.  
        /// </summary>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <returns> The posed attachement plane. </returns>
        public abstract Plane CalculatePositionSave(ExternalJointPosition externalJointPosition);

        /// <summary>
        /// Calculates the the transformation matrix for a given External Joint Position.
        /// This calculations takes into account the external axis limits. 
        /// If the defined External Joint Posiiton is outside its limits the closest valid external axis value will be used. 
        /// </summary>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <returns> The transformation matrix. </returns>
        public abstract Transform CalculateTransformationMatrixSave(ExternalJointPosition externalJointPosition);

        /// <summary>
        /// Reinitializes the fields and properties to construct valid External Axis instance. 
        /// </summary>
        public abstract void ReInitialize();

        /// <summary>
        /// Calculates the position of the external axis meshes for a given External Joint Position.
        /// </summary>
        /// <param name="externalJointPosition"> The External Joint Position. </param>
        /// <returns> The posed meshes. </returns>
        public abstract List<Mesh> PoseMeshes(ExternalJointPosition externalJointPosition);

        /// <summary>
        /// Transforms the external axis spatial properties (planes and meshes). 
        /// </summary>
        /// <param name="xform"> The spatial deform. </param>
        public abstract void Transform(Transform xform);
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        public abstract bool IsValid { get; }

        /// <summary>
        /// Gets or sets the external axis name. 
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the axis limits.
        /// </summary>
        public abstract Interval AxisLimits { get; set; }

        /// <summary>
        /// Gets the Axis Type.
        /// </summary>
        public abstract AxisType AxisType { get; }

        /// <summary>
        /// Gets or sets the attachment plane to attach a robot or work object.
        /// </summary>
        public abstract Plane AttachmentPlane { get; set; }

        /// <summary>
        /// Gets or sets the axis plane.
        /// In case of a rotational axis the z-axis of the plane defines the rotation center. 
        /// In case of linear axis the z-axis of the plane defines the movement direction.
        /// </summary>
        public abstract Plane AxisPlane { get; set; }

        /// <summary>
        /// Gets or sets the axis logic as a number (-1, 0, 1, 2, 3, 4, 5).
        /// </summary>
        public abstract int AxisNumber { get; set; }

        /// <summary>
        /// Gets the axis logic as a char (-, A, B, C, E, E, F).
        /// </summary>
        public abstract char AxisLogic { get; set; }

        /// <summary>
        /// Gets or sets the fixed base mesh of the external axis. 
        /// </summary>
        public abstract Mesh BaseMesh { get; set; }

        /// <summary>
        /// Gets or sets the movable link mesh of the external axis posed for external axis value set to 0. 
        /// </summary>
        public abstract Mesh LinkMesh { get; set; }

        /// <summary>
        /// Gets latest calculated posed axis meshes.
        /// </summary>
        public abstract List<Mesh> PosedMeshes { get; }

        /// <summary>
        /// Gets a value indicating whether or not this External Axis moves the Robot.
        /// </summary>
        public abstract bool MovesRobot { get; set; }
        #endregion
    }
}