using Godot;

namespace PackageResolved.Logic
{
    using SoundEffectPlayer = AudioStreamPlayer2D;

    /// <summary>
    /// A class that represents a pickable item the player can pick up upon contact.
    /// </summary>
    public class Pickable : Area2D, ITeardownable
    {
        /// <summary>
        /// An enumeration representing the different types of items the player can pick up.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// A single package.
            /// </summary>
            Package,

            /// <summary>
            /// A package that represents two packages.
            /// </summary>
            PackagePlus,

            /// <summary>
            /// A modifier that adds time to the time remaining, extending the lifetime of the level.
            /// </summary>
            TimeModifier
        }


        /// <summary>
        /// The type of pickable item the current instance is.
        /// </summary>
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

        /// <summary>
        /// The sound effect player that plays a pick up sound.
        /// </summary>
        private SoundEffectPlayer AudioPickup;

        /// <summary>
        /// The sound effect player that plays the "powerup" sound effect.
        /// </summary>
        private SoundEffectPlayer AudioPowerup;

        /// <summary>
        /// The type of pickable item for the current instance.
        /// </summary>
        private Type IKind = Type.Package;

        /// <summary>
        /// The animate sprite of the pickable item.
        /// </summary>
        private AnimatedSprite Sprite;

        /// <summary>
        /// A tween animation used to fade the item in and out of the scene.
        /// </summary>
        private Tween ITween;

        /// <summary>
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
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

        /// <summary>
        /// Instantiate fields that reference nodes in the scene tree.
        /// </summary>
        /// <remarks>
        /// In GDScript, these fields would be marked with <c>onready</c>.
        /// </remarks>
        private void InstantiateOnreadyInstances()
        {
            Sprite = GetNode<AnimatedSprite>("Sprite");
            ITween = GetNode<Tween>("Tween");
            AudioPickup = GetNode<SoundEffectPlayer>("Pickup");
            AudioPowerup = GetNode<SoundEffectPlayer>("Powerup");
        }

        /// <summary>
        /// A callback method that executes when a body enters contact with the item.
        /// </summary>
        /// <param name="body">The body that has entered contact with the item</param>
        /// <remarks>
        /// This method will listen for when a player touches the item and gives them the appropriate item action on
        /// contact. The <c>PickedPackage</c> or <c>PickedModifier</c> signal will be emitted upon contact. 
        /// </remarks>
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
            Teardown();
        }

        /// <summary>
        /// Redraws the pickable item's sprite based on its type.
        /// </summary>
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

        /// <summary>
        /// Tears down the current instance of the pickable item.
        /// </summary>
        /// <remarks>
        /// This method will start the fade out animation and then free itself from memory.
        /// </remarks>
        public void Teardown()
        {
            ITween.Start();
        }

        /// <summary>
        /// A signal emitted when the player picks up a time modifier.
        /// </summary>
        [Signal]
        delegate void PickedModifier();


        /// <summary>
        /// A signal emitted when the player picks up a package.
        /// </summary>
        /// <param name="score">The number of packages the package represents.</param>
        [Signal]
        delegate void PickedPackage(int score);
    }
}