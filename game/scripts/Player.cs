// Player
// (C) 2021 Marquis Kurt.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;
using System;

namespace PackageDelivered
{
    /// <summary>Documentation for <c>Player</c> goes here.</summary>
    public class Player : KinematicBody2D
    {

        private const int Mass = 100;
        private const int Acceleration = 50;

        private const int Speed = 175;
        private const int MaxSpeed = 200;

        private const int Friction = 200;
        private Vector2 velocity = Vector2.Zero;

        public override void _Ready()
        {

        }

        public Vector2 GetMovementVector()
        {
            var moveVector = Vector2.Zero;
            moveVector.x = Input.GetActionStrength("moveRight") - Input.GetActionStrength("moveLeft");
            moveVector.y = 1;
            return moveVector.Normalized();
        }

        public override void _PhysicsProcess(float delta)
        {
            var currentMovementVector = GetMovementVector();
            if (currentMovementVector != Vector2.Zero)
            {
                velocity = currentMovementVector * Acceleration * Mass * delta;
                velocity = velocity.Clamped(MaxSpeed * Mass * delta);
            }
            else
            {
                velocity = velocity.MoveToward(Vector2.Zero, Friction * delta);
            }
            MoveAndSlide(velocity * delta * Speed);
            if (GlobalPosition.y > 600)
                SetPosition(new Vector2(GlobalPosition.x, 32));
        }
    }
}
