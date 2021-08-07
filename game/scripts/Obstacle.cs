// Obstacle
// (C) 2021 Marquis Kurt.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;

namespace PackageResolved
{
    ///<summary>A class that represents an obstacle in the game.</summary>
    public class Obstacle : Area2D
    {
        ///<summary>An enumeration that represents the kinds of obstacles.</summary>
        public enum Kind
        {
            ///<summary>An obstacle that results in instant death on contact.</summary>
            InstantDeath,

            /// <summary>
            ///     An obstacle that results in players that pass through to speed
            ///     up and lose their friction.
            /// </summary>
            FasterSpeed
        }

        private TileMap _tilemapDeath;
        private TileMap _tilemapSpeed;

        ///<summary>The kind of obstacle this instance will be.</summary>
        [Export] public Kind ObstacleKind = Kind.InstantDeath;

        public override void _Ready()
        {
            Connect("body_entered", this, "_OnAreaEntered");
            Connect("body_exited", this, "_OnAreaExited");

            _tilemapDeath = GetNode("DeathMap") as TileMap;
            _tilemapSpeed = GetNode("FastSpeedMap") as TileMap;

            SwapTextures();
        }

        /// <summary>Loads the appropriate texture for the obstacle.</summary>
        public void SwapTextures()
        {
            if (_tilemapDeath == null || _tilemapSpeed == null)
                return;
            if (ObstacleKind == Kind.FasterSpeed)
                _tilemapSpeed.Visible = true;
            else if (ObstacleKind == Kind.InstantDeath)
                _tilemapDeath.Visible = true;
        }

        private void _OnAreaEntered(Node2D body)
        {
            if (!(body is Player))
                return;
            if (ObstacleKind == Kind.FasterSpeed)
                (body as Player).UpdateFrictionAndSpeed(true);
        }

        private void _OnAreaExited(Node2D body)
        {
            if (!(body is Player))
                return;
            if (ObstacleKind == Kind.FasterSpeed)
                (body as Player).UpdateFrictionAndSpeed(false);
        }
    }
}