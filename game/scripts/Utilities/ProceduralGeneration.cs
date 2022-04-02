// ProceduralGeneration.cs
// Package Resolved
//
// Copyright (C) 2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;
using PackageResolved.Extensions;
using PackageResolved.Logic;
using PackageResolved.Objects;

namespace PackageResolved.Utilities
{
    /// <summary>
    /// A utility class that handles procedural generation of hazards and pickable items.
    /// </summary>
    public static class ProceduralGeneration
    {
        /// <summary>
        /// Creates a hazard node that will be placed in the scene tree.
        /// </summary>
        /// <param name="gameLoop">The game loop to create the hazard in.</param>
        /// <return> An instance of <c>Hazard</c>. </return>
        /// <seealso> Hazard </seealso>
        /// <remarks>
        /// Hazards will start spawning on the third level in Arcade mode and after the first pass in Endless mode.
        /// </remarks>
        public static Hazard MakeHazard(GameLoop gameLoop)
        {
            Hazard hazard = Instancing.InstanceHazard();
            ConnectHazardToGameLoop(hazard, gameLoop);
            hazard.SetupHazard();
            return hazard;
        }

        /// <summary>
        /// Creates a pickable item node that will be placed in the scene tree.
        /// </summary>
        /// <param name="gameLoop">The game loop to create the pickable item in.</param>
        /// <return> An instance of <c>Pickable</c>. </return>
        /// <seealso> Pickable </seealso>
        /// <remarks>
        /// Timepieces will not be spawned on the first level in Arcade mode and in Endless mode.
        /// </remarks>
        public static Pickable MakePickable(GameLoop gameLoop)
        {
            var state = gameLoop.GetCurrentState();
            Pickable pickable = Instancing.InstancePickable();
            SetRandomPickableKind(state, pickable);

            pickable.RedrawSprite();
            _ = pickable.Connect("PickedPackage", gameLoop, "OnPickedPackage");
            _ = pickable.Connect("PickedModifier", gameLoop, "OnPickedModifier");
            return pickable;
        }

        /// <summary>
        /// Place a new hazard on the map.
        /// </summary>
        /// <param name="lastVertPosition">The vertical position of the last item that was placed.</param>
        /// <param name="gameLoop">The game loop to place the pickable item into.</param>
        /// <returns>The next vertical position that the next item should be placed at.</returns>
        public static float PlaceNewHazard(float lastVertPosition, GameLoop gameLoop)
        {
            Hazard hazard = MakeHazard(gameLoop);
            hazard.Position = Vector.GetPlacableVector(lastVertPosition, 96);
            _ = gameLoop.GetObstaclePositions().Add(hazard.Position);
            gameLoop.CallDeferred("add_child", hazard);
            lastVertPosition += 64;
            return lastVertPosition;
        }

        /// <summary>
        /// Place a new pickable item on the map.
        /// </summary>
        /// <param name="lastVertPosition">The vertical position of the last item that was placed.</param>
        /// <param name="gameLoop">The game loop to place the pickable item into.</param>
        /// <returns>The next vertical position that the next item should be placed at.</returns>
        public static float PlaceNewPickable(float lastVertPosition, GameLoop gameLoop)
        {
            Pickable pickableObject = MakePickable(gameLoop);
            Vector2 position = Vector.GetPlacableVector(lastVertPosition, 48);
            if (gameLoop.GetObstaclePositions().Contains(position))
                position -= new Vector2(0, 48);
            pickableObject.Position = position;
            gameLoop.CallDeferred("add_child", pickableObject);
            lastVertPosition += 100;
            return lastVertPosition;
        }

        /// <summary>
        /// Uses a random seed to connect the hazard's signals to the game loop.
        /// </summary>
        /// <param name="hazard">The hazard to connect the signals from.</param>
        /// <param name="gameLoop">The game loop to connect the signals to.</param>
        private static void ConnectHazardToGameLoop(Hazard hazard, GameLoop gameLoop)
        {
            double hazardSeed = GD.RandRange(1, 20);
            if (hazardSeed < 15)
            {
                _ = hazard.Connect("StartedContact", gameLoop, "GameOver");
                return;
            }
            hazard.Kind = Hazard.Type.WetFloor;
            _ = hazard.Connect("StartedContact", gameLoop.GetCurrentPlayer(), "SpeedUp");
            _ = hazard.Connect("StoppedContact", gameLoop.GetCurrentPlayer(), "SlowDown");
        }

        /// <summary>
        /// Sets the pickable item's kind based off of a random seed.
        /// </summary>
        /// <param name="state">The game state that will be used to determine if timepieces exist.</param>
        /// <param name="pickable">The pickable item to update it's kind to.</param>
        private static void SetRandomPickableKind(GameState state, Pickable pickable)
        {
            double seed = GD.RandRange(0, 50);
            bool timeEligible = (state.GetCurrentLevel() > 0) && (state.CurrentGameMode == GameState.GameMode.Arcade);
            if (seed > 40)
            {
                pickable.Kind = Pickable.Type.PackagePlus;
                return;
            }
            if ((seed > 30) && (seed <= 39) && (timeEligible))
            {
                pickable.Kind = Pickable.Type.TimeModifier;
                return;
            }
        }
    }
}

