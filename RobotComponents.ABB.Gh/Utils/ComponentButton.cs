// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2022-2024 Robots Authors - Original work from https://github.com/visose/Robots
// Copyright (c) 2026 Arjen Deetman - Modifications
//
// Authors:
//   - Vicente Soler a.k.a visose (2022-2024) - Original author, Robot Authors
//   - Arjen Deetman (2026) - Modifications for Robot Components
//
// For license details:
//   - Original work: MIT License (see https://github.com/visose/Robots)
//   - Modifications: GPL-3.0-or-later (see LICENSE file in the project root)

// System Libs
using System;
using System.Drawing;
using System.Windows.Forms;
// Grasshoper Libs
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;

namespace RobotComponents.ABB.Gh.Utils
{
    /// <summary>
    /// Represents an attribute to add a custom button to a Grasshopper component.
    /// This class extends GH_ComponentAttributes to provide interactive button functionality.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use this class to add a clickable button to a Grasshopper component.
    /// The button can trigger a specified action when clicked.
    /// </para>
    /// <para>
    /// Example usage in a component:
    /// <code>
    /// public override void CreateAttributes()
    /// {
    ///     m_attributes = new ComponentButton(this, "Button Name", ToggleButton);
    /// }
    ///
    /// private void ToggleButton()
    /// {
    ///     // Define the action to perform when the button is clicked.
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    internal class ComponentButton : GH_ComponentAttributes
    {
        private readonly string _label;
        private readonly Action _action;
        private const int _buttonSize = 18;
        private bool _mouseDown;
        private RectangleF _buttonBounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentButton"/> class.
        /// </summary>
        /// <param name="owner">The Grasshopper component to which this attribute is attached.</param>
        /// <param name="label">The text displayed on the button.</param>
        /// <param name="action">The action to execute when the button is clicked.</param>
        public ComponentButton(GH_Component owner, string label, Action action) : base(owner)
        {
            _label = label;
            _action = action;
        }

        /// <summary>
        /// Adjusts the layout of the component and its button.
        /// </summary>
        protected override void Layout()
        {
            base.Layout();

            const int margin = 3;

            var bounds = GH_Convert.ToRectangle(Bounds);
            var button = bounds;

            button.X += margin;
            button.Width -= margin * 2;
            button.Y = bounds.Bottom;
            button.Height = _buttonSize;

            bounds.Height += _buttonSize + margin;

            Bounds = bounds;
            _buttonBounds = button;
        }

        /// <summary>
        /// Renders the button on the Grasshopper canvas.
        /// </summary>
        /// <param name="canvas">The canvas on which to render.</param>
        /// <param name="graphics">The graphics object used for rendering.</param>
        /// <param name="channel">The rendering channel.</param>
        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            base.Render(canvas, graphics, channel);

            if (channel == GH_CanvasChannel.Objects)
            {
                var prototype = GH_FontServer.StandardAdjusted;
                var font = GH_FontServer.NewFont(prototype, 6f / GH_GraphicsUtil.UiScale);
                var radius = 3;
                var highlight = !_mouseDown ? 8 : 0;

                var button = GH_Capsule.CreateTextCapsule(_buttonBounds, _buttonBounds, GH_Palette.Black, _label, font, radius, highlight);
                button.Render(graphics, false, Owner.Locked, false);
                button.Dispose();
            }
        }

        /// <summary>
        /// Sets the mouse down state and triggers the button action if necessary.
        /// </summary>
        /// <param name="value">Whether the mouse is down.</param>
        /// <param name="canvas">The canvas where the interaction occurs.</param>
        /// <param name="e">The mouse event arguments.</param>
        /// <param name="action">Whether to trigger the button action.</param>
        void SetMouseDown(bool value, GH_Canvas canvas, GH_CanvasMouseEvent e, bool action = true)
        {
            if (Owner.Locked || _mouseDown == value)
            {
                return;
            }

            if (value && e.Button != MouseButtons.Left)
            {
                return;
            }

            if (!_buttonBounds.Contains(e.CanvasLocation))
            {
                return;
            }

            if (_mouseDown && !value && action)
            {
                _action();
            }

            _mouseDown = value;
            canvas.Invalidate();
        }

        /// <summary>
        /// Handles the mouse down event for the button.
        /// </summary>
        /// <param name="sender">The canvas sending the event.</param>
        /// <param name="e">The mouse event arguments.</param>
        /// <returns>The response to the mouse event.</returns>
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            SetMouseDown(true, sender, e);
            return base.RespondToMouseDown(sender, e);
        }

        /// <summary>
        /// Handles the mouse up event for the button.
        /// </summary>
        /// <param name="sender">The canvas sending the event.</param>
        /// <param name="e">The mouse event arguments.</param>
        /// <returns>The response to the mouse event.</returns>
        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            SetMouseDown(false, sender, e);
            return base.RespondToMouseUp(sender, e);
        }

        /// <summary>
        /// Handles the mouse move event for the button.
        /// </summary>
        /// <param name="sender">The canvas sending the event.</param>
        /// <param name="e">The mouse event arguments.</param>
        /// <returns>The response to the mouse event.</returns>
        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            SetMouseDown(false, sender, e, false);
            return base.RespondToMouseMove(sender, e);
        }
    }
}
