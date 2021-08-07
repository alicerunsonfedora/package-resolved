// Player
// (C) 2021 Marquis Kurt.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;

namespace PackageResolved
{
    /// <summary>A class that represents the player in the game.</summary>
    public class Player : KinematicBody2D
    {
        private const int Mass = 100;
        private const int Acceleration = 50;

        private const int MaxSpeed = 200;

        private int _friction = 200;
        private int _speed = 175;
        private Vector2 _velocity = Vector2.Zero;

        /// <summary>Returns the movement vector from the player's input.</summary>
        /// <returns>
        ///     A <c>Vector2</c> that represents the direction that the player
        ///     move.
        /// </returns>
        private Vector2 GetMovementVector()
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
                _velocity = currentMovementVector * Acceleration * Mass * delta;
                _velocity = _velocity.Clamped(MaxSpeed * Mass * delta);
            }
            else
            {
                _velocity = _velocity.MoveToward(Vector2.Zero, _friction * delta);
            }

            MoveAndSlide(_velocity * delta * _speed);
            if (GlobalPosition.y > 928)
            {
                Position = new Vector2(GlobalPosition.x, -650);
                EmitSignal("PlayerRelocated");
            }
        }

        /// <summary>Update the player's friction.</summary>
        /// <param name="isSliding">Whether the player is sliding.</param>
        public void UpdateFrictionAndSpeed(bool isSliding)
        {
            _friction = isSliding ? 100 : 200;
            _speed = isSliding ? 225 : 175;
        }

        /// <summary>
        ///     A signal that indicated the player has moved to the top
        ///     of the screen.
        /// </summary>
        [Signal]
        private delegate void PlayerRelocated();
    }
}