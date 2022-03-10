using Godot;

namespace PackageResolved.Logic
{
    using SoundEffectPlayer = AudioStreamPlayer2D;
    public class Hazard : Area2D, ITeardownable
    {
        public enum Type
        {
            Palette,
            WetFloor
        }

        [Export]
        public Type Kind = Type.Palette;

        private CollisionShape2D ShapePalette;
        private CollisionShape2D ShapeWetFloor;
        private SoundEffectPlayer AudioEffect;

        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            SetupHazard();
            Connect("body_entered", this, "OnBodyEntered");
            Connect("body_exited", this, "OnBodyExited");
        }

        public RectangleShape2D GetRect()
        {
            if (ShapePalette == null || ShapeWetFloor == null)
                return new RectangleShape2D();
            return (Kind == Type.Palette ? ShapePalette.Shape : ShapeWetFloor.Shape) as RectangleShape2D;
        }

        private void InstantiateOnreadyInstances()
        {
            ShapePalette = GetNode<CollisionShape2D>("Palette");
            ShapeWetFloor = GetNode<CollisionShape2D>("WetFloor");
            AudioEffect = GetNode<SoundEffectPlayer>("Effect");
        }

        private void OnBodyEntered(Node2D body)
        {
            if (!(body is Player))
                return;
            AudioEffect.Play();
            EmitSignal("StartedContact");
        }


        private void OnBodyExited(Node2D body)
        {
            if (!(body is Player))
                return;
            AudioEffect.Play();
            EmitSignal("StoppedContact");
        }

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

        public void Teardown()
        {
            QueueFree();
        }

        [Signal]
        delegate void StartedContact();

        [Signal]
        delegate void StoppedContact();

    }
}