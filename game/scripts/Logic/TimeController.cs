// GameTimeController.cs
// Package Resolved
//
// Copyright (C) 2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;
using PackageResolved.Extensions;
using PackageResolved.Objects;

namespace PackageResolved.Logic
{
    /// <summary>
    /// A class that handles timing events with the starting countdown, the ticker, and the game's level timer.
    /// </summary>
    public class TimeController
    {
        /// <summary>
        /// The timer responsible for the starting countdown.
        /// </summary>
        private readonly Timer _start;

        /// <summary>
        /// The timer that fires off every second to allow for methods to execute on every second.
        /// </summary>
        private readonly Timer _tick;

        /// <summary>
        /// The timer that controls how much time the player has before the level ends.
        /// </summary>
        private readonly Timer _loop;

        /// <summary>
        /// Instantiates the GameTimeController.
        /// </summary>
        /// <param name="start">The timer that holds the starting countdown.</param>
        /// <param name="tick">The timer that executes every second.</param>
        /// <param name="loop">The timer that controls how much time the player has.</param>
        public TimeController(Timer start, Timer tick, Timer loop)
        {
            _start = start;
            _tick = tick;
            _loop = loop;

            _start.Connect("timeout", _loop, "start");
        }

        /// <summary>
        /// Adds time to the level timer.
        /// </summary>
        /// <param name="extraTime">The amount of time to add to the timer.</param>
        public void AddTimeToLimit(float extraTime)
        {
            _loop.AddTime(extraTime);
        }

        /// <summary>
        /// Release the player from blocked movement when the starting countdown finishes.
        /// </summary>
        /// <param name="player">The player node to release the block from.</param>
        public void ConnectPlayerLock(Player player)
        {
            _start.Connect("timeout", player, nameof(player.UnblockMovement));
        }

        /// <summary>
        /// Connect a game loop's tick function to the timer that fire off every second.
        /// </summary>
        /// <param name="gameLoop">The game loop that will be connected to the ticker.</param>
        public void ConnectTick(GameLoop gameLoop)
        {
            _tick.Connect("timeout", gameLoop, nameof(gameLoop.Tick));
        }

        /// <summary>
        /// Connect the game loop's GameOver method to the game's level timer.
        /// </summary>
        /// <param name="gameLoop">The game loop that will handle the game over once the timer level times out.</param>
        public void ConnectGameOver(GameLoop gameLoop)
        {
            if (gameLoop.GetCurrentState().CurrentGameMode != GameState.GameMode.Arcade)
                return;
            _loop.Connect("timeout", gameLoop, nameof(gameLoop.GameOver));
        }

        /// <summary>
        /// Returns the time left on the starting countdown.
        /// </summary>
        public float GetStartCountdown() => _start.TimeLeft;

        /// <summary>
        /// Returns the time left on the game's level timer.
        /// </summary>
        public float GetTimeLeft() => _loop.TimeLeft;

        /// <summary>
        /// Set the amount of time left on the game's level timer.
        /// </summary>
        /// <param name="limit">The time limit to impose on the level timer.</param>
        public void SetTimeLimit(float limit)
        {
            _loop.WaitTime = limit;
        }

        /// <summary>
        /// Stop the game's level timer from ticking.
        /// </summary>
        public void StopLoop()
        {
            _loop.Stop();
        }
    }
}

