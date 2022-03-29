// NodeExtension.cs
// Package Resolved
//
// (C) 2021-2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using Godot;
using PackageResolved.Logic;

namespace PackageResolved.Extensions
{
    /// <summary>
    /// A class containing extensions for Godot's <c>Node</c> class.
    /// </summary>
    public static class NodeExtension
    {
        /// <summary>
        /// Returns the GameState at the root of the scene tree.
        /// </summary>
        public static GameState GetCurrentState(this Node root) => root.GetNode<GameState>("/root/GameState");

        /// <summary>
        /// Returns the GameState at the root of the scene tree.
        /// </summary>
        public static GameState GetCurrentState(this Control root) => root.GetNode<GameState>("/root/GameState");
    }

}