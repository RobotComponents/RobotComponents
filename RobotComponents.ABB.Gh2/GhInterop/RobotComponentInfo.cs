// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2022 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2022)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper2.Framework;
using Grasshopper2.UI;
using Grasshopper2.UI.Icon;

namespace RobotComponents.ABB.Gh2.GhInterop
{
    public class PluginInfo : Plugin
    {
        public static readonly Guid PluginId = new Guid("{60765982-3CAC-4DBC-B55E-E9E68E70381F}");
        public PluginInfo()
            : base(PluginId,
                  new Nomen("Robot Components ABB", "Intuitive robot programming."),
                  typeof(PluginInfo).Assembly.GetName().Version)
        {
            _icon = null;
        }

        private readonly IIcon _icon;

        public override string Author => "Robot Components";

        public override IIcon Icon => _icon;

        public override string LicenceDescription => "GPL v3";

        public override string LicenceAgreement => "GPL v3";
    }
}