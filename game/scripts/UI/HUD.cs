using Godot;
using PackageResolved.Logic;

namespace PackageResolved.UI
{
    /// <summary>
    /// A class that controls the user interface for the heads-up display.
    /// </summary>
    public class HUD : Control
    {
        /// <summary>
        /// A label that displays the number of packages remaining.
        /// </summary>
        private Label packagesRemaining;

        /// <summary>
        /// A label that displays the time remaining.
        /// </summary>
        private Label timeLimit;

        /// <summary>
        /// A timer used to time how long to display the tutorial screens for.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// A tween animation node used to fade out the tutorial screen.
        /// </summary>
        private Tween tween;

        /// <summary>
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            var introLabel = GetNode<Control>("IntroLabel");
            var state = GetNode<GameState>("/root/GameState");
            if (state.GetCurrentLevel() > 0 || state.GetGameMode() == GameState.GameMode.Endless)
                introLabel.Visible = false;
            else
            {
                tween.InterpolateProperty(
                    introLabel,
                    "modulate",
                    Colors.White,
                    Colors.Transparent,
                    0.5f,
                    Tween.TransitionType.Linear,
                    Tween.EaseType.InOut
                );
                timer.Connect("timeout", tween, "start");
                timer.Start();
            }
        }

        /// <summary>
        /// Update the packages remaining label.
        /// </summary>
        /// <param name="text">The number of packages remaining as a string value.</param>
        public void UpdatePackagesRemaining(string text)
        {
            packagesRemaining.Text = text;
        }

        /// <summary>
        /// Update the time limit label.
        /// </summary>
        /// <param name="text">The remaining time as a string value.</param>
        public void UpdateTimeLimit(string text)
        {
            timeLimit.Text = text;
        }


        /// <summary>
        /// Instantiate fields that reference nodes in the scene tree.
        /// </summary>
        /// <remarks>
        /// In GDScript, these fields would be marked with <c>onready</c>.
        /// </remarks>
        private void InstantiateOnreadyInstances()
        {
            timer = GetNode<Timer>("Timer");
            tween = GetNode<Tween>("Tween");
            packagesRemaining = GetNode<Label>("PackagesRemaining/Label");
            timeLimit = GetNode<Label>("TimeLimit/Label");
        }
    }
}