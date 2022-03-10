using Godot;
using PackageResolved.Logic;

namespace PackageResolved.UI
{
    public class HUD : Control
    {
        private Label packagesRemaining;
        private Label timeLimit;
        private Timer timer;
        private Tween tween;

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

        public void UpdatePackagesRemaining(string text)
        {
            packagesRemaining.Text = text;
        }

        public void UpdateTimeLimit(string text)
        {
            timeLimit.Text = text;
        }

        private void InstantiateOnreadyInstances()
        {
            timer = GetNode<Timer>("Timer");
            tween = GetNode<Tween>("Tween");
            packagesRemaining = GetNode<Label>("PackagesRemaining/Label");
            timeLimit = GetNode<Label>("TimeLimit/Label");
        }
    }
}