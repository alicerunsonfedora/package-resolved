// Maingame
// (C) 2021 Marquis Kurt.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using Godot;

namespace PackageResolved
{
    /// <summary>Documentation for <c>Maingame</c> goes here.</summary>
    public class Maingame : Node2D
    {
        private readonly List<Vector2> _boxPositions = new List<Vector2>();

        private readonly List<Vector2> _obstaclePositions = new List<Vector2>();

        private Player _player;
        private Timer _timer;
        private Timer _tick;

        private Label _packagesRemainingLabel;
        private Label _timeRemainingLabel;

        private int _packagesRemaining = 0;

        public override void _Ready()
        {
            GameState state = (GameState) GetNode("/root/GameState");
            
            _player = (Player)GetNode("Player");
            _player.Connect("PlayerRelocated", this, "_OnPlayerRelocated");

            _timer = (Timer)GetNode("Timer");
            _timer.WaitTime = state.GetTimeLimit();
            
            _tick = (Timer)GetNode("Tick");
            _tick.Connect("timeout", this, "_Tick");
            _tick.OneShot = false;

            _packagesRemaining = (int)state.GetRequiredPackages();

            _packagesRemainingLabel = (Label)GetNode("CanvasLayer/HUD/PackagesRemaining");
            _packagesRemainingLabel.Text = $"{_packagesRemaining} packages to collect";

            _timeRemainingLabel = (Label)GetNode("CanvasLayer/HUD/TimeRemaining");
            _timeRemainingLabel.Text = $"{_timer.TimeLeft} seconds remaining";
            
            ReplaceObstacles();
            ReplacePackages();

            if (state.GameMode != GameState.Mode.Story) return;
            _tick.Start();
            _timer.Start();

        }

        ///<summary>Replace the obstacles in the level.</summary>
        public void ReplaceObstacles()
        {
            var data = (PackedScene) ResourceLoader.Load("res://scenes/Obstacle.tscn");

            var lastVerticalPosition = 48;
            var useFastSpeed = false;

            foreach (Node child in GetChildren())
            {
                if (!(child is Obstacle))
                    continue;
                child.QueueFree();
            }

            for (var i = 0; i < 5; i++)
            {
                var newHorizontalPosition = (int) GD.RandRange(1, 21);
                var newObstacle = (Obstacle) data.Instance();
                newObstacle.ObstacleKind = useFastSpeed
                    ? Obstacle.Kind.FasterSpeed
                    : Obstacle.Kind.InstantDeath;
                newObstacle.Position = new Vector2(newHorizontalPosition * 48, lastVerticalPosition);
                newObstacle.SwapTextures();
                _obstaclePositions.Add(newObstacle.Position);
                AddChild(newObstacle);

                lastVerticalPosition += 192;
                useFastSpeed = !useFastSpeed;
            }
        }

        ///<summary>Place the packages in the world.</summary>
        public void ReplacePackages()
        {
            var lastVerticalPosition = 48;
            var data = (PackedScene) ResourceLoader.Load("res://scenes/Box.tscn");

            foreach (Node child in GetChildren())
            {
                if (!(child is Box))
                    continue;
                child.QueueFree();
            }

            _boxPositions.Clear();

            for (var i = 0; i < 4; i++)
            {
                var newHorizontalPosition = (int) GD.RandRange(1, 21);
                var newBox = (Box) data.Instance();
                newBox.Connect("PackageCollected", this, "_OnPackageCollected");
                newBox.Position = new Vector2(newHorizontalPosition * 48 - 24, lastVerticalPosition);

                // NOTE: There may be a slim chance this produces a problem by repeating positions.
                if (_boxPositions.Contains(newBox.Position))
                {
                    newHorizontalPosition = (int) GD.RandRange(1, 21);
                    newBox.Position = new Vector2(newHorizontalPosition * 48 - 24, newBox.Position.y);
                }

                AddChild(newBox);
                lastVerticalPosition += 192;
            }
        }

        private void _OnPlayerRelocated()
        {
            ReplaceObstacles();
            ReplacePackages();
        }

        private void _OnPackageCollected()
        {
            _packagesRemaining--;
            _packagesRemainingLabel.Text = $"{_packagesRemaining} packages to collect";
            if (_packagesRemaining <= 0)
                _FinishLevel();
        }

        private void _Tick()
        {
            _timeRemainingLabel.Text = $"{(int)_timer.TimeLeft} seconds remaining";
        }

        private void _FinishLevel()
        {
            GameState state = (GameState) GetNode("/root/GameState");
            state.Progress();
            if (!state.GameIsComplete)
            {
                GetTree().ChangeScene("res://scenes/Maingame.tscn");
            }
        }
    }
}