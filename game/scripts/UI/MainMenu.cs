using Godot;
using PackageResolved.Logic;

namespace PackageResolved.UI
{
    public class MainMenu : Control
    {
        private Button BtnStartArcade;
        private Button BtnStartEndless;
        private CheckButton ChkMusic;
        private CheckButton ChkSfx;
        private TextureRect Helmet;

        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            ConnectSignals();
            var state = GetNode<GameState>("/root/GameState");

            ChkMusic.Pressed = state.IsMusicEnabled();
            ChkSfx.Pressed = state.IsSoundEffectsEnabled();

            if (state.IsComplete())
                Helmet.Visible = false;
        }

        private void InstantiateOnreadyInstances()
        {
            BtnStartArcade = GetNode<Button>("VBoxContainer/Start");
            BtnStartEndless = GetNode<Button>("VBoxContainer/Endless");
            ChkMusic = GetNode<CheckButton>("VBoxContainer/Music");
            ChkSfx = GetNode<CheckButton>("VBoxContainer/SFX");
            Helmet = GetNode<TextureRect>("Base/Helmet");
        }

        private void BtnPressStartArcade()
        {
            var state = GetNode<GameState>("/root/GameState");
            state.SetGameMode(GameState.GameMode.Arcade);
            GetTree().ChangeScene("res://scenes/game_loop.tscn");
        }

        private void BtnPressStartEndless()
        {
            var state = GetNode<GameState>("/root/GameState");
            state.SetGameMode(GameState.GameMode.Endless);
            GetTree().ChangeScene("res://scenes/game_loop.tscn");
        }

        private void ChkMusicToggle(bool value)
        {
            var state = GetNode<GameState>("/root/GameState");
            state.SetMusicEnabled(value);
        }

        private void ChkSfxToggle(bool value)
        {
            var state = GetNode<GameState>("/root/GameState");
            state.SetSoundEffectsEnabled(value);
        }

        private void ConnectSignals()
        {
            BtnStartArcade.Connect("button_up", this, "BtnPressStartArcade");
            BtnStartEndless.Connect("button_up", this, "BtnPressStartEndless");
            ChkMusic.Connect("toggled", this, "ChkMusicToggle");
            ChkSfx.Connect("toggled", this, "ChkSfxToggle");
        }
    }
}