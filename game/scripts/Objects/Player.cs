// Player.cs
// Package Resolved
//
// (C) 2021-2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;

namespace PackageResolved.Objects
{
    /// <summary>
    /// A class used to represent the player controller.
    /// </summary>
    public class Player : KinematicBody2D
    {
        /// <summary>
        /// The player's mass, which is used in physics calculations.
        /// </summary>
        private const int _mass = 100;

        /// <summary>
        /// A vector that described the player's current movement.
        /// </summary>
        private Vector2 _movementVector;

        /// <summary>
        /// The rate at which the player can accelerate by.
        /// </summary>
        private int _acceleration = 100;

        /// <summary>
        /// The force of friction when running.
        /// </summary>
        private int _friction = 100;

        /// <summary>
        /// The maximum speed that the player can run at.
        /// </summary>
        private readonly int _maxSpeed = 200;

        /// <summary>
        /// The player's current speed.
        /// </summary>
        private readonly int _speed = 150;

        /// <summary>
        /// Resets the acceleration and friction to their default values.
        /// </summary>
        /// <remarks>
        /// This is typically called once a player is no longer in contact with a wet floor.
        /// </remarks>
        public void SlowDown()
        {
            _acceleration = 100;
            _friction = 100;
        }

        /// <summary>
        /// Increases the acceleration and decreases the friction values.
        /// </summary>
        /// <remarks>
        /// This is typically used to speed up the player when they are in contact with a wet floor tile.
        /// </remarks>
        public void SpeedUp()
        {
            _acceleration = 150;
            _friction = 50;
        }

        /// <summary>
        /// Process the physics calculations for the player controller to allow the player to move in the right 
        /// direction.
        /// </summary>
        /// <param name="delta">The change in time since the previous frame.</param>
        public override void _PhysicsProcess(float delta)
        {
            var movement = GetMovementVector();
            if (movement == Vector2.Zero)
            {
                _movementVector = _movementVector.MoveToward(Vector2.Zero, _friction * delta);
            }
            else
            {
                _movementVector = movement * _acceleration * delta * _mass;
                _movementVector = _movementVector.Clamped(_maxSpeed * _mass * delta);
            }
            MoveAndSlide(_movementVector * delta * _speed);
        }

        /// <summary>
        /// Gets the movement vector from the player's input.
        /// </summary>
        /// <returns>
        /// A Vector2 representing the change in pressing the <c>move_left</c> and <c>move_right</c> keys
        /// </returns>
        private Vector2 GetMovementVector()
        {
            var newVector = Vector2.One;
            newVector.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
            return newVector.Normalized();
        }

    }
}