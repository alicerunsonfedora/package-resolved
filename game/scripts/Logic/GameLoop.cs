using Godot;
using Godot.Collections;
using PackageResolved.UI;

namespace PackageResolved.Logic
{
    public class GameLoop : Node2D, ITeardownable
    {
        private HUD HeadsUpDisplay;
        private PackedScene HazardPacked = GD.Load("res://objects/hazard.tscn") as PackedScene;
        private Array ObstaclePositions = new Array();
        private PackedScene PickablePacked = GD.Load("res://objects/pickable.tscn") as PackedScene;
        private Player PlayerNode;
        private int RemainingPackages;
        private Area2D TeleportTrigger;
        private Node2D TeleportDestination;
        private Timer TimerTick;
        private Timer TimerLevel;

        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            TeleportTrigger.Connect("body_entered", this, "OnBodyEntered");
            TimerTick.Connect("timeout", this, "Tick");
            TimerLevel.Connect("timeout", this, "GameOver");

            GD.Randomize();
            PlaceHazards();
            PlacePickables();

            var state = GetNode<GameState>("/root/GameState");
            if (state.GetGameMode() == GameState.GameMode.Arcade)
            {
                RemainingPackages = state.GetRequiredPackages();
                TimerLevel.WaitTime = state.GetTimeLimit();
                HeadsUpDisplay.UpdatePackagesRemaining($"{RemainingPackages}");
                Tick();
                TimerLevel.Start();
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            TeleportDestination.Position = new Vector2(PlayerNode.Position.x, TeleportDestination.Position.y);
        }

        private void GameOver()
        {
            GetTree().ChangeScene("res://scenes/game_over.tscn");
        }

        private void InstantiateOnreadyInstances()
        {
            HeadsUpDisplay = GetNode<HUD>("CanvasLayer/HUD");
            PlayerNode = GetNode<Player>("Player");
            TeleportTrigger = GetNode<Area2D>("TeleportTrigger");
            TeleportDestination = GetNode<Node2D>("TeleportDestination");
            TimerTick = GetNode<Timer>("Tick");
            TimerLevel = GetNode<Timer>("Timer");
        }

        private Hazard MakeHazard()
        {
            Hazard hazard = HazardPacked.Instance() as Hazard;
            var hazardSeed = GD.RandRange(1, 20);
            if (hazardSeed < 15)
                hazard.Connect("StartedContact", this, "GameOver");
            else
            {
                hazard.Kind = Hazard.Type.WetFloor;
                hazard.Connect("StartedContact", PlayerNode, "SpeedUp");
                hazard.Connect("StoppedContact", PlayerNode, "SlowDown");
            }
            hazard.SetupHazard();
            return hazard;
        }

        private Pickable MakePickable()
        {
            Pickable pickable = PickablePacked.Instance() as Pickable;
            var pickableSeed = GD.RandRange(0, 50);
            if (pickableSeed > 40)
                pickable.Kind = Pickable.Type.PackagePlus;
            else if (pickableSeed > 30 && pickableSeed <= 39)
                pickable.Kind = Pickable.Type.TimeModifier;
            pickable.RedrawSprite();
            pickable.Connect("PickedPackage", this, "OnPickedPackage");
            pickable.Connect("PickedModifier", this, "OnPickedModifier");
            return pickable;
        }

        private void OnBodyEntered(Node2D body)
        {
            if (!(body is Player))
                return;
            body.Position = TeleportDestination.Position;
            Teardown();
            PlaceHazards();
            PlacePickables();
        }

        private void OnPickedModifier()
        {
            var state = GetNode<GameState>("/root/GameState");
            if (state.GetGameMode() == GameState.GameMode.Endless)
                return;
            var elapsedTime = TimerLevel.TimeLeft;
            TimerLevel.Stop();
            TimerLevel.WaitTime = elapsedTime + 7;
            TimerLevel.Start();
        }

        private void OnPickedPackage(int amount)
        {
            var state = GetNode<GameState>("/root/GameState");
            if (state.GetGameMode() == GameState.GameMode.Endless)
                RemainingPackages += 1;
            else
            {
                RemainingPackages -= 1;
                if (RemainingPackages <= 0)
                    GetTree().ChangeScene("res://scenes/level_success.tscn");
            }
            HeadsUpDisplay.UpdatePackagesRemaining($"{RemainingPackages}");
        }

        private void PlaceHazards()
        {
            var lastVertPosition = 300f;
            for (int i = 0; i <= 4; i += 1)
            {
                var randomXPosition = (float)GD.RandRange(-4, 8);
                if (randomXPosition == 0.0f)
                    randomXPosition += GD.Randf() > 0 ? 3 : -3;
                Hazard hazard = MakeHazard();
                hazard.Position = new Vector2(randomXPosition * 48 * 2, lastVertPosition);
                ObstaclePositions.Add(hazard.Position);
                CallDeferred("add_child", hazard);
                lastVertPosition += hazard.GetRect().Extents.y * 2;
                lastVertPosition += hazard.GetRect().Extents.y / 3;
            }
        }

        private void PlacePickables()
        {
            var lastVertPosition = -64;
            for (int i = 0; i <= 8; i += 1)
            {
                float randomXPosition = (float)GD.RandRange(-5, 7);
                var pickableObject = MakePickable();
                pickableObject.Position = new Vector2(randomXPosition * 48 * 2, lastVertPosition);
                if (ObstaclePositions.Contains(pickableObject.Position))
                    pickableObject.Position -= new Vector2(0, 48);
                CallDeferred("add_child", pickableObject);
                lastVertPosition += 50 * 2;
            }
        }

        public void Teardown()
        {
            foreach (var child in GetChildren())
            {
                if (child is ITeardownable)
                    (child as ITeardownable).Teardown();
            }
            ObstaclePositions.Clear();
        }

        private void Tick()
        {
            int timeLeft = (int)TimerLevel.TimeLeft;
            HeadsUpDisplay.UpdateTimeLimit($"{timeLeft}");
        }
    }
}