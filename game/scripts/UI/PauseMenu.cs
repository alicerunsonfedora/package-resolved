// PauseMenu.cs
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
    /// A class that represents the pause menu in the user interface.
    /// </summary>
    public class PauseMenu : Control
    {
        /// <summary>
        /// A button that allows the player to resume execution of the current game loop.
        /// </summary>
        private Button _btnResume;

        /// <summary>
        /// A button that allows the player to restart the current level.
        /// </summary>
        private Button _btnRestart;

        /// <summary>
        /// A button that allows the player to go back to the main menu.
        /// </summary>
        private Button _btnMainMenu;

        /// <summary>
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            _btnResume.Connect("button_up", this, nameof(BtnResumePress));
            _btnRestart.Connect("button_up", this, nameof(BtnRestartPress));
            _btnMainMenu.Connect("button_up", this, nameof(BtnMainMenuPress));
        }

        /// <summary>
        /// A callback method that resumes execution of the current game loop.
        /// </summary>
        private void BtnResumePress()
        {
            GetTree().Paused = false;
            Visible = false;
        }

        /// <summary>
        /// A callback method that restarts the game loop.
        /// </summary>
        private void BtnRestartPress()
        {
            GetTree().Paused = false;
            GetTree().ChangeScene("res://scenes/game_loop.tscn");
        }

        /// <summary>
        /// A callback method that returns the player to the main menu, without saving.
        /// </summary>
        private void BtnMainMenuPress()
        {
            GetTree().Paused = false;
            GetTree().ChangeScene("res://scenes/main_menu.tscn");
        }

        /// <summary>
        /// Instantiate fields that reference nodes in the scene tree.
        /// </summary>
        /// <remarks>
        /// In GDScript, these fields would be marked with <c>onready</c>.
        /// </remarks>
        private void InstantiateOnreadyInstances()
        {
            _btnResume = GetNode<Button>("VBoxContainer/Resume");
            _btnRestart = GetNode<Button>("VBoxContainer/Restart");
            _btnMainMenu = GetNode<Button>("VBoxContainer/MainMenu");
        }
    }
}


