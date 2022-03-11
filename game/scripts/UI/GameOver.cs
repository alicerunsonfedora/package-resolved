using Godot;

namespace PackageResolved.UI
{
    /// <summary>
    /// A class that manages the user interface for the game over screen.
    /// </summary>
    public class GameOver : Control
    {
        /// <summary>
        /// The button that causes the game to restart when clicked.
        /// </summary>
        public Button BtnRestart;

        /// <summary>
        /// The button that allows the player to return to the main menu.
        /// </summary>
        public Button BtnQuitToMenu;

        /// <summary>
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
        public override void _Ready()
        {
            BtnRestart = GetNode<Button>("VBoxContainer/Restart");
            BtnQuitToMenu = GetNode<Button>("VBoxContainer/MainMenu");

            BtnRestart.Connect("button_up", this, "BtnPressRestart");
            BtnQuitToMenu.Connect("button_up", this, "BtnPressQuitToMenu");
        }

        /// <summary>
        /// A callback method called when the user clicks on the restart button.
        /// </summary>
        private void BtnPressRestart()
        {
            GetTree().ChangeScene("res://scenes/game_loop.tscn");
        }

        /// <summary>
        /// A callback method called when the user clicks on the main menu button.
        /// </summary>
        private void BtnPressQuitToMenu()
        {
            GetTree().ChangeScene("res://scenes/main_menu.tscn");
        }
    }
}