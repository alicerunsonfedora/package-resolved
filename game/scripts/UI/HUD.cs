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
            var tutLbl = GetNode<Control>("IntroLabel");
            var state = this.GetCurrentState();

            if (state.GetGameMode() == GameState.GameMode.Endless)
                _timeLimit.GetParent<Control>().Visible = false;

            if (state.GetCurrentLevel() > 2 || state.GetGameMode() == GameState.GameMode.Endless)
                tutLbl.Visible = false;
            else
                ConnectAnimation(tutLbl);
        }

        /// <summary>
        /// Connects the fade animation to the specified control.
        /// </summary>
        /// <param name="ctrl">The control to be faded out to transparency.</param>
        private void ConnectAnimation(Control ctrl)
        {
            var trans = Tween.TransitionType.Linear;
            var ease = Tween.EaseType.InOut;
            _tween.InterpolateProperty(ctrl, "modulate", Colors.White, Colors.Transparent, 0.5f, trans, ease);
            _timer.Connect("timeout", _tween, "start");
            _timer.Start();
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
                        lbl.Text = "Press the left/right arrow keys or the A/D keys to move on the factory floor.";
                    break;
                case TutorialText.ExtraTime:
                    lbl.Text = "Collect timepieces to extend your time needed to fulfill requests.";
                    break;
                case TutorialText.Hazards:
                    lbl.Text = "Be safe. Be careful on wet floors, as they are slippery. Avoid running into palettes.";
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