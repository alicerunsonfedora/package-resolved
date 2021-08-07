// GameState
// (C) 2021 Marquis Kurt.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

namespace PackageResolved
{
    /// <summary>A singleton class that manages the game's state.</summary>
    public class GameState : Node
    {
        public enum Mode
        {
            Story,
            Endless
        }

        private int _currentLevel;

        private Array _levelList = new Array();
        private int _maxLevels = 0;

        public Mode GameMode = Mode.Story;

        public bool GameIsComplete => _currentLevel > _maxLevels;

        public override void _Ready()
        {
            var jsonFile = new File();
            jsonFile.Open("res://levels.json", File.ModeFlags.Read);
            var resultingData = JSON.Parse(jsonFile.GetAsText());
            jsonFile.Close();
            _levelList = resultingData.Result as Array;
            
            if (_levelList != null)
                _maxLevels = _levelList.Count;
        }

        /// <summary>
        /// Advance to the next level.
        /// </summary>
        public void Progress()
        {
            _currentLevel += 1;
        }

        /// <summary>
        /// Returns the number of packages required for the current level.
        /// </summary>
        public float GetRequiredPackages()
        {
            if (_currentLevel > _maxLevels) return 0;
            Dictionary currentLevelData = (Dictionary)_levelList[_currentLevel];
            return (float)currentLevelData["requiredPackages"];
        }

        /// <summary>
        /// Returns the time limit in seconds.
        /// </summary>
        public float GetTimeLimit()
        {
            if (_currentLevel > _maxLevels) return 0;
            Dictionary currentLevelData = (Dictionary)_levelList[_currentLevel];
            return (float)currentLevelData["timeLimit"];
        }
    }
}