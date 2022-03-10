using Godot;

namespace PackageResolved.Logic
{
    using SoundEffectPlayer = AudioStreamPlayer2D;
    public class Pickable : Area2D, ITeardownable
    {
        public enum Type
        {
            Package,
            PackagePlus,
            TimeModifier
        }

        [Export]
        public Type Kind
        {
            get => IKind;
            set
            {
                IKind = value;
                RedrawSprite();
            }
        }

        private SoundEffectPlayer AudioPickup;
        private SoundEffectPlayer AudioPowerup;
        private Type IKind = Type.Package;
        private AnimatedSprite Sprite;
        private Tween ITween;

        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            Connect("body_entered", this, "OnBodyEntered");
            ITween.Connect("tween_all_completed", this, "queue_free");
            ITween.InterpolateProperty(
                this,
                "modulate",
                Colors.White,
                Colors.Transparent,
                0.25f,
                Tween.TransitionType.Cubic,
                Tween.EaseType.InOut
            );
            RedrawSprite();
        }

        private void InstantiateOnreadyInstances()
        {
            Sprite = GetNode<AnimatedSprite>("Sprite");
            ITween = GetNode<Tween>("Tween");
            AudioPickup = GetNode<SoundEffectPlayer>("Pickup");
            AudioPowerup = GetNode<SoundEffectPlayer>("Powerup");
        }

        private void OnBodyEntered(Node2D body)
        {
            if (!(body is Player))
                return;
            switch (IKind)
            {
                case Type.Package:
                    AudioPickup.Play();
                    EmitSignal("PickedPackage", 1);
                    break;
                case Type.PackagePlus:
                    AudioPickup.Play();
                    EmitSignal("PickedPackage", 2);
                    break;
                case Type.TimeModifier:
                    AudioPowerup.Play();
                    EmitSignal("PickedModifier");
                    break;
            }
        }

        public void RedrawSprite()
        {
            if (Sprite == null)
                return;
            switch (IKind)
            {
                case Type.Package:
                    Sprite.Animation = "Package";
                    break;
                case Type.PackagePlus:
                    Sprite.Animation = "PackagePlus";
                    break;
                case Type.TimeModifier:
                    Sprite.Animation = "TimeModifier";
                    break;
            }
        }

        public void Teardown()
        {
            ITween.Start();
        }

        [Signal]
        delegate void PickedModifier();

        [Signal]
        delegate void PickedPackage(int score);
    }
}