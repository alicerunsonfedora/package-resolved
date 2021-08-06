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
    /// <summary>A class that represents the player in the game.</summary>
    public class Player : KinematicBody2D
    {

        private const int Mass = 100;
        private const int Acceleration = 50;

        private const int MaxSpeed = 200;

        private int Friction = 200;
        private int Speed = 175;
        private Vector2 velocity = Vector2.Zero;

        public override void _Ready()
        {

        }

        /// <summary>Returns the movement vector from the player's input.</summary>
        /// <returns>A <c>Vector2</c> that represents the direction that the player
        /// move.</returns>
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
                Position = new Vector2(GlobalPosition.x, 32);
        }

        /// <summary>Update the player's friction.</summary>
        /// <param name="isSliding">Whether the player is sliding.</param>
        public void UpdateFrictionAndSpeed(bool isSliding)
        {
            Friction = isSliding ? 100 : 200;
            Speed = isSliding ? 225 : 175;
        }
    }
}
