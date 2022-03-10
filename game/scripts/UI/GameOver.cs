using Godot;

namespace PackageResolved.UI
{
    public class GameOver : Control
    {
        public Button BtnRestart;
        public Button BtnQuitToMenu;


        public override void _Ready()
        {
            BtnRestart = GetNode<Button>("VBoxContainer/Restart");
            BtnQuitToMenu = GetNode<Button>("VBoxContainer/MainMenu");

            BtnRestart.Connect("button_up", this, "BtnPressRestart");
            BtnQuitToMenu.Connect("button_up", this, "BtnPressQuitToMenu");
        }

        private void BtnPressRestart()
        {
            GetTree().ChangeScene("res://scenes/game_loop.tscn");
        }

        private void BtnPressQuitToMenu()
        {
            GetTree().ChangeScene("res://scenes/main_menu.tscn");
        }
    }
}