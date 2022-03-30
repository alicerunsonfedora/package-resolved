// Vector.cs
// Package Resolved
//
// (C) 2021-2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;

namespace PackageResolved.Utilities
{
    /// <summary>
    /// A utilities class for working with vectors.
    /// </summary>
    public static class Vector
    {
        /// <summary>
        /// Returns a random vector that describes a position for either a hazard or pickable item.
        /// </summary>
        /// <param name="lastVerticalPosition">The vertical position of the previous item placed.</param>
        /// <param name="width">The width of the item that will be placed.</param>
        /// <returns>A Vector2 that represents the position for the newly placed item.</returns>
        public static Vector2 GetPlacableVector(float lastVerticalPosition, float width)
        {
            float upperBound = 400f, lowerBound = -400f;

            RandomNumberGenerator rand = new RandomNumberGenerator();
            rand.Randomize();
            float randomXPosition = rand.RandiRange(-5, 7);

            Vector2 position = new Vector2(randomXPosition * width, lastVerticalPosition);
            if (position.x > upperBound)
                position.x = upperBound;
            else if (position.x < lowerBound)
                position.x = lowerBound;
            return position;
        }
    }
}