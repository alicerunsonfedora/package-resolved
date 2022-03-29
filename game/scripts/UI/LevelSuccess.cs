// LevelSuccess.cs
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
    /// A class that represents the level success user interface screen.
    /// </summary>
    public class LevelSuccess : Control
    {
        /// <summary>
        /// A button that allows the player to restart the game or advance to the next level.
        /// </summary>
        private Button _btnAdvance;

        /// <summary>
        /// A button that allows the player to go to the main menu.
        /// </summary>
        private Button _btnQuitToMenu;

        /// <summary>
        /// The texture rectangle that contains the sprite image for the bird helmet.
        /// </summary>
        private TextureRect _helmet;

        /// <summary>
        /// The label that displays the number of packages the player needed to collect.
        /// </summary>
        private Label _lblPackagesRequired;

        /// <summary>
        /// The label that displays the time limit the player had.
        /// </summary>
        private Label _lblTimeLimit;

        /// <summary>
        /// The label that displays the level number in the form of "Order #Num Delivered".
        /// </summary>
        private Label _lblLevelNumber;

        /// <summary>
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            _btnAdvance.Connect("button_up", this, nameof(BtnPressRestart));
            _btnQuitToMenu.Connect("button_up", this, nameof(BtnPressQuitToMenu));

            var state = this.GetCurrentState();
            _lblLevelNumber.Text = $"Order #{state.GetCurrentLevel() + 1:D3} Delivered";
            _lblPackagesRequired.Text = state.GetRequiredPackages().ToString();
            _lblTimeLimit.Text = state.GetTimeLimit().ToString();

            HideHelmetAndAdvanceButton();
        }

        /// <summary>
        /// A callback method called when the player clicks on the advance button.
        /// </summary>
        private void BtnPressRestart()
        {
            GetTree().ChangeScene("res://scenes/screens/preflight.tscn");
        }

        /// <summary>
        /// A callback method called when the player clicks on the button to go to the main menu.
        /// </summary>
        private void BtnPressQuitToMenu()
        {
            GetTree().ChangeScene("res://scenes/screens/main_menu.tscn");
        }

        /// <summary>
        /// Determine if the helmet should be hidden and, if necessary, hide it from the screen.
        /// </summary>
        /// <remarks>
        /// The helmet will usually be hidden when the player completes the game.
        /// </remarks>
        private void HideHelmetAndAdvanceButton()
        {
            var state = this.GetCurrentState();
            state.Progress();
            if (!state.IsComplete())
                return;
            _btnAdvance.Visible = false;
            _helmet.Visible = false;
        }

        /// <summary>
        /// Instantiate fields that reference nodes in the scene tree.
        /// </summary>
        /// <remarks>
        /// In GDScript, these fields would be marked with <c>onready</c>.
        /// </remarks>
        private void InstantiateOnreadyInstances()
        {
            _btnAdvance = GetNode<Button>("Panel/VStack/HBoxContainer2/Restart");
            _btnQuitToMenu = GetNode<Button>("Panel/VStack/HBoxContainer2/MainMenu");
            _helmet = GetNode<TextureRect>("BackgroundPanel/Base/Helmet");
            _lblLevelNumber = GetNode<Label>("Panel/VStack/Title");
            _lblPackagesRequired = GetNode<Label>("Panel/VStack/PackageHStack/RequiredPackages");
            _lblTimeLimit = GetNode<Label>("Panel/VStack/TimeHStack/TimeLimit");
        }

    }
}