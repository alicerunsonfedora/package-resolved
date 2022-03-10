using Godot;

namespace PackageResolved.Logic
{
    public class Player : KinematicBody2D
    {
        private const int Mass = 100;

        private Vector2 MovementVector;
        private int Acceleration = 100;
        private int Friction = 100;
        private int MaxSpeed = 200;
        private int Speed = 150;

        public void SlowDown()
        {
            Acceleration = 100;
            Friction = 100;
        }

        public void SpeedUp()
        {
            Acceleration = 150;
            Friction = 50;
        }

        public override void _PhysicsProcess(float delta)
        {
            var movement = GetMovementVector();
            if (movement == Vector2.Zero)
            {
                MovementVector = MovementVector.MoveToward(Vector2.Zero, Friction * delta);
            }
            else
            {
                MovementVector = movement * Acceleration * delta * Mass;
                MovementVector = MovementVector.Clamped(MaxSpeed * Mass * delta);
            }
            MoveAndSlide(MovementVector * delta * Speed);
        }

        private Vector2 GetMovementVector()
        {
            var newVector = Vector2.One;
            newVector.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
            return newVector.Normalized();
        }

    }
}