// GameOver.cs
// Package Resolved
//
// (C) 2021-2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;

namespace PackageResolved.UI
{
    /// <summary>
    /// A class that manages the user interface for the game over screen.
    /// </summary>
    public class GameOver : Control
    {
        /// <summary>
        /// The button that causes the game to restart when clicked.
        /// </summary>
        private Button _btnRestart;

        /// <summary>
        /// The button that allows the player to return to the main menu.
        /// </summary>
        private Button _btnQuitToMenu;

        /// <summary>
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
        public override void _Ready()
        {
            _btnRestart = GetNode<Button>("VBoxContainer/Restart");
            _btnQuitToMenu = GetNode<Button>("VBoxContainer/MainMenu");

            _btnRestart.Connect("button_up", this, "BtnPressRestart");
            _btnQuitToMenu.Connect("button_up", this, "BtnPressQuitToMenu");
        }

        /// <summary>
        /// A callback method called when the user clicks on the restart button.
        /// </summary>
        private void BtnPressRestart()
        {
            GetTree().ChangeScene("res://scenes/game_loop.tscn");
        }

        /// <summary>
        /// A callback method called when the user clicks on the main menu button.
        /// </summary>
        private void BtnPressQuitToMenu()
        {
            GetTree().ChangeScene("res://scenes/main_menu.tscn");
        }
    }
}