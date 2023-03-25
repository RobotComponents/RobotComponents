// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Actions.Declarations;

namespace RobotComponents.ABB.Definitions
{
    /// <summary>
    /// Represent the interface of mechanical units
    /// </summary>
    public interface IMechanicalUnit
    {
        #region fields

        #endregion

        #region constructors
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
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> 
        /// A string that represents the current object.
        /// </returns>
        string ToString();

        /// <summary>
        /// Calculates and returns the position of the meshes for a given Joint Target.
        /// </summary>
        /// <param name="jointTarget"> The Joint Target. </param>
        /// <returns> 
        /// The posed meshes. 
        /// </returns>
        List<Mesh> PoseMeshes(JointTarget jointTarget);
       
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
        /// Gets the number of axes for the mechanical unit.
        /// </summary>
        int NumberOfAxes { get; }
        #endregion
    }
}
