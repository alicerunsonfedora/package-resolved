// HUD.cs
// Package Resolved
//
// (C) 2021-2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;
using PackageResolved.Logic;

namespace PackageResolved.UI
{
    /// <summary>
    /// A class that controls the user interface for the heads-up display.
    /// </summary>
    public class HUD : Control
    {
        /// <summary>
        /// A label that displays the number of packages remaining.
        /// </summary>
        private Label _packagesRemaining;

        /// <summary>
        /// A label that displays the time remaining.
        /// </summary>
        private Label _timeLimit;

        /// <summary>
        /// A timer used to time how long to display the tutorial screens for.
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// A tween animation node used to fade out the tutorial screen.
        /// </summary>
        private Tween _tween;

        /// <summary>
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            var introLabel = GetNode<Control>("IntroLabel");
            var state = GetNode<GameState>("/root/GameState");
            if (state.GetCurrentLevel() > 0 || state.GetGameMode() == GameState.GameMode.Endless)
                introLabel.Visible = false;
            else
            {
                _tween.InterpolateProperty(
                    introLabel,
                    "modulate",
                    Colors.White,
                    Colors.Transparent,
                    0.5f,
                    Tween.TransitionType.Linear,
                    Tween.EaseType.InOut
                );
                _timer.Connect("timeout", _tween, "start");
                _timer.Start();
            }
        }

        /// <summary>
        /// Update the packages remaining label.
        /// </summary>
        /// <param name="text">The number of packages remaining as a string value.</param>
        public void UpdatePackagesRemaining(string text)
        {
            _packagesRemaining.Text = text;
        }

        /// <summary>
        /// Update the time limit label.
        /// </summary>
        /// <param name="text">The remaining time as a string value.</param>
        public void UpdateTimeLimit(string text)
        {
            _timeLimit.Text = text;
        }


        /// <summary>
        /// Instantiate fields that reference nodes in the scene tree.
        /// </summary>
        /// <remarks>
        /// In GDScript, these fields would be marked with <c>onready</c>.
        /// </remarks>
        private void InstantiateOnreadyInstances()
        {
            _timer = GetNode<Timer>("Timer");
            _tween = GetNode<Tween>("Tween");
            _packagesRemaining = GetNode<Label>("PackagesRemaining/Label");
            _timeLimit = GetNode<Label>("TimeLimit/Label");
        }
    }
}