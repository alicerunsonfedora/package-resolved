// Box
// (C) 2021 Marquis Kurt.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;

namespace PackageResolved
{
    /// <summary>Documentation for <c>Box</c> goes here.</summary>
    public class Box : Area2D
    {
        ///<summary>A signal emitted when a player enters the collision space.</summary>
        [Signal]
        public delegate void PackageCollected();

        public override void _Ready()
        {
            Connect("body_entered", this, "_OnBodyEntered");
        }

        private void _OnBodyEntered(Node2D body)
        {
            if (!(body is Player))
                return;
            CollectPackage();
        }

        private void CollectPackage()
        {
            EmitSignal("PackageCollected");
            QueueFree();
        }
    }
}