using Godot;

namespace PackageResolved.Logic
{
    using SoundEffectPlayer = AudioStreamPlayer2D;

    /// <summary>
    /// A class that represents a hazard to the player.
    /// </summary>
    public class Hazard : Area2D, ITeardownable
    {
        /// <summary>
        /// An enumeration for the different types of hazards the player can encounter.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// A heavy palette. Upon contact, the player will "die", and the game ends.
            /// </summary>
            Palette,

            /// <summary>
            /// A puddle of water with wet floor signes. Upon contact, the player will temporarily gain momentum.
            /// </summary>
            WetFloor
        }

        /// <summary>
        /// The kind of hazard this instance will be. Defaults to <c>Type.Palette</c>.
        /// </summary>
        [Export]
        public Type Kind = Type.Palette;

        /// <summary>
        /// The collision shape that represents the palette.
        /// </summary>
        private CollisionShape2D ShapePalette;

        /// <summary>
        /// The collision shape that represents the wet floor.
        /// </summary>
        private CollisionShape2D ShapeWetFloor;

        /// <summary>
        /// The sound effect player that will play a sound when the player enters contact with the hazard.
        /// </summary>
        private SoundEffectPlayer AudioEffect;

        /// <summary>
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            SetupHazard();
            Connect("body_entered", this, "OnBodyEntered");
            Connect("body_exited", this, "OnBodyExited");
        }

        /// <summary>
        /// Gets the rectangle shape of the collision mask.
        /// </summary>
        /// <returns>A <c>RectangleShape2D</c> that describes the shape of the hazard.</returns>
        public RectangleShape2D GetRect()
        {
            if (ShapePalette == null || ShapeWetFloor == null)
                return new RectangleShape2D();
            return (Kind == Type.Palette ? ShapePalette.Shape : ShapeWetFloor.Shape) as RectangleShape2D;
        }

        /// <summary>
        /// Instantiate fields that reference nodes in the scene tree.
        /// </summary>
        /// <remarks>
        /// In GDScript, these fields would be marked with <c>onready</c>.
        /// </remarks>
        private void InstantiateOnreadyInstances()
        {
            ShapePalette = GetNode<CollisionShape2D>("Palette");
            ShapeWetFloor = GetNode<CollisionShape2D>("WetFloor");
            AudioEffect = GetNode<SoundEffectPlayer>("Effect");
        }

        /// <summary>
        /// A callback method that runs when a body enters the collision area.
        /// </summary>
        /// <param name="body">The body that has entered the collision area.</param>
        /// <remarks>
        /// This method will listen for when the player has made contact with the hazard, play the sound effect, and
        /// emit the <c>StartedContact</c> signal.
        /// </remarks>
        private void OnBodyEntered(Node2D body)
        {
            if (!(body is Player))
                return;
            AudioEffect.Play();
            EmitSignal("StartedContact");
        }

        /// <summary>
        /// A callback method that runs when a body exits the collision area.
        /// </summary>
        /// <param name="body">The body that has left the collision area.</param>
        /// <remarks>
        /// This method will listen for when the player has left contact with the hazard, play the sound effect, and
        /// emit the <c>StoppedContact</c> signal.
        /// </remarks>
        private void OnBodyExited(Node2D body)
        {
            if (!(body is Player))
                return;
            AudioEffect.Play();
            EmitSignal("StoppedContact");
        }

        /// <summary>
        /// Sets up the visibility and collision shape for the hazard based on the current hazard type.
        /// </summary>
        public void SetupHazard()
        {
            if (ShapeWetFloor != null)
            {
                ShapeWetFloor.Disabled = Kind == Type.Palette;
                ShapeWetFloor.Visible = Kind != Type.Palette;
            }

            if (ShapePalette != null)
            {
                ShapePalette.Disabled = Kind == Type.WetFloor;
                ShapePalette.Visible = Kind != Type.WetFloor;
            }
        }

        /// <summary>
        /// Tears down the current hazard and frees it from memory.
        /// </summary>
        public void Teardown()
        {
            QueueFree();
        }

        /// <summary>
        /// A signal emitted when a player has made contact with the hazard.
        /// </summary>
        [Signal]
        delegate void StartedContact();

        /// <summary>
        /// A signal emitted when a player has left contact with the hazard.
        /// </summary>
        [Signal]
        delegate void StoppedContact();

    }
}