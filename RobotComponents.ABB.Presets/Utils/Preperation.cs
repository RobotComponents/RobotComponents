// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
// Eto Libs
using Eto.Forms;
// Rhino Libs
using Rhino;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Presets.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static RobotComponents.ABB.Presets.Utils.HelperMethods;
using static RobotComponents.Utils.MeshPreperation;

namespace RobotComponents.ABB.Presets.Utils
{
    /// <summary>
    /// Represents methods to prepare the robot meshes. 
    /// </summary>
    public static class Preperation
    {
        #region fields
        private static readonly MeshingParameters _meshingParameters = new MeshingParameters
        {
            SimplePlanes = true,
            MinimumEdgeLength = 1
        };

        private static readonly List<string> _layerNamesRobotLinks = new List<string>()
        {
            "link_0",
            "link_1",
            "link_2",
            "link_3",
            "link_4",
            "link_5",
            "link_6",
        };

        private static readonly List<string> _layerNamesRobotAttributes = new List<string>()
        {
            "balancing_device_rear_base_body",
            "balancing_device_rear_base_rod",
            "balancing_device_upper_arm_body",
            "balancing_device_upper_arm_rod",
            "parallel_rod_counter_weight",
            "parallel_rod_rod",
        };
        #endregion

        #region methods
        /// <summary>
        /// Gets and write the robot link meshes.
        /// </summary>
        /// <param name="numberOfElements"> The maximum number of elements per mesh. </param>
        /// <returns> The list with meshes. </returns>
        [Obsolete("Warning: Use this method carefully. It is intended for developer use only and may change without notice.", false)]
        public static List<Mesh> WriteRobotMeshes(List<int> numberOfElements)
        {
            return WriteMeshes(numberOfElements, _layerNamesRobotLinks);
        }

        /// <summary>
        /// Gets and write the robot attribute meshes.
        /// </summary>
        /// <param name="numberOfElements"> The maximum number of elements per mesh. </param>
        /// <returns> The list with meshes. </returns>
        [Obsolete("Warning: Use this method carefully. It is intended for developer use only and may change without notice.", false)]
        public static List<Mesh> WriteRobotAttributeMeshes(List<int> numberOfElements)
        {
            return WriteMeshes(numberOfElements, _layerNamesRobotAttributes);
        }

        /// <summary>
        /// Imports STEP files for robot links, organizing them into formatted layers.
        /// </summary>
        [Obsolete("Warning: Use this method carefully. It is intended for developer use only and may change without notice.", false)]
        public static void ImportStepsRobotLinks()
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            List<string> labels = new List<string>();

            for (int i = 0; i < _layerNamesRobotLinks.Count; i++)
            {
                string name = _layerNamesRobotLinks[i];
                string formatted = FormatLayerName(name);
                labels.Add(formatted);
            }

            ImportStepAndOrganizeMulti(doc, labels, _layerNamesRobotLinks);
        }

        /// <summary>
        /// Imports STEP files for robot attributes, organizing them into formatted layers.
        /// </summary>
        [Obsolete("Warning: Use this method carefully. It is intended for developer use only and may change without notice.", false)]
        public static void ImportStepsRobotAttributes()
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;

            List<string> labels = new List<string>();

            for (int i = 0; i < _layerNamesRobotAttributes.Count; i++)
            {
                string name = _layerNamesRobotAttributes[i];
                string formatted = FormatLayerName(name);
                labels.Add(formatted);
            }

            ImportStepAndOrganizeMulti(doc, labels, _layerNamesRobotAttributes);
        }

        /// <summary>
        /// Gets and write the meshes.
        /// </summary>
        /// <param name="numberOfElements"> The maximum number of elements per mesh. </param>
        /// <param name="layerNames"> The name of the Rhino layer to create the mesh from. </param>
        /// <returns> The list with meshes. </returns>
        private static List<Mesh> WriteMeshes(List<int> numberOfElements, List<string> layerNames)
        {
            // Get active Rhino doc
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            string directory = Path.GetDirectoryName(doc.Path);

            // Get base name for files
            string baseName = Path.GetFileNameWithoutExtension(doc.Name).Replace('-', '_');

            // List meshes
            List<Mesh> meshes = new List<Mesh>();


            // Get and write robot meshes
            for (int i = 0; i < layerNames.Count; i++)
            {
                // Get breps
                List<Brep> breps = GetBrepsFromLayer(doc, layerNames[i]);

                // Make mesh
                Mesh mesh = new Mesh();

                for (int j = 0; j < breps.Count; j++)
                {
                    mesh.Append(Mesh.CreateFromBrep(breps[j], _meshingParameters));
                }

                // Repair, reduce and write meshes
                if (mesh.Faces.Count != 0)
                {
                    // Reduce and repair
                    mesh = Repair(mesh);
                    mesh = Repair(mesh);
                    mesh = Repair(mesh);
                    mesh = Reduce(mesh, numberOfElements[i < numberOfElements.Count ? i : numberOfElements.Count - 1]);
                    mesh = Repair(mesh);
                    mesh = Repair(mesh);
                    mesh = Repair(mesh);

                    // Serialize the mesh to a JSON string
                    string serializedMesh = mesh.ToJSON(null);

                    // Write to file
                    string filePath = Path.Combine(directory, $"{baseName}_{layerNames[i]}.txt");
                    File.WriteAllText(filePath, serializedMesh);
                }

                // Add to list
                meshes.Add(mesh);
            }

            return meshes;
        }

        /// <summary>
        /// Formats a layer name by replacing underscores with spaces and capitalizing the first letter.
        /// </summary>
        private static string FormatLayerName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            string replaced = input.Replace("_", " ");

            return replaced;
        }

        /// <summary>
        /// Imports multiple STEP files grouped by labels, assigns each imported object's geometry
        /// to the corresponding layers specified by <paramref name="layerNames"/>, and organizes
        /// the objects within the Rhino document. Explodes any block instances to individual objects
        /// and purges unused definitions after import.
        /// </summary>
        /// <param name="doc">The active Rhino document where the STEP files will be imported.</param>
        /// <param name="labels">A list of labels used to prompt the user for STEP file selections.</param>
        /// <param name="layerNames">A list of layer names that correspond to each label and to which imported objects will be assigned.</param>
        private static void ImportStepAndOrganizeMulti(RhinoDoc doc, List<string> labels, List<string> layerNames)
        {
            List<Tuple<string, int>> allFiles = new List<Tuple<string, int>>();

            // Step 1: Collect file paths
            for (int i = 0; i < labels.Count; i++)
            {
                string label = labels[i];

                OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = $"Select STEP files for {label}",
                    MultiSelect = true,
                    Filters = {new FileFilter("STEP Files", ".step", ".stp")
                    }
                };

                DialogResult result = ofd.ShowDialog(null);
                List<string> fileNames = ofd.Filenames.ToList();

                if (result != DialogResult.Ok || fileNames.Count == 0)
                {
                    RhinoApp.WriteLine("No files selected for: " + label);
                    continue;
                }

                for (int j = 0; j < fileNames.Count; j++)
                {
                    allFiles.Add(new Tuple<string, int>(fileNames[j], i));
                }
            }

            if (allFiles.Count == 0)
            {
                RhinoApp.WriteLine("No files selected. Aborting.");
                return;
            }

            // Step 2: Import and organize
            for (int i = 0; i < allFiles.Count; i++)
            {
                Tuple<string, int> fileEntry = allFiles[i];
                string filePath = fileEntry.Item1;
                string layerName = layerNames[fileEntry.Item2];

                // Create layer if it doesn't exist
                if (doc.Layers.FindName(layerName) == null)
                {
                    doc.Layers.Add(layerName, System.Drawing.Color.Black);
                }

                // Get layer index safely
                int layerIndexId = doc.Layers.FindByFullPath(layerName, -1);

                if (layerIndexId == -1)
                {
                    Layer layer = doc.Layers.FindName(layerName);

                    if (layer != null)
                    {
                        layerIndexId = layer.Index;
                    }
                    else
                    {
                        layerIndexId = doc.Layers.CurrentLayerIndex; // fallback
                    }
                }

                // Set as current layer
                doc.Layers.SetCurrentLayerIndex(layerIndexId, true);

                // Capture existing object IDs before import
                List<Guid> before = new List<Guid>();
                List<RhinoObject> existingObjects = new List<RhinoObject>(doc.Objects.GetObjectList(ObjectType.AnyObject));

                for (int j = 0; j < existingObjects.Count; j++)
                {
                    before.Add(existingObjects[j].Id);
                }

                // Import STEP file
                string command = "-_Import \"" + filePath + "\" _Enter";
                RhinoApp.RunScript(command, false);

                // Give Rhino time to complete the import
                RhinoApp.Wait();

                // Get imported objects
                List<Guid> imported = GetLastImportedObjects(doc, before);

                if (imported == null || imported.Count == 0)
                {
                    continue;
                }

                // Explode nested blocks
                List<Guid> exploded = ExplodeAllBlocks(imported, doc);

                // Move all exploded objects to correct layer
                for (int j = 0; j < exploded.Count; j++)
                {
                    Guid objId = exploded[j];
                    RhinoObject rhinoObj = doc.Objects.FindId(objId);

                    if (rhinoObj != null)
                    {
                        ObjectAttributes attributes = rhinoObj.Attributes.Duplicate();
                        attributes.LayerIndex = layerIndexId;
                        doc.Objects.ModifyAttributes(rhinoObj, attributes, true);
                    }
                }

                doc.Views.Redraw();
            }

            // Step 3: Purge unused definitions (run multiple times to be thorough)
            RhinoApp.RunScript("_-Purge _All _Yes _Enter", false);
            RhinoApp.RunScript("_-Purge _All _Yes _Enter", false);
            RhinoApp.RunScript("_-Purge _All _Yes _Enter", false);

            // Keep default layer
            if (doc.Layers.FindName("Default") == null)
            {
                doc.Layers.Add("Default", System.Drawing.Color.Black);
            }

            // Set as current layer
            int layerIndex = doc.Layers.FindByFullPath("Default", -1);

            if (layerIndex != -1)
            {
                doc.Layers.SetCurrentLayerIndex(layerIndex, true);
            }
        }

        /// <summary>
        /// Captures new objects added to the document after importing.
        /// </summary>
        /// <param name="doc"> The active Rhino document. </param>
        /// <param name="before"> Object IDs before import .</param>
        /// <returns>List of newly added object IDs.</returns>
        private static List<Guid> GetLastImportedObjects(RhinoDoc doc, List<Guid> before)
        {
            // Capture all objects after import
            List<Guid> after = new List<Guid>();
            List<RhinoObject> allObjects = new List<RhinoObject>(doc.Objects.GetObjectList(ObjectType.AnyObject));

            for (int i = 0; i < allObjects.Count; i++)
            {
                after.Add(allObjects[i].Id);
            }

            // Find the difference
            List<Guid> newObjects = new List<Guid>();

            for (int i = 0; i < after.Count; i++)
            {
                if (!before.Contains(after[i]))
                {
                    newObjects.Add(after[i]);
                }
            }

            return newObjects;
        }

        /// <summary>
        /// Recursively explodes all block instances in the given objects and returns the resulting geometry object IDs.
        /// </summary>
        /// <param name="objectIds">Initial objects to process (may include block instances).</param>
        /// <param name="doc">Active Rhino document.</param>
        /// <returns>List of exploded object IDs in world space.</returns>
        private static List<Guid> ExplodeAllBlocks(IEnumerable<Guid> objectIds, RhinoDoc doc)
        {
            List<Guid> exploded = new List<Guid>();
            Queue<Guid> queue = new Queue<Guid>();

            // Initialize queue
            IEnumerator<Guid> enumerator = objectIds.GetEnumerator();

            while (enumerator.MoveNext())
            {
                queue.Enqueue(enumerator.Current);
            }

            while (queue.Count > 0)
            {
                Guid id = queue.Dequeue();
                RhinoObject rhinoObj = doc.Objects.FindId(id);

                if (rhinoObj is InstanceObject instance)
                {
                    InstanceDefinition definition = instance.InstanceDefinition;

                    if (definition == null)
                    {
                        continue;
                    }

                    RhinoObject[] defObjects = definition.GetObjects();

                    if (defObjects == null)
                    {
                        continue;
                    }

                    Transform xform = instance.InstanceXform;
                    List<Guid> newIds = new List<Guid>();

                    for (int i = 0; i < defObjects.Length; i++)
                    {
                        RhinoObject defObj = defObjects[i];
                        GeometryBase geom = defObj.Geometry.Duplicate();
                        geom.Transform(xform);

                        ObjectAttributes attributes = defObj.Attributes.Duplicate();
                        Guid newId = doc.Objects.Add(geom, attributes);

                        if (newId != Guid.Empty)
                        {
                            queue.Enqueue(newId);
                            newIds.Add(newId);
                        }
                    }

                    doc.Objects.Delete(id, true);

                    for (int i = 0; i < newIds.Count; i++)
                    {
                        exploded.Add(newIds[i]);
                    }
                }
                else
                {
                    exploded.Add(id);
                }
            }

            return exploded;
        }

        /// <summary>
        /// Retrieves all BRep geometries from a specified layer in the active Rhino document.
        /// Includes BReps and optionally converts extrusions on the layer to BReps as well.
        /// </summary>
        /// <param name="doc"> The active Rhino document to search. </param>
        /// <param name="layerName"> The name of the layer from which to collect BRep geometry. </param>
        /// <returns> A list of BReps found on the specified layer. </returns>

        private static List<Brep> GetBrepsFromLayer(RhinoDoc doc, string layerName)
        {
            List<Brep> breps = new List<Brep>();

            // Get the layer index from its name
            int layerIndex = doc.Layers.FindByFullPath(layerName, -1);

            if (layerIndex < 0)
            {
                return breps;
            }

            // Get all objects on the layer
            ObjectTable objectTable = doc.Objects;

            foreach (RhinoObject obj in objectTable.FindByLayer(doc.Layers[layerIndex]))
            {
                if (obj.Geometry is Brep brep)
                {
                    breps.Add(brep.DuplicateBrep());
                }
                else if (obj.Geometry is Extrusion extrusion)
                {
                    // Optionally convert extrusions to breps
                    Brep brepFromExtrusion = extrusion.ToBrep();

                    if (brepFromExtrusion != null)
                    {
                        breps.Add(brepFromExtrusion);
                    }
                }
            }

            return breps;
        }

        /// <summary>
        /// Converts all current Robot Preset meshes to JSON and writes them to .txt files.
        /// </summary>
        /// <param name="directory"> Directory where to save the JSON files with serialized meshes. </param>
        [Obsolete("Warning: Use this method carefully. It is intended for developer use only and may change without notice.", false)]
        public static void ConvertCurrentRobotPresetMeshesToJSON(string directory)
        {
            // Get presets
            foreach (RobotPreset preset in Enum.GetValues(typeof(RobotPreset)))
            {
                if (preset != RobotPreset.EMPTY)
                {
                    Robot robot = Factory.GetRobotPreset(preset, Plane.WorldXY);
                    string baseName = preset.ToString();

                    for (int i = 0; i < 7; i++)
                    {
                        // Serialize to JSON
                        Mesh mesh = robot.Meshes[i];
                        string json = mesh.ToJSON(null);

                        // Write to file
                        string filePath = Path.Combine(directory, $"{baseName}_link_{i}.txt");
                        File.WriteAllText(filePath, json);
                    }
                }
            }
        }

        /// <summary>
        /// Generates the robot preset code lines. 
        /// </summary>
        /// <param name="robot"> The robot to write the codelines for. </param>
        /// <param name="className"> The class name. </param>
        /// <returns> The template as a list with code lines. </returns>
        [Obsolete("Warning: Use this method carefully. It is intended for developer use only and may change without notice.", false)]
        public static List<string> GenerateRobotPresetCodeLines(Robot robot, out string className)
        {
            className = GetRobotClassNameFromName(robot.Name);

            List<string> lines = new List<string>
            {
                "// SPDX-License-Identifier: GPL-3.0-or-later",
                "// This file is part of Robot Components",
                "// Project: https://github.com/RobotComponents/RobotComponents",
                "//",
               $"// Copyright (c) {DateTime.Now.Year} Arjen Deetman",
                "//",
                "// Authors:",
               $"//   - Arjen Deetman ({DateTime.Now.Year})",
                "//",
                "// For license details, see the LICENSE file in the project root.",
                "",
                "// System Libs",
                "using System.Collections.Generic;",
                "// Rhino Libs",
                "using Rhino.Geometry;",
                "",
                "namespace RobotComponents.ABB.Presets.Robots",
                "{",
                "    /// <summary>",
               $"    /// Represent the robot data of the {robot.Name}.",
                "    /// </summary>",
               $"    public class {className} : RobotPresetData",
                "    {",
                "        #region properties",
                "        /// <summary>",
                "        /// Gets the name of the Robot.",
                "        /// </summary>",
                "        public override string Name",
                "        {",
               $"            get {{ return \"{robot.Name}\"; }}",
                "        }",
                "",
                "        /// <summary>",
                "        /// Gets the kinematics parameters.",
                "        /// </summary>",
               $"        public override double[] KinematicParameters => new double[] {{ {robot.A1}, {robot.A2}, {robot.A3}, {robot.B}, {robot.C1}, {robot.C2}, {robot.C3}, {robot.C4} }};",

                "",
                "        /// <summary>",
                "        /// Gets the axis limits.",
                "        /// </summary>",
                "        public override List<Interval> AxisLimits => new List<Interval>",
                "        {",
               $"            new Interval({robot.InternalAxisLimits[0].T0}, {robot.InternalAxisLimits[0].T1}),",
               $"            new Interval({robot.InternalAxisLimits[1].T0}, {robot.InternalAxisLimits[1].T1}),",
               $"            new Interval({robot.InternalAxisLimits[2].T0}, {robot.InternalAxisLimits[2].T1}),",
               $"            new Interval({robot.InternalAxisLimits[3].T0}, {robot.InternalAxisLimits[3].T1}),",
               $"            new Interval({robot.InternalAxisLimits[4].T0}, {robot.InternalAxisLimits[4].T1}),",
               $"            new Interval({robot.InternalAxisLimits[5].T0}, {robot.InternalAxisLimits[5].T1})",
                "        };",
                "",
                "        /// <summary>",
                "        /// Gets the name of the Mesh resources embedded in the assembly.",
                "        /// </summary>",
                "        public override string[] MeshResources => new[]",
                "        {",
               $"            \"{className}_{_layerNamesRobotLinks[0]}\",",
               $"            \"{className}_{_layerNamesRobotLinks[1]}\",",
               $"            \"{className}_{_layerNamesRobotLinks[2]}\",",
               $"            \"{className}_{_layerNamesRobotLinks[3]}\",",
               $"            \"{className}_{_layerNamesRobotLinks[4]}\",",
               $"            \"{className}_{_layerNamesRobotLinks[5]}\",",
               $"            \"{className}_{_layerNamesRobotLinks[6]}\"",
                "        };",
                "        #endregion",
                "    }",
                "}"
            };

            return lines;
        }

        /// <summary>
        /// Generates and writes the code lines of all existing robot presets.
        /// </summary>
        /// <param name="directory">The target directory path.</param>
        [Obsolete("Warning: Use this method carefully. It is intended for developer use only and may change without notice.", false)]
        public static void WriteCodeLinesOfAllExistingRobotPresets(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentException("Directory cannot be null or empty.", nameof(directory));
            }

            List<RobotPreset> robotPresets = Enum.GetValues(typeof(RobotPreset))
                                     .Cast<RobotPreset>()
                                     .Where(p => p != RobotPreset.EMPTY) // Skip EMPTY
                                     .ToList();

            for (int i = 0; i < robotPresets.Count; i++)
            {
                RobotPreset preset = robotPresets[i];
                Robot robot = Factory.GetRobotPreset(preset, Plane.WorldXY, new RobotTool(), new List<IExternalAxis>() { });
                List<string> codeLines = GenerateRobotPresetCodeLines(robot, out string className);
                WriteCodeToFile(codeLines, className, directory);
            }

        }

        /// <summary>
        /// Writes generated robot preset code lines to a .cs file.
        /// </summary>
        /// <param name="codeLines">The code lines to write.</param>
        /// <param name="fileName">The file name without extension.</param>
        /// <param name="directory">The target directory path.</param>
        [Obsolete("Warning: Use this method carefully. It is intended for developer use only and may change without notice.", false)]
        public static void WriteCodeToFile(List<string> codeLines, string fileName, string directory)
        {
            if (codeLines == null || codeLines.Count == 0)
            {
                throw new ArgumentException("Code lines cannot be null or empty.", nameof(codeLines));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
            }

            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentException("Directory cannot be null or empty.", nameof(directory));
            }

            // Ensure directory exists
            Directory.CreateDirectory(directory);

            // Build full path with .cs extension
            string filePath = Path.Combine(directory, $"{fileName}.cs");

            // Write all lines
            File.WriteAllLines(filePath, codeLines);
        }
        #endregion
    }
}
