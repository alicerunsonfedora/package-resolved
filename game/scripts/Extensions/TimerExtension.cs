// TimerExtension.cs
// Package Resolved
//
// (C) 2021-2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;

namespace PackageResolved.Extensions
{
    /// <summary>
    /// A class that contains extension methods for the Godot Timer class.
    /// </summary>
    public static class TimerExtension
    {
        /// <summary>
        /// Adds time to a specified timer and restarts the timer to that time.
        /// </summary>
        /// <param name="timer">The timer to add time to.</param>
        /// <param name="time">The number of seconds to add to the timer.</param>
        /// <returns>The sum of the new time and the original elapsed time.</returns>
        public static float AddTime(this Timer timer, float time)
        {
            float elapsedTime = timer.TimeLeft;
            timer.Stop();
            timer.WaitTime = elapsedTime + time;
            timer.Start();
            return elapsedTime + time;
        }
    }
}