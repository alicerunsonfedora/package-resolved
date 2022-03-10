using Godot;
using Godot.Collections;

namespace PackageResolved.Logic
{
    /// <summary> A class that manages the state of the game throughout its lifecycle. </summary>
    /// <remarks>
    /// This class is AutoLoaded as a singleton in Godot.
    /// <example>
    /// To reference the game state node in any scene: 
    /// <code>
    /// var state = GetNode&lt;GameState&gt;("/root/GameState");
    /// </code>
    /// </example>
    /// </remarks>
    public class GameState : Node
    {
        /// <summary>
        /// An enumeration that represents the different game modes available.
        /// </summary>
        public enum GameMode
        {
            /// <summary>
            /// The Arcade mode, which uses a timer-based system and a target number of packages to collect.
            /// </summary>
            Arcade,
            /// <summary>
            /// The Endless mode, which has no time limit and tests how many packages a player can collect while 
            /// surviving.
            /// </summary>
            Endless
        }

        #region Private Fields
        /// <summary>
        /// The current level that the player is playing or will play.
        /// </summary>
        private int currentLevel = 0;

        /// <summary>
        /// The current game mode for the session.
        /// </summary>
        private GameMode currentMode = GameMode.Arcade;

        /// <summary>
        /// The maximum number of levels defined in the level data.
        /// </summary>
        private int maxLevels = 0;

        /// <summary>
        /// An array containing all of the game's level data.
        /// </summary>
        /// <remarks>
        /// For each entry in the level data, there are two keys: <c>requiredPackages</c>, which defines the number of
        /// packages the player needs to collect, and <c>timeLimit</c>, which defines how much time the player will be
        /// given initially to complete the task.
        /// 
        /// To get the data corresponding to the current level, call <c>GetCurrentLevelData</c>.
        /// </remarks>
        private Array gameLevelData = new Array();

        /// <summary>
        /// Whether the music should play in the background.
        /// </summary>
        private bool musicEnabled = true;

        /// <summary>
        /// Whether sound effects should play when the player interacts with items in the game.
        /// </summary>
        private bool sfxEnabled = true;

        #endregion


        public override void _Ready()
        {
            var jsonFile = new File();
            var err = jsonFile.Open("res://levels.json", File.ModeFlags.Read);
            if (err != Error.Ok)
            {
                GD.PushError($"Error occured: {err}");
                return;
            }

            var jsonData = JSON.Parse(jsonFile.GetAsText());
            jsonFile.Close();
            if (jsonData.Error != Error.Ok)
                return;

            gameLevelData = jsonData.Result as Array;
            maxLevels = gameLevelData.Count;
        }

        /// <summary>
        /// Gets the current level that the player is playing or will play.
        /// </summary>
        public int GetCurrentLevel() => currentLevel;

        /// <summary>
        /// Gets the current game mode for the session.
        /// </summary>
        public GameMode GetGameMode() => currentMode;

        /// <summary>
        /// Gets the number of packages required for the current level.
        /// </summary>
        public int GetRequiredPackages()
        {
            if (currentLevel > gameLevelData.Count)
                return -99;

            return System.Convert.ToInt32(GetCurrentLevelData()["requiredPackages"]);
        }

        /// <summary>
        /// Gets the time limit for the current level.
        /// </summary>
        public int GetTimeLimit()
        {
            if (currentLevel > gameLevelData.Count)
                return -99;
            return System.Convert.ToInt32(GetCurrentLevelData()["timeLimit"]);
        }

        /// <summary>
        /// Returns whether the game has been completed.
        /// </summary>
        public bool IsComplete()
        {
            if (currentMode == GameMode.Endless)
                return false;
            return currentLevel > maxLevels;
        }

        /// <summary>
        /// Returns whether music should be playing in the background.
        /// </summary>
        /// <returns></returns>
        public bool IsMusicEnabled() => musicEnabled;

        /// <summary>
        /// Returns whether sound effects should play in the background as the player interacts with items in the game.
        /// </summary>
        public bool IsSoundEffectsEnabled() => sfxEnabled;

        /// <summary>
        /// Progress to the next level.
        /// </summary>
        public void Progress()
        {
            currentLevel += 1;
        }

        /// <summary>
        /// Sets the game mode for the session.
        /// </summary>
        /// <param name="mode">The new game mode that will be applied for the current session.</param>
        public void SetGameMode(GameMode mode)
        {
            currentMode = mode;
        }

        /// <summary>
        /// Sets whether the music should play in the background.
        /// </summary>
        /// <param name="enabled">Whether the music should be played in the background.</param>
        public void SetMusicEnabled(bool enabled)
        {
            musicEnabled = enabled;
            AudioServer.SetBusMute(AudioServer.GetBusIndex("Music"), !enabled);
        }

        /// <summary>
        /// Sets whether the sound effects should play when the player interacts with items.
        /// </summary>
        /// <param name="enabled">Whether the sound effects should play when the player interacts with items.</param>
        public void SetSoundEffectsEnabled(bool enabled)
        {
            sfxEnabled = enabled;
            AudioServer.SetBusMute(AudioServer.GetBusIndex("SFX"), !enabled);
        }

        /// <summary>
        /// Gets the data for the current level.
        /// </summary>
        /// <returns>A <c>Dictionary</c> containing the data about the current level.</returns>
        private Dictionary GetCurrentLevelData()
        {
            var currentLevelData = gameLevelData[GetCurrentLevel()];
            return currentLevelData as Dictionary;
        }
    }
}