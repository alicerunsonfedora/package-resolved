// GameOver.cs
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
    /// A class that manages the user interface for the game over screen.
    /// </summary>
    public class GameOver : Control
    {

        /// <summary>
        /// The body text that is displayed under the "Game Over" screen.
        /// </summary>
        private Label _body;

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
            InstantiateOnreadyInstances();
            _ = _btnRestart.Connect("button_up", this, "BtnPressRestart");
            _ = _btnQuitToMenu.Connect("button_up", this, "BtnPressQuitToMenu");
            UpdateBody();
        }

        /// <summary>
        /// A callback method called when the user clicks on the restart button.
        /// </summary>
        private void BtnPressRestart()
        {
            var state = GetNode<GameState>("/root/GameState");
            state.Reset(false);
            GetTree().ChangeScene("res://scenes/game_loop.tscn");
        }

        /// <summary>
        /// A callback method called when the user clicks on the main menu button.
        /// </summary>
        private void BtnPressQuitToMenu()
        {
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
            _body = GetNode<Label>("VBoxContainer/Body");
            _btnRestart = GetNode<Button>("VBoxContainer/Restart");
            _btnQuitToMenu = GetNode<Button>("VBoxContainer/MainMenu");
        }

        private void UpdateBody()
        {
            string text;
            var state = GetNode<GameState>("/root/GameState");
            if (state.GetGameMode() == GameState.GameMode.Endless)
                text = $"You ran into a palette and tripped!\nScore: {state.GetPreviousScore()} packages";
            else if (state.GetGameMode() == GameState.GameMode.Arcade && state.GetPreviousTimeLeft() <= 0)
                text = $"You couldn't collect all of the packages in time!\nScore: {state.GetPreviousScore()} "
                    + "packages";
            else
                text = $"You ran into a palette and tripped!\nScore: {state.GetPreviousScore()} packages\n"
                    + $"Time: {state.GetPreviousTimeLeft()} seconds";
            _body.Text = text;
        }
    }
}