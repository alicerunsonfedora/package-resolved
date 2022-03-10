using Godot;
using PackageResolved.Logic;

namespace PackageResolved.UI
{
    public class LevelSuccess : Control
    {
        private Button BtnRestart;
        private Button BtnQuitToMenu;

        private TextureRect Helmet;

        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            BtnRestart.Connect("button_up", this, "BtnPressRestart");
            BtnQuitToMenu.Connect("button_up", this, "BtnPressQuitToMenu");
            HideHelmetAndRestart();
        }

        private void BtnPressRestart()
        {
            GetTree().ChangeScene("res://scenes/game_loop.tscn");
        }

        private void BtnPressQuitToMenu()
        {
            GetTree().ChangeScene("res://scenes/main_menu.tscn");
        }

        private void HideHelmetAndRestart()
        {
            var state = GetNode<GameState>("/root/GameState");
            state.Progress();
            if (!state.IsComplete())
                return;
            BtnRestart.Visible = false;
            Helmet.Visible = false;
        }

        private void InstantiateOnreadyInstances()
        {
            BtnRestart = GetNode<Button>("VBoxContainer/Restart");
            BtnQuitToMenu = GetNode<Button>("VBoxContainer/MainMenu");
            Helmet = GetNode<TextureRect>("Panel/Base/Helmet");
        }

    }
}