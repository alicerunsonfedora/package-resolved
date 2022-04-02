// GameLoop.cs
// Package Resolved
//
// (C) 2021-2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;
using Godot.Collections;
using PackageResolved.Extensions;
using PackageResolved.Objects;
using PackageResolved.UI;
using PackageResolved.Utilities;

namespace PackageResolved.Logic
{
    /// <summary>
    /// The primary game loop.
    /// </summary>
    public class GameLoop : Node2D, ITeardownable
    {

        /// <summary>
        /// The rate at which timer modifiers grow over time.
        /// </summary>
        /// <remarks>
        /// This is a hyperparamter which is used to control the timepiece addition algorithm.
        /// </remarks>
        /// <seealso cref="PackageResolved.Logic.GameLevelState.CalculateTimeModifier(float, float)"/>
        [Export]
        public float TimerGrowthRate = 0.1f;

        /// <summary>
        /// The heads-up display that shows over the main level.
        /// </summary>
        private HUD _headsUpDisplay;

        /// <summary>
        /// An array of obstacle positions.
        /// </summary>
        /// <remarks>
        /// This is typically used to determine what positions pickable items cannot be placed.
        /// </remarks>
        private readonly Array _obstaclePositions = new Array();

        /// <summary>
        /// The player controller used in the level.
        /// </summary>
        private Player _playerNode;

        /// <summary>
        /// The current game state of the level.
        /// </summary>
        private GameLevelState _state;

        /// <summary>
        /// The teleportation controller that handles the teleportation logic.
        /// </summary>
        private TeleportController _teleportController;

        /// <summary>
        /// The time controller that handles all of the logic pertaining to the level's timers.
        /// </summary>
        private TimeController _timeController;

        /// <summary>
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            ConnectInstances();
            SetupTween();
            _playerNode.BlockMovement();
            SetupInitialScene();
            ConfigureHeadsUpDisplay();
        }

        /// <summary>
        /// Process the physics objects in the scene.
        /// </summary>
        /// <param name="delta">The change in time since the previous frame.</param>
        public override void _PhysicsProcess(float delta)
        {
            _teleportController.UpdateTargetPosition(_playerNode.Position);
        }

        /// <summary>
        /// Process keystrokes in the game loop and perform the appropriate action.
        /// </summary>
        /// <param name="event">The keystoke event that needs to be processed.</param>
        /// <remarks>
        /// This method will listen for the pause key to suspend the current tree state and show the pause menu.
        /// </remarks>
        public override void _UnhandledKeyInput(InputEventKey @event)
        {
            if (@event.GetActionStrength("ui_pause") > 0)
                HandlePause();
        }

        /// <summary>
        /// Indicate the game is over and show the game over screen.
        /// </summary>
        public void GameOver()
        {
            GameState state = this.GetCurrentState();
            UpdatePreviousRunInState(state);
            _ = GetTree().ChangeScene("res://scenes/screens/game_over.tscn");
        }

        /// <summary>
        /// Returns the current player in the game loop.
        /// </summary>
        public Player GetCurrentPlayer() => _playerNode;

        /// <summary>
        /// Returns an array containing the positions of the current obstacles on the map.
        /// </summary>
        public Array GetObstaclePositions() => _obstaclePositions;

        /// <summary>
        /// A callback method that runs every second when the <c>TimerTick</c> times out.
        /// </summary>
        /// <remarks>
        /// This method primarily updates the heads-up display with the remaining time.
        /// </remarks>
        public void Tick()
        {
            int timeLeft = (int)_timeController.GetTimeLeft();
            _headsUpDisplay.UpdateTimeLimit($"{timeLeft}");

            if (_timeController.GetStartCountdown() >= 0)
                _headsUpDisplay.UpdateStartTimer();
        }

        /// <summary>
        /// Configures the heads-up display with the level requirements and any tutorials.
        /// </summary>
        private void ConfigureHeadsUpDisplay()
        {
            GameState gameState = this.GetCurrentState();
            _state = new GameLevelState { PackagesRemaining = 0, LastPackageTimestamp = 0 };
            if (gameState.CurrentGameMode == GameState.GameMode.Arcade)
            {
                SetupArcadeMode();
                _headsUpDisplay.UpdatePackagesRemaining(_state.PackagesRemaining.ToString());
                _headsUpDisplay.UpdateTimeLimit(gameState.GetTimeLimit().ToString());
                Tick();
            }

            _headsUpDisplay.SetupTutorialText();
        }

        /// <summary>
        /// Hooks up instanced objects to their corresponding callback methods.
        /// </summary>
        private void ConnectInstances()
        {
            _timeController.ConnectPlayerLock(_playerNode);
            _timeController.ConnectTick(this);
            _timeController.ConnectGameOver(this);
            _teleportController.Connect(nameof(TeleportController.Teleported), this, nameof(OnTeleported));
        }

        /// <summary>
        /// Creates the teleportation controller used to teleport the player.
        /// </summary>
        private void CreateTeleportController()
        {
            var teleportTrigger = GetNode<Area2D>("TeleportTrigger");
            var teleportDestination = GetNode<Node2D>("TeleportDestination");
            _teleportController = new TeleportController(teleportTrigger, teleportDestination);
        }

        /// <summary>
        /// Grabs the timers in the level and instantiates the time controller.
        /// </summary>
        private void CreateTimeController()
        {
            var timerStart = GetNode<Timer>("StartTimer");
            var timerTick = GetNode<Timer>("Tick");
            var timerLevel = GetNode<Timer>("Timer");
            _timeController = new TimeController(timerStart, timerTick, timerLevel);
        }

        /// <summary>
        /// Opens the pause menu and pauses the tree.
        /// </summary>
        private void HandlePause()
        {
            var pauseMenu = GetNode<PauseMenu>("CanvasLayer/PauseMenu");
            pauseMenu.Visible = true;
            GetTree().Paused = true;
        }

        /// <summary>
        /// Instantiate fields that reference nodes in the scene tree.
        /// </summary>
        /// <remarks>
        /// In GDScript, these fields would be marked with <c>onready</c>.
        /// </remarks>
        private void InstantiateOnreadyInstances()
        {
            _headsUpDisplay = GetNode<HUD>("CanvasLayer/HUD");
            _playerNode = GetNode<Player>("Player");
            CreateTimeController();
            CreateTeleportController();
        }

        /// <summary>
        /// A callback method that runs when the user picks up a pickable item marked as a time modifier.
        /// </summary>
        /// <remarks>
        /// This method will reset the timer to add the additional time that the modifier will provide using the formula
        /// <c>bonusTime = packagesLeft * (lastPackageCollectTime - timeLeft) ^ (a)</c>, where <c>a</c> corresponds to
        /// the growth rate described in the editor as <c>TimerGrowthRate</c>. If the algorithm's result value is
        /// greater than zero, that time will be added to the timer. Effectively, this forces the player to consider 
        /// whether to pick up modifiers all the time or to pick up modifiers less frequently in hopes of a bigger
        /// rewarded time bonus.
        /// </remarks>
        private void OnPickedModifier()
        {
            if (this.GetCurrentState().CurrentGameMode == GameState.GameMode.Endless)
                return;

            var growth = _state.CalculateTimeModifier(TimerGrowthRate, _timeController.GetTimeLeft());
            if (growth > 0)
                _timeController.AddTimeToLimit(growth);
            Tick();
        }

        /// <summary>
        /// A callback method that runs when the player picks up a pickable item marked as a package of some kind.
        /// </summary>
        /// <param name="amount"> The number of packages to decrease <c>PackagesRemaining</c> by. </param>
        /// <remarks>
        /// This method will subtract the amount of packages the pickable represents from the number of packages
        /// remaining.
        /// </remarks>
        private void OnPickedPackage(int amount)
        {
            UpdatePackageCounter(amount);
            _headsUpDisplay.UpdatePackagesRemaining(_state.PackagesRemaining.ToString());
            _state.LastPackageTimestamp = _timeController.GetTimeLeft();
        }

        /// <summary>
        /// A callback method that runs when the player is teleported.
        /// </summary>
        /// <remarks>
        /// This method will tear down the old generated pickable items and hazards and place new ones accordingly.
        /// </remarks>
        private void OnTeleported()
        {
            Teardown();
            var state = this.GetCurrentState();
            if ((state.GetCurrentLevel() > 1) || (state.CurrentGameMode == GameState.GameMode.Endless))
                PlaceHazards();
            PlacePickables();
        }

        /// <summary>
        /// Place the hazards randomly in the level play area.
        /// </summary>
        private void PlaceHazards()
        {
            float lastVertPosition = 300f;
            for (int i = 0; i <= 4; i += 1)
                lastVertPosition = ProceduralGeneration.PlaceNewHazard(lastVertPosition, this);
        }

        /// <summary>
        /// Place the pickable items randomly in the level play area.
        /// </summary>
        private void PlacePickables()
        {
            float lastVertPosition = -64f;
            for (int i = 0; i <= 8; i += 1)
                lastVertPosition = ProceduralGeneration.PlaceNewPickable(lastVertPosition, this);
        }

        /// <summary>
        /// Sets up the arcade mode for the game.
        /// </summary>
        private void SetupArcadeMode()
        {
            var gameState = this.GetCurrentState();
            _state.PackagesRemaining = gameState.GetRequiredPackages();
            _timeController.SetTimeLimit(gameState.GetTimeLimit());
        }

        /// <summary>
        /// Sets up the initial layout of the procedurally-generated items.
        /// </summary>
        private void SetupInitialScene()
        {
            GD.Randomize();
            if (this.GetCurrentState().GetCurrentLevel() > 1)
                PlaceHazards();
            PlacePickables();
        }

        /// <summary>
        /// Sets up the tween node that fades the level out on a success.
        /// </summary>
        private void SetupTween()
        {
            var tween = GetNode<Tween>("Tween");
            tween.Fadeout(this, 3.0f);
            tween.Connect("tween_all_completed", this, nameof(SuccessCallback));
        }

        /// <summary>
        /// Switch the scene to the level success screen.
        /// </summary>
        /// <remarks>
        /// Called as a callback after the tween finishes animating.
        /// </remarks>
        private void SuccessCallback()
        {
            _ = GetTree().ChangeScene("res://scenes/screens/level_success.tscn");
        }

        /// <summary>
        /// Start the tween to fade out the level, stop the player movement, and end the timer.
        /// </summary>
        private void SuccessStart()
        {
            _playerNode.BlockMovement();
            _timeController.StopLoop();
            var tween = GetNode<Tween>("Tween");
            tween.Start();
        }

        /// <summary>
        /// Tears down the current scene to prepare for reinstantiation.
        /// </summary>
        /// <remarks>
        /// This method does <i>not</i> call <c>QueueFree</c>.
        /// </remarks>
        public void Teardown()
        {
            foreach (var child in GetChildren())
                TeardownChild(child);
            _obstaclePositions.Clear();
        }

        /// <summary>
        /// Tears down a child node if they conform to the ITeardownable interface.
        /// </summary>
        /// <param name="child">The child to try tearing down.</param>
        private static void TeardownChild(object child)
        {
            if (!(child is ITeardownable))
                return;
            (child as ITeardownable).Teardown();
        }

        /// <summary>
        /// Updates the package counter by the amount specified from the pickup.
        /// </summary>
        /// <param name="amount">The number of packages to either add or remove.</param>
        private void UpdatePackageCounter(int amount)
        {
            var gameState = this.GetCurrentState();
            if (gameState.CurrentGameMode == GameState.GameMode.Endless)
            {
                _state.PackagesRemaining += amount;
                return;
            }
            _state.PackagesRemaining -= amount;
            if (_state.PackagesRemaining <= 0)
                SuccessStart();
        }

        /// <summary>
        /// Updates the previous run scores in the specified game state.
        /// </summary>
        /// <param name="state">The state that will receive the previous scores.</param>
        private void UpdatePreviousRunInState(GameState state)
        {
            if (state.CurrentGameMode == GameState.GameMode.Endless)
            {
                state.UpdatePreviousRun(_state.PackagesRemaining, 0);
                return;
            }
            var time = _timeController.GetTimeLeft();
            state.UpdatePreviousRun(state.GetRequiredPackages() - _state.PackagesRemaining, (int)time);
        }
    }
}