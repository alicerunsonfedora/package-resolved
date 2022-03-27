// LevelSuccess.cs
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
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            _btnAdvance.Connect("button_up", this, nameof(BtnPressRestart));
            _btnQuitToMenu.Connect("button_up", this, nameof(BtnPressQuitToMenu));
            HideHelmetAndAdvanceButton();
        }

        /// <summary>
        /// A callback method called when the player clicks on the advance button.
        /// </summary>
        private void BtnPressRestart()
        {
            GetTree().ChangeScene("res://scenes/preflight.tscn");
        }

        /// <summary>
        /// A callback method called when the player clicks on the button to go to the main menu.
        /// </summary>
        private void BtnPressQuitToMenu()
        {
            GetTree().ChangeScene("res://scenes/main_menu.tscn");
        }

        /// <summary>
        /// Determine if the helmet should be hidden and, if necessary, hide it from the screen.
        /// </summary>
        /// <remarks>
        /// The helmet will usually be hidden when the player completes the game.
        /// </remarks>
        private void HideHelmetAndAdvanceButton()
        {
            var state = GetNode<GameState>("/root/GameState");
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
            _btnAdvance = GetNode<Button>("VBoxContainer/Restart");
            _btnQuitToMenu = GetNode<Button>("VBoxContainer/MainMenu");
            _helmet = GetNode<TextureRect>("Panel/Base/Helmet");
        }

    }
}