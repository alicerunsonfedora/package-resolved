// GameLevelData.cs
// Package Resolved
//
// (C) 2021-2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace PackageResolved.Logic
{
    /// <summary>
    /// A data structure that represents data for a particular level in the game.
    /// </summary>
    struct GameLevelData
    {
        /// <summary>
        /// The number of packages the player needs to collect in this level.
        /// </summary>
        public int RequiredPackages;

        /// <summary>
        /// The time limit of the level in seconds.
        /// </summary>
        public int TimeLimit;
    }
}