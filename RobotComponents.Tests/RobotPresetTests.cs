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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
// Xunit libs
using Xunit;
// Robot Components Libs
using RobotComponents.ABB.Presets.Enumerations;
using RobotComponents.ABB.Presets.Robots;

namespace RobotComponents.Tests
{
    public class RobotPresetTests
    {
        [Fact]
        public void EveryEnumHasCorrespondingClass()
        {
            List<RobotPreset> robotPresets = Enum.GetValues(typeof(RobotPreset))
                .Cast<RobotPreset>()
                .Where(p => p != RobotPreset.EMPTY) // Skip EMPTY
                .ToList();

            List<string> missing = new List<string>();
            List<string> wrongType = new List<string>();

            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < robotPresets.Count; i++)
            {
                string typeName = $"RobotComponents.ABB.Presets.Robots.{robotPresets[i]}";
                Type found = null;

                try
                {
                    Assembly asm = Assembly.Load("RobotComponents.ABB.Presets");
                    found = asm.GetType(typeName, false);
                }
                catch
                {
                    // ignore if assembly can't load
                }

                if (found == null)
                {
                    missing.Add(typeName);
                }
                else
                {
                    // Check that it is assignable to RobotPresetData
                    if (!typeof(RobotPresetData).IsAssignableFrom(found))
                    {
                        wrongType.Add(typeName);
                    }
                }
            }

            Assert.True(missing.Count == 0 && wrongType.Count == 0,
                "Preset class check failed.\n" +
                (missing.Count > 0 ? "Missing Robot Preset Data classes:\n" + string.Join("\n", missing) + "\n" : string.Empty) +
                (wrongType.Count > 0 ? "Wrong type (should inherit from RobotPresetData):\n" + string.Join("\n", wrongType) + "\n" : string.Empty));
        }
    }
}