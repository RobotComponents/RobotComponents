// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2023 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2023)
//
// For license details, see the LICENSE file in the project root.

// Rhino Libs
using Rhino.Geometry;

namespace RobotComponents.Utils
{
    /// <summary>
    /// Represents mesh preperation methods. 
    /// </summary>
    public static class MeshPreperation
    {
        /// <summary>
        /// Returns a repaired mesh. Makes a duplicate of the orignal mesh. 
        /// Heals the naked edges, culls degenerate faces and unifies the normals.
        /// <remarks>
        /// Used for repairing robot and external axes meshes. 
        /// </remarks>
        /// </summary>
        /// <param name="mesh"> The mesh to repair. </param>
        /// <param name="tolerance"> The tolerance for the naked edges. </param>
        /// <param name="duplicate"> Indicates whether or not, the mesh should be duplicated first. </param>
        /// <returns> The repaired mesh. </returns>
        public static Mesh Repair(Mesh mesh, double tolerance = 0.25, bool duplicate = false)
        {
            Mesh result;

            if (duplicate == true)
            {
                result = mesh.DuplicateMesh();
            }
            else
            {
                result = mesh;
            }

            //result.UnifyNormals(); // <-- decreases the display quality
            result.Normals.ComputeNormals();
            result.FaceNormals.ComputeFaceNormals();
            result.ExtractNonManifoldEdges(true);
            result.HealNakedEdges(tolerance);
            result.Faces.ExtractDuplicateFaces();
            result.Faces.CullDegenerateFaces();
            result.Vertices.CullUnused();
            //result.UnifyNormals(); <-- decreases the display quality
            result.Normals.ComputeNormals();
            result.FaceNormals.ComputeFaceNormals();
            result.Compact();

            return result;
        }

        /// <summary>
        /// Returns a reduced mesh.
        /// </summary>
        /// <remarks>
        /// Used for constructing robot and external axis meshes.
        /// </remarks>
        /// <param name="mesh"> The mesh. </param>
        /// <param name="count"> The desired polygon count. </param>
        /// <returns> The reduced mesh. </returns>
        public static Mesh Reduce(Mesh mesh, int count = 5000)
        {
            Mesh result = mesh.DuplicateMesh();

            result.Reduce(count, true, 10, false);

            return result;
        }
    }
}
