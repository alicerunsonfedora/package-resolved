// TeleportController.cs
// Package Resolved
//
// Copyright (C) 2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;
using PackageResolved.Objects;

namespace PackageResolved.Logic
{
    /// <summary>
    /// A class that handles teleporting the player once they have reached the bottom of the map.
    /// </summary>
    public class TeleportController : Node
    {

        /// <summary>
        /// The trigger node that the player passes through to teleport to the destination.
        /// </summary>
        private readonly Area2D _source;

        /// <summary>
        /// The desination node that the player will teleport to.
        /// </summary>
        private readonly Node2D _target;

        /// <summary>
        /// Instantiates the TeleportController. 
        /// </summary>
        /// <param name="from">The trigger node that the player passes through.</param>
        /// <param name="to">The destination node that the player will telelport to.</param>
        public TeleportController(Area2D from, Node2D to)
        {
            _source = from;
            _target = to;
            _source.Connect("body_entered", this, nameof(OnBodyEntered));
        }

        /// <summary>
        /// Update the target's position to reflect the source position on the horizontal axis.
        /// </summary>
        /// <param name="sourcePosition">The position that the target's horizontal axis will be derived from.</param>
        public void UpdateTargetPosition(Vector2 sourcePosition)
        {
            _target.Position = new Vector2(sourcePosition.x, _target.Position.y);
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
            body.Position = _target.Position;
            EmitSignal(nameof(Teleported));
        }

        /// <summary>
        /// A signal that emits when a player body is teleported to the target.
        /// </summary>
        [Signal]
        public delegate void Teleported();

    }
}

