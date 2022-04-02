// TweenExtension.cs
// Package Resolved
//
// Copyright (C) 2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using Godot;

namespace PackageResolved.Extensions
{
    /// <summary>
    /// A class that contains extension methods for the Tween node.
    /// </summary>
    public static class TweenExtension
    {
        /// <summary>
        /// Add a fadeout property interpolation to a specified object.
        /// </summary>
        /// <param name="tween">The tween that will perform the fadeout.</param>
        /// <param name="object">The object that will be faded out.</param>
        /// <param name="duration">How long the fadeout should last.</param>
        public static void Fadeout(this Tween tween, Object @object, float duration)
        {
            var trans = Tween.TransitionType.Linear;
            var ease = Tween.EaseType.InOut;
            tween.InterpolateProperty(@object, "modulate", Colors.White, Colors.Transparent, duration, trans, ease);
        }
    }
}

