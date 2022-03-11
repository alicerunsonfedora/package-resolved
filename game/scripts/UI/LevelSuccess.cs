using Godot;
using PackageResolved.Logic;

namespace PackageResolved.UI
{
    /// <summary>
    /// A class that represents the level success user interface screen.
    /// </summary>
    public class LevelSuccess : Control
    {
        /// <summary>
        /// A button that allows the player to restart the game or advance to the next level.
        /// </summary>
        private Button BtnAdvance;

        /// <summary>
        /// A button that allows the player to go to the main menu.
        /// </summary>
        private Button BtnQuitToMenu;

        /// <summary>
        /// The texture rectangle that contains the sprite image for the bird helmet.
        /// </summary>
        private TextureRect Helmet;

        /// <summary>
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            BtnAdvance.Connect("button_up", this, "BtnPressRestart");
            BtnQuitToMenu.Connect("button_up", this, "BtnPressQuitToMenu");
            HideHelmetAndAdvanceButton();
        }

        /// <summary>
        /// A callback method called when the player clicks on the advance button.
        /// </summary>
        private void BtnPressRestart()
        {
            GetTree().ChangeScene("res://scenes/game_loop.tscn");
        }

        /// <summary>
        /// A callback method called when the player clicks on the button to go to the main menu.
        /// </summary>
        private void BtnPressQuitToMenu()
        {
            GetTree().ChangeScene("res://scenes/main_menu.tscn");
        }

        /// <summary>
        /// Determine if the helmet should be hidden and, if necessary, hide it from the screen.
        /// </summary>
        /// <remarks>
        /// The helmet will usually be hidden when the player completes the game.
        /// </remarks>
        private void HideHelmetAndAdvanceButton()
        {
            var state = GetNode<GameState>("/root/GameState");
            state.Progress();
            if (!state.IsComplete())
                return;
            BtnAdvance.Visible = false;
            Helmet.Visible = false;
        }

        /// <summary>
        /// Instantiate fields that reference nodes in the scene tree.
        /// </summary>
        /// <remarks>
        /// In GDScript, these fields would be marked with <c>onready</c>.
        /// </remarks>
        private void InstantiateOnreadyInstances()
        {
            BtnAdvance = GetNode<Button>("VBoxContainer/Restart");
            BtnQuitToMenu = GetNode<Button>("VBoxContainer/MainMenu");
            Helmet = GetNode<TextureRect>("Panel/Base/Helmet");
        }

    }
}