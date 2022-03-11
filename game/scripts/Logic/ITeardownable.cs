// ITeardownable.cs
// Package Resolved
//
// (C) 2021-2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace PackageResolved.Logic
{
    /// <summary>
    /// An interface that indicates that the class has a teardown function that should be called when being 
    /// deinitialized from the scene tree.
    /// </summary>
    public interface ITeardownable
    {
        /// <summary>
        /// Tears down the class to either be freed from memory or be marked as ready for reinitialization.
        /// </summary>
        void Teardown();
    }
}