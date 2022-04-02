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
        /// This is a hyperparamter which is used to control the equation for calculating how much time to add to the
        /// timer: <c>bonusTime = packagesLeft * (lastPackageCollectTime - timeLeft) ^ (a)</c>, where <c>a</c> is the
        /// timer growth rate.
        /// </remarks>
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
        /// The pause menu that will be displayed when the player presses the pause key.
        /// </summary>
        private PauseMenu _pauseMenu;

        /// <summary>
        /// The player controller used in the level.
        /// </summary>
        private Player _playerNode;

        /// <summary>
        /// The current game state of the level.
        /// </summary>
        private GameLevelState _state;

        /// <summary>
        /// The Area2D trigger that teleports the player to the top of the map.
        /// </summary>
        private Area2D _teleportTrigger;

        /// <summary>
        /// The Area2D used to define the top of the map.
        /// </summary>
        /// <remarks>
        /// This is used in conjunction with <c>TeleportTrigger</c> to teleport the player.
        /// </remarks>
        private Node2D _teleportDestination;

        /// <summary>
        /// The timer used to halt the player for a few seconds before starting the level.
        /// </summary>
        /// <remarks>
        /// This is done to give the player enough time to prepare.
        /// </remarks>
        private Timer _timerStart;

        /// <summary>
        /// The timer used to run code on every second.
        /// </summary>
        /// <remarks>
        /// The <c>Tick</c> method will be executed when this timer times out every day.
        /// </remarks>
        private Timer _timerTick;

        /// <summary>
        /// The timer used to run the game loop.
        /// </summary>
        /// <remarks>
        /// This timer will correspond to how much time the player has left.
        /// </remarks>
        private Timer _timerLevel;

        /// <summary>
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            ConnectInstances();
            SetupTween();
            _playerNode.BlockMovement();

            GD.Randomize();
            if (this.GetCurrentState().GetCurrentLevel() > 1)
                PlaceHazards();
            PlacePickables();

            ConfigureHeadsUpDisplay();
        }

        /// <summary>
        /// Process the physics objects in the scene.
        /// </summary>
        /// <param name="delta">The change in time since the previous frame.</param>
        public override void _PhysicsProcess(float delta)
        {
            _teleportDestination.Position = new Vector2(_playerNode.Position.x, _teleportDestination.Position.y);
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
            {
                _pauseMenu.Visible = true;
                GetTree().Paused = true;
            }
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

            SetupTutorialText();
        }

        /// <summary>
        /// Hooks up instanced objects to their corresponding callback methods.
        /// </summary>
        private void ConnectInstances()
        {
            _ = _teleportTrigger.Connect("body_entered", this, nameof(OnBodyEntered));
            _ = _timerStart.Connect("timeout", _timerLevel, "start");
            _ = _timerStart.Connect("timeout", _playerNode, nameof(_playerNode.UnblockMovement));
            _ = _timerTick.Connect("timeout", this, nameof(Tick));
            if (this.GetCurrentState().CurrentGameMode == GameState.GameMode.Arcade)
                _ = _timerLevel.Connect("timeout", this, nameof(GameOver));
        }

        /// <summary>
        /// Indicate the game is over and show the game over screen.
        /// </summary>
        private void GameOver()
        {
            GameState state = this.GetCurrentState();
            if (state.CurrentGameMode == GameState.GameMode.Endless)
                state.UpdatePreviousRun(_state.PackagesRemaining, (int)_timerLevel.WaitTime);
            else
                state.UpdatePreviousRun(state.GetRequiredPackages() - _state.PackagesRemaining, (int)_timerLevel.WaitTime);
            _ = GetTree().ChangeScene("res://scenes/screens/game_over.tscn");
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
            _pauseMenu = GetNode<PauseMenu>("CanvasLayer/PauseMenu");
            _playerNode = GetNode<Player>("Player");
            _teleportTrigger = GetNode<Area2D>("TeleportTrigger");
            _teleportDestination = GetNode<Node2D>("TeleportDestination");
            _timerStart = GetNode<Timer>("StartTimer");
            _timerTick = GetNode<Timer>("Tick");
            _timerLevel = GetNode<Timer>("Timer");
        }

        /// <summary>
        /// Creates a hazard node that will be placed in the scene tree.
        /// </summary>
        /// <return> An instance of <c>Hazard</c>. </return>
        /// <seealso> Hazard </seealso>
        /// <remarks>
        /// Hazards will start spawning on the third level in Arcade mode and after the first pass in Endless mode.
        /// </remarks>
        private Hazard MakeHazard()
        {
            Hazard hazard = Instancing.InstanceHazard();
            double hazardSeed = GD.RandRange(1, 20);
            if (hazardSeed < 15)
                _ = hazard.Connect("StartedContact", this, "GameOver");
            else
            {
                hazard.Kind = Hazard.Type.WetFloor;
                _ = hazard.Connect("StartedContact", _playerNode, "SpeedUp");
                _ = hazard.Connect("StoppedContact", _playerNode, "SlowDown");
            }
            hazard.SetupHazard();
            return hazard;
        }

        /// <summary>
        /// Creates a pickable item node that will be placed in the scene tree.
        /// </summary>
        /// <return> An instance of <c>Pickable</c>. </return>
        /// <seealso> Pickable </seealso>
        /// <remarks>
        /// Timepieces will not be spawned on the first level in Arcade mode and in Endless mode.
        /// </remarks>
        private Pickable MakePickable()
        {
            Pickable pickable = Instancing.InstancePickable();
            double seed = GD.RandRange(0, 50);
            var state = this.GetCurrentState();
            bool timeEligible = (state.GetCurrentLevel() > 0) && (state.CurrentGameMode == GameState.GameMode.Arcade);
            if (seed > 40)
                pickable.Kind = Pickable.Type.PackagePlus;
            else if ((seed > 30) && (seed <= 39) && (timeEligible))
                pickable.Kind = Pickable.Type.TimeModifier;

            pickable.RedrawSprite();
            _ = pickable.Connect("PickedPackage", this, "OnPickedPackage");
            _ = pickable.Connect("PickedModifier", this, "OnPickedModifier");
            return pickable;
        }

        /// <summary>
        /// A callback method that runs when the player enters the teleport trigger.
        /// </summary>
        /// <remarks> This method will teleport the player to the top of the screen and replace items in the
        /// level.
        /// </remarks>
        private void OnBodyEntered(Node2D body)
        {
            if (!(body is Player))
                return;

            body.Position = _teleportDestination.Position;
            Teardown();

            var state = this.GetCurrentState();
            if ((state.GetCurrentLevel() > 1) || (state.CurrentGameMode == GameState.GameMode.Endless))
                PlaceHazards();

            PlacePickables();
        }

        /// <summary>
        /// A callback method that runs when the user picks up a pickable item marked as a 
        /// time modifier.
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

            var growth = _state.CalculateTimeModifier(TimerGrowthRate, _timerLevel.TimeLeft);
            if (growth > 0)
                _timerLevel.AddTime(growth);
            Tick();
        }

        /// <summary>
        /// A callback method that runs when the user picks up a pickable item marked as a package of some
        /// kind.
        /// </summary>
        /// <param name="amount"> The number of packages to decrease <c>PackagesRemaining</c> by. </param>
        /// <remarks>
        /// This method will subtract the amount of packages the pickable represents from the number of
        /// packages remaining.
        /// </remarks>
        private void OnPickedPackage(int amount)
        {
            UpdatePackageCounter(amount);
            _headsUpDisplay.UpdatePackagesRemaining(_state.PackagesRemaining.ToString());
            _state.LastPackageTimestamp = _timerLevel.TimeLeft;
        }

        /// <summary>
        /// Place the hazards randomly in the level play area.
        /// </summary>
        private void PlaceHazards()
        {
            float lastVertPosition = 300f;
            for (int i = 0; i <= 4; i += 1)
                lastVertPosition = PlaceNewHazard(lastVertPosition);
        }

        /// <summary>
        /// Place a new hazard on the map.
        /// </summary>
        /// <param name="lastVertPosition">The vertical position of the last item that was placed.</param>
        /// <returns>The next vertical position that the next item should be placed at.</returns>
        private float PlaceNewHazard(float lastVertPosition)
        {
            Hazard hazard = MakeHazard();
            hazard.Position = Vector.GetPlacableVector(lastVertPosition, 96);
            _ = _obstaclePositions.Add(hazard.Position);
            CallDeferred("add_child", hazard);
            lastVertPosition += 64;
            return lastVertPosition;
        }

        /// <summary>
        /// Place a new pickable item on the map.
        /// </summary>
        /// <param name="lastVertPosition">The vertical position of the last item that was placed.</param>
        /// <returns>The next vertical position that the next item should be placed at.</returns>
        private float PlaceNewPickable(float lastVertPosition)
        {
            Pickable pickableObject = MakePickable();
            Vector2 position = Vector.GetPlacableVector(lastVertPosition, 48);
            if (_obstaclePositions.Contains(position))
                position -= new Vector2(0, 48);
            pickableObject.Position = position;
            CallDeferred("add_child", pickableObject);
            lastVertPosition += 100;
            return lastVertPosition;
        }

        /// <summary>
        /// Place the pickable items randomly in the level play area.
        /// </summary>
        private void PlacePickables()
        {
            float lastVertPosition = -64f;
            for (int i = 0; i <= 8; i += 1)
                lastVertPosition = PlaceNewPickable(lastVertPosition);
        }

        /// <summary>
        /// Sets up the arcade mode for the game.
        /// </summary>
        private void SetupArcadeMode()
        {
            var gameState = this.GetCurrentState();
            _state.PackagesRemaining = gameState.GetRequiredPackages();
            _timerLevel.WaitTime = gameState.GetTimeLimit();
        }

        /// <summary>
        /// Sets up the tutorial text on the HUD.
        /// </summary>
        private void SetupTutorialText()
        {
            switch (this.GetCurrentState().GetCurrentLevel())
            {
                case 0:
                    _headsUpDisplay.SetTutorialText(HUD.TutorialText.Movement);
                    break;
                case 1:
                    _headsUpDisplay.SetTutorialText(HUD.TutorialText.ExtraTime);
                    break;
                case 2:
                    _headsUpDisplay.SetTutorialText(HUD.TutorialText.Hazards);
                    break;
                default:
                    break;
            }
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
            _timerLevel.Stop();
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
            {
                TeardownChild(child);
            }
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
        /// A callback method that runs every second when the <c>TimerTick</c> times out.
        /// </summary>
        /// <remarks>
        /// This method primarily updates the heads-up display with the remaining time.
        /// </remarks>
        private void Tick()
        {
            int timeLeft = (int)_timerLevel.TimeLeft;
            _headsUpDisplay.UpdateTimeLimit($"{timeLeft}");

            if (_timerStart.TimeLeft >= 0)
                _headsUpDisplay.UpdateStartTimer();
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
    }
}