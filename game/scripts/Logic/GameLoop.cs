// GameLoop.cs
// Package Resolved
//
// (C) 2021-2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;
using Godot.Collections;
using PackageResolved.Objects;
using PackageResolved.UI;

namespace PackageResolved.Logic
{
    /// <summary>
    /// The primary game loop.
    /// </summary>
    public class GameLoop : Node2D, ITeardownable
    {
        /// <summary>
        /// The heads-up display that shows over the main level.
        /// </summary>
        private HUD _headsUpDisplay;

        /// <summary>
        /// The packed scene information to construct a hazard.
        /// </summary>
        private readonly PackedScene _hazardPackedScene = GD.Load("res://objects/hazard.tscn") as PackedScene;

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
        /// The packed scene information to construct a pickable item.
        /// </summary>
        private readonly PackedScene _pickablePacked = GD.Load("res://objects/pickable.tscn") as PackedScene;

        /// <summary>
        /// The player controller used in the level.
        /// </summary>
        private Player _playerNode;

        /// <summary>
        /// The number of packages the player still needs to collect.
        /// </summary>
        private int _remainingPackages;

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
            _teleportTrigger.Connect("body_entered", this, nameof(OnBodyEntered));
            _timerTick.Connect("timeout", this, nameof(Tick));
            _timerLevel.Connect("timeout", this, nameof(GameOver));

            GD.Randomize();
            PlaceHazards();
            PlacePickables();

            var state = GetNode<GameState>("/root/GameState");
            if (state.GetGameMode() == GameState.GameMode.Arcade)
            {
                _remainingPackages = state.GetRequiredPackages();
                _timerLevel.WaitTime = state.GetTimeLimit();
                _headsUpDisplay.UpdatePackagesRemaining($"{_remainingPackages}");
                Tick();
                _timerLevel.Start();
            }
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
        /// Indicate the game is over and show the game over screen.
        /// </summary>
        private void GameOver()
        {
            var state = GetNode<GameState>("/root/GameState");
            state.UpdatePreviousRun(state.GetRequiredPackages() - _remainingPackages, (int)_timerLevel.WaitTime);
            GetTree().ChangeScene("res://scenes/game_over.tscn");
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
            _timerTick = GetNode<Timer>("Tick");
            _timerLevel = GetNode<Timer>("Timer");
        }

        /// <summary>
        /// Creates a hazard node that will be placed in the scene tree.
        /// </summary>
        /// <return>
        /// An instance of <c>Hazard</c>. </return>
        /// <seealso> Hazard </seealso>
        private Hazard MakeHazard()
        {
            Hazard hazard = _hazardPackedScene.Instance() as Hazard;
            var hazardSeed = GD.RandRange(1, 20);
            if (hazardSeed < 15)
                hazard.Connect("StartedContact", this, "GameOver");
            else
            {
                hazard.Kind = Hazard.Type.WetFloor;
                hazard.Connect("StartedContact", _playerNode, "SpeedUp");
                hazard.Connect("StoppedContact", _playerNode, "SlowDown");
            }
            hazard.SetupHazard();
            return hazard;
        }

        /// <summary>
        /// Creates a pickable item node that will be placed in the scene tree.
        /// </summary>
        /// <return> An instance of <c>Pickable</c>. </return>
        /// <seealso> Pickable </seealso>
        private Pickable MakePickable()
        {
            Pickable pickable = _pickablePacked.Instance() as Pickable;
            var pickableSeed = GD.RandRange(0, 50);
            if (pickableSeed > 40)
                pickable.Kind = Pickable.Type.PackagePlus;
            else if (pickableSeed > 30 && pickableSeed <= 39)
                pickable.Kind = Pickable.Type.TimeModifier;
            pickable.RedrawSprite();
            pickable.Connect("PickedPackage", this, "OnPickedPackage");
            pickable.Connect("PickedModifier", this, "OnPickedModifier");
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
            PlaceHazards();
            PlacePickables();
        }

        /// <summary>
        /// A callback method that runs when the user picks up a pickable item marked as a 
        /// time modifier.
        /// </summary>
        /// <remarks>
        /// This method will reset the timer to add the additional time that the modifier will
        /// provide.
        /// </remarks>
        private void OnPickedModifier()
        {
            var state = GetNode<GameState>("/root/GameState");
            if (state.GetGameMode() == GameState.GameMode.Endless)
                return;
            var elapsedTime = _timerLevel.TimeLeft;
            _timerLevel.Stop();
            _timerLevel.WaitTime = elapsedTime + 7;
            _timerLevel.Start();
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
            var state = GetNode<GameState>("/root/GameState");
            if (state.GetGameMode() == GameState.GameMode.Endless)
                _remainingPackages += amount;
            else
            {
                _remainingPackages -= amount;
                if (_remainingPackages <= 0)
                    GetTree().ChangeScene("res://scenes/level_success.tscn");
            }
            _headsUpDisplay.UpdatePackagesRemaining($"{_remainingPackages}");
        }

        /// <summary>
        /// Place the hazards randomly in the level play area.
        /// </summary>
        private void PlaceHazards()
        {
            var lastVertPosition = 300f;
            for (int i = 0; i <= 4; i += 1)
            {
                var randomXPosition = (float)GD.RandRange(-4, 8);
                if (randomXPosition == 0.0f)
                    randomXPosition += GD.Randf() > 0 ? 3 : -3;
                Hazard hazard = MakeHazard();
                hazard.Position = new Vector2(randomXPosition * 48 * 2, lastVertPosition);
                _obstaclePositions.Add(hazard.Position);
                CallDeferred("add_child", hazard);
                lastVertPosition += hazard.GetRect().Extents.y * 2;
                lastVertPosition += hazard.GetRect().Extents.y / 3;
            }
        }

        /// <summary>
        /// Place the pickable items randomly in the level play area.
        /// </summary>
        private void PlacePickables()
        {
            var lastVertPosition = -64;
            for (int i = 0; i <= 8; i += 1)
            {
                float randomXPosition = (float)GD.RandRange(-5, 7);
                var pickableObject = MakePickable();
                pickableObject.Position = new Vector2(randomXPosition * 48 * 2, lastVertPosition);
                if (_obstaclePositions.Contains(pickableObject.Position))
                    pickableObject.Position -= new Vector2(0, 48);
                CallDeferred("add_child", pickableObject);
                lastVertPosition += 50 * 2;
            }
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
                if (child is ITeardownable)
                    (child as ITeardownable).Teardown();
            }
            _obstaclePositions.Clear();
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
        }
    }
}