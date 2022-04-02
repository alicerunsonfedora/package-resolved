// GameLevelState.cs
// Package Resolved
//
// Copyright (C) 2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;

namespace PackageResolved.Logic
{
    /// <summary>
    /// A structure that contains a game level state, which is used to keep track of player progress.
    /// </summary>
    public struct GameLevelState
    {
        /// <summary>
        /// The number of packages that the player still needs to collect to complete the level.
        /// </summary>
        public int PackagesRemaining;

        /// <summary>
        /// The timestamp of when a package was last collected.
        /// </summary>
        /// <remarks>
        /// This is commonly used to update the timepiece modifiers. The value represents what the timer was at when the
        /// player collected a package.
        /// </remarks>
        public float LastPackageTimestamp;

        /// <summary>
        /// Calcuates the amount of time to add to a timer using the time modifier algorithm.
        /// </summary>
        /// <param name="growthRate">The rate of growth; i.e., how quickly the modofier grows, exponentially.</param>
        /// <param name="timeLeft">The amount of time the player has left.</param>
        /// <returns>The amount of time to be added to the timer.</returns>
        /// <remarks>
        /// The algorithm used to determine how much time to add is defined as <c>bonusTime = packagesLeft *
        /// (lastPackageCollectTime - timeLeft) ^ (a)</c>, where <c>a</c> corresponds to the growth rate.
        /// </remarks>
        public float CalculateTimeModifier(float growthRate, float timeLeft)
        {
            var growth = PackagesRemaining * (LastPackageTimestamp - timeLeft);
            return Mathf.Pow(growth, growthRate);
        }

        /// <summary>
        /// Update the amount of packages based on the specified game mode.
        /// </summary>
        /// <param name="magnitude">The amount of packages to either add or subtract from the state.</param>
        /// <param name="mode">The game mode that determines which operation to perform.</param>
        /// <returns></returns>
        public int UpdatePackageAmount(int magnitude, GameState.GameMode mode)
        {
            if (mode == GameState.GameMode.Endless)
            {
                PackagesRemaining += magnitude;
                return PackagesRemaining;
            }
            PackagesRemaining -= magnitude;
            return PackagesRemaining;
        }
    }
}

