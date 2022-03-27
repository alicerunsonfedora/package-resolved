// MainMenu.cs
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
    /// A class that represents the main menu user interface.
    /// </summary>
    public class MainMenu : Control
    {
        /// <summary>
        /// A button that allows the player to start the game in arcade mode.
        /// </summary>
        private Button _btnStartArcade;

        /// <summary>
        /// A button that allows the player to start the game in endless mode.
        /// </summary>
        private Button _btnStartEndless;

        /// <summary>
        /// A checkbox toggle that allows the player to turn the music on or off.
        /// </summary>
        private CheckButton _chkMusic;

        /// <summary>
        /// A checkbox toggle that allows the player to turn the sound effects on or off.
        /// </summary>
        private CheckButton _chkSfx;

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
            ConnectSignals();
            var state = GetNode<GameState>("/root/GameState");

            _chkMusic.Pressed = state.IsMusicEnabled();
            _chkSfx.Pressed = state.IsSoundEffectsEnabled();

            if (state.IsComplete())
                _helmet.Visible = false;
        }

        /// <summary>
        /// A callback method called when the player clicks the Start Arcade button. 
        /// </summary>
        private void BtnPressStartArcade()
        {
            var state = GetNode<GameState>("/root/GameState");
            state.SetGameMode(GameState.GameMode.Arcade);
            GetTree().ChangeScene("res://scenes/preflight.tscn");
        }

        /// <summary>
        /// A callback method called when the player clicks the Start Endless button.
        /// </summary>
        private void BtnPressStartEndless()
        {
            var state = GetNode<GameState>("/root/GameState");
            state.SetGameMode(GameState.GameMode.Endless);
            GetTree().ChangeScene("res://scenes/game_loop.tscn");
        }

        /// <summary>
        /// A callback method that updates the state of the music checkbox toggle.
        /// </summary>
        /// <param name="value">The value that will become the new state of the toggle.</param>
        /// <remarks>
        /// This method will also toggle the state of whether the music is enabled.
        /// </remarks>
        private void ChkMusicToggle(bool value)
        {
            var state = GetNode<GameState>("/root/GameState");
            state.SetMusicEnabled(value);
        }

        /// <summary>
        /// A callback method that updates the state of the sound effects toggle.
        /// </summary>
        /// <param name="value">The value that will become the new state of the toggle.</param>
        /// <remarks>
        /// This method will also toggle the state of whether the sound effects are enabled.
        /// </remarks>
        private void ChkSfxToggle(bool value)
        {
            var state = GetNode<GameState>("/root/GameState");
            state.SetSoundEffectsEnabled(value);
        }

        /// <summary>
        /// Connect the signals to their corresponding callback methods, which ensures that the appropriate actions are
        /// performed when a player clicks on them.
        /// </summary>
        private void ConnectSignals()
        {
            _btnStartArcade.Connect("button_up", this, nameof(BtnPressStartArcade));
            _btnStartEndless.Connect("button_up", this, nameof(BtnPressStartEndless));
            _chkMusic.Connect("toggled", this, nameof(ChkMusicToggle));
            _chkSfx.Connect("toggled", this, nameof(ChkSfxToggle));
        }

        /// <summary>
        /// Instantiate fields that reference nodes in the scene tree.
        /// </summary>
        /// <remarks>
        /// In GDScript, these fields would be marked with <c>onready</c>.
        /// </remarks>
        private void InstantiateOnreadyInstances()
        {
            _btnStartArcade = GetNode<Button>("VBoxContainer/Start");
            _btnStartEndless = GetNode<Button>("VBoxContainer/Endless");
            _chkMusic = GetNode<CheckButton>("VBoxContainer/Music");
            _chkSfx = GetNode<CheckButton>("VBoxContainer/SFX");
            _helmet = GetNode<TextureRect>("Base/Helmet");
        }
    }
}