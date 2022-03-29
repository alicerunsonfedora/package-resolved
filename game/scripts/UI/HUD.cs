// HUD.cs
// Package Resolved
//
// (C) 2021-2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;
using PackageResolved.Extensions;
using PackageResolved.Logic;

namespace PackageResolved.UI
{
    /// <summary>
    /// A class that controls the user interface for the heads-up display.
    /// </summary>
    public class HUD : Control
    {

        /// <summary>
        /// An enumeration representing the different tutorial text types.
        /// </summary>
        public enum TutorialText
        {
            /// <summary>
            /// The tutorial texts for basic movement.
            /// </summary>
            Movement,

            /// <summary>
            /// The tutorial texts describing how to extend a run.
            /// </summary>
            ExtraTime,

            /// <summary>
            /// The tutorial texts describing what hazards to be aware of.
            /// </summary>
            Hazards
        }

        /// <summary>
        /// A label that displays the number of packages remaining.
        /// </summary>
        private Label _packagesRemaining;

        /// <summary>
        /// The label that displays the start timer.
        /// </summary>
        private Label _startTimeLbl;

        /// <summary>
        /// The starting timer to display on the HUD.
        /// </summary>
        private int _startTime = 4;

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
            var state = this.GetCurrentState();
            if (state.GetCurrentLevel() > 2 || state.GetGameMode() == GameState.GameMode.Endless)
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
        /// Sets the tutorial text on the HUD for the player to read.
        /// </summary>
        /// <param name="text">The kind of tutorial text to display.</param>
        public void SetTutorialText(TutorialText text)
        {
            Label lbl = GetNode<Label>("IntroLabel/Top/VBoxContainer/Text");
            switch (text)
            {
                case TutorialText.Movement:
                    if (OS.HasTouchscreenUiHint())
                        lbl.Text = "Tap the left or right edges of the screen to move on the factory floor.";
                    else
                        lbl.Text = "Press the left or right arrow keys to move on the factory floor.";
                    break;
                case TutorialText.ExtraTime:
                    lbl.Text = "Collect timepieces to extend your time needed to fulfill requests.";
                    break;
                case TutorialText.Hazards:
                    lbl.Text = "Watch out for wet floors, and don't run into palettes.";
                    break;
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
        /// Decrement the start timer and display it on the HUD.
        /// </summary>
        public void UpdateStartTimer()
        {
            if (_startTime < 1)
            {
                _startTimeLbl.Visible = false;
                return;
            }
            _startTime -= 1;
            if (_startTime > 0)
                _startTimeLbl.Text = _startTime.ToString();
            else
                _startTimeLbl.Text = "GO";
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
            _startTimeLbl = GetNode<Label>("StartTime");
            _timeLimit = GetNode<Label>("TimeLimit/Label");
        }
    }
}