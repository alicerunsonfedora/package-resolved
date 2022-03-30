// Instancing.cs
// Package Resolved
//
// (C) 2021-2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;
using PackageResolved.Objects;

namespace PackageResolved.Utilities
{
    /// <summary>
    /// A utilities class that instances objects used in the game.
    /// </summary>
    public static class Instancing
    {
        /// <summary>
        /// Creates an instance of a Hazard object.
        /// </summary>
        public static Hazard InstanceHazard()
        {
            PackedScene instance = GD.Load<PackedScene>("res://objects/hazard.tscn");
            return instance.Instance() as Hazard;
        }

        /// <summary>
        /// Creates an instance of a Pickable object.
        /// </summary>
        public static Pickable InstancePickable()
        {
            PackedScene instance = GD.Load<PackedScene>("res://objects/pickable.tscn");
            return instance.Instance() as Pickable;
        }
    }
}
