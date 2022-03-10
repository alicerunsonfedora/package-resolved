using Godot;
using Godot.Collections;

namespace PackageResolved.Logic
{
    public class GameState : Node
    {
        public enum GameMode
        {
            Arcade,
            Endless
        }

        #region Private Fields
        private int currentLevel = 0;
        private GameMode currentMode = GameMode.Arcade;
        private int maxLevels = 0;
        private Array gameLevelData = new Array();
        private bool musicEnabled = true;
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

        public int GetCurrentLevel() => currentLevel;
        public GameMode GetGameMode() => currentMode;

        public int GetRequiredPackages()
        {
            if (currentLevel > gameLevelData.Count)
                return -99;

            return System.Convert.ToInt32(GetCurrentLevelData()["requiredPackages"]);
        }

        public int GetTimeLimit()
        {
            if (currentLevel > gameLevelData.Count)
                return -99;
            return System.Convert.ToInt32(GetCurrentLevelData()["timeLimit"]);
        }

        public bool IsComplete()
        {
            if (currentMode == GameMode.Endless)
                return false;
            return currentLevel > maxLevels;
        }

        public bool IsMusicEnabled() => musicEnabled;
        public bool IsSoundEffectsEnabled() => sfxEnabled;

        public void Progress()
        {
            currentLevel += 1;
        }

        public void SetGameMode(GameMode mode)
        {
            currentMode = mode;
        }

        public void SetMusicEnabled(bool enabled)
        {
            musicEnabled = enabled;
            AudioServer.SetBusMute(AudioServer.GetBusIndex("Music"), !enabled);
        }
        public void SetSoundEffectsEnabled(bool enabled)
        {
            sfxEnabled = enabled;
            AudioServer.SetBusMute(AudioServer.GetBusIndex("SFX"), !enabled);
        }

        private Dictionary GetCurrentLevelData()
        {
            var currentLevelData = gameLevelData[GetCurrentLevel()];
            return currentLevelData as Dictionary;
        }
    }
}