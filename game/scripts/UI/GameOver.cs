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
        /// The label displaying the number of packages that were collected.
        /// </summary>
        private Label _packagesCollected;

        /// <summary>
        /// The label displaying the time remaining.
        /// </summary>
        private Label _timeRemaining;

        /// <summary>
        /// The horizontal stack containing the time remaining text.
        /// </summary>
        private HBoxContainer _timeRemainingStack;

        /// <summary>
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            _ = _btnRestart.Connect("button_up", this, nameof(BtnPressRestart));
            _ = _btnQuitToMenu.Connect("button_up", this, nameof(BtnPressQuitToMenu));
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
            var state = GetNode<GameState>("/root/GameState");
            state.Reset(false);
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
            _body = GetNode<Label>("Background/Panel/VStack/Body");
            _btnRestart = GetNode<Button>("Background/Panel/VStack/HBoxContainer2/Restart");
            _btnQuitToMenu = GetNode<Button>("Background/Panel/VStack/HBoxContainer2/MainMenu");
            _packagesCollected = GetNode<Label>("Background/Panel/VStack/PackageHStack/RequiredPackages");
            _timeRemaining = GetNode<Label>("Background/Panel/VStack/TimeHStack/TimeLimit");
            _timeRemainingStack = GetNode<HBoxContainer>("Background/Panel/VStack/TimeHStack");
        }

        private void UpdateBody()
        {
            string text;
            var state = GetNode<GameState>("/root/GameState");
            if (state.GetGameMode() == GameState.GameMode.Endless)
            {
                text = "Tripped over a palette and damaged company property.";
                _timeRemainingStack.Visible = false;
            }
            else if (state.GetGameMode() == GameState.GameMode.Arcade && state.GetPreviousTimeLeft() <= 0)
            {
                text = "Failed to fulfill request in alotted time.";
                _timeRemaining.Text = "0";
            }
            else
            {
                text = "Tripped over a palette and damaged company property.";
                _timeRemaining.Text = state.GetPreviousTimeLeft().ToString();
            }
            _body.Text = text;
            _packagesCollected.Text = state.GetPreviousScore().ToString();
        }
    }
}