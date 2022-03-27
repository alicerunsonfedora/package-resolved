// Preflight.cs
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
    /// A class that represents the Preflight scren the player sees when starting a new level.
    /// </summary>
    /// <remarks>
    /// This screen is used to allow the player to understand the objectives for the current level before starting it.
    /// </remarks>
    class Preflight : Control
    {
        /// <summary>
        /// The button that the player clicks to start the level.
        /// </summary>
        private Button _btnStart;

        /// <summary>
        /// The label containing the level number, formatted as "Order Number #num".
        /// </summary>
        private Label _lblLevel;

        /// <summary>
        /// The label containing the number of required packages to collect in this level.
        /// </summary>
        private Label _lblPackages;

        /// <summary>
        /// The label containing the time limit for this level.
        /// </summary>
        private Label _lblTime;

        /// <summary>
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            _btnStart.Connect("button_up", this, nameof(StartRequest));

            var state = GetNode<GameState>("/root/GameState");
            SetRequest(state.GetCurrentLevel() + 1, state.GetRequiredPackages(), state.GetTimeLimit());
        }

        /// <summary>
        /// Update the preflight screen with the level details.
        /// </summary>
        /// <param name="orderNumber">The human-readable version of the current level number.</param>
        /// <param name="requiredPackages">The number of packages the player needs to collect.</param>
        /// <param name="timeLimit">The time limit (in seconds) the player has to complete the level.</param>
        public void SetRequest(int orderNumber, int requiredPackages, int timeLimit)
        {
            _lblLevel.Text = $"Order Request #{orderNumber}";
            _lblPackages.Text = requiredPackages.ToString();
            _lblTime.Text = timeLimit.ToString();
        }

        /// <summary>
        /// Instantiate fields that reference nodes in the scene tree.
        /// </summary>
        /// <remarks>
        /// In GDScript, these fields would be marked with <c>onready</c>.
        /// </remarks>
        private void InstantiateOnreadyInstances()
        {
            _btnStart = GetNode<Button>("Panel/HStack/VStack2/Button");
            _lblLevel = GetNode<Label>("Panel/HStack/VStack/Title");
            _lblPackages = GetNode<Label>("Panel/HStack/VStack/PackageHStack/RequiredPackages");
            _lblTime = GetNode<Label>("Panel/HStack/VStack/TimeHStack/TimeLimit");
        }

        /// <summary>
        /// Starts the level.
        /// </summary>
        private void StartRequest()
        {
            GetTree().ChangeScene("res://scenes/game_loop.tscn");
        }
    }
}