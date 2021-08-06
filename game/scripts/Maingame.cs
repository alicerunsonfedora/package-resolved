// Maingame
// (C) 2021 Marquis Kurt.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;
using System;
using System.Collections.Generic;

namespace PackageResolved
{
    /// <summary>Documentation for <c>Maingame</c> goes here.</summary>
    public class Maingame : Node2D
    {

        private Player _player;

        private List<Vector2> _obstaclePositions = new List<Vector2>();

        public override void _Ready()
        {
            _player = GetNode("Player") as Player;
            _player.Connect("PlayerRelocated", this, "ReplaceObstacles");
            ReplaceObstacles();
        }

        public void ReplaceObstacles()
        {
            var data = (PackedScene)ResourceLoader.Load("res://scenes/Obstacle.tscn");

            int lastVerticalPosition = 0;
            bool useFastSpeed = false;

            foreach (Node2D child in GetChildren())
            {
                if (!(child is Obstacle))
                    continue;
                RemoveChild((Obstacle)child);
            }

            for (int i = 0; i < 6; i++)
            {
                int newHorizontalPosition = (int)GD.RandRange(1, 21);
                Obstacle newObstacle = (Obstacle)data.Instance();
                newObstacle.ObstacleKind = useFastSpeed ? Obstacle.Kind.Faster_Speed : Obstacle.Kind.Instant_Death;
                newObstacle.Position = new Vector2(newHorizontalPosition * 48, lastVerticalPosition);
                newObstacle.SwapTextures();
                _obstaclePositions.Add(newObstacle.Position);
                AddChild(newObstacle);
                lastVerticalPosition += 144;
                useFastSpeed = !useFastSpeed;
            }

            GD.Print(_obstaclePositions);
        }
    }
}
