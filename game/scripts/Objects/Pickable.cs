// Pickable.cs
// Package Resolved
//
// (C) 2021-2022 Marquis Kurt.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Godot;
using PackageResolved.Extensions;
using PackageResolved.Logic;

namespace PackageResolved.Objects
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
        private SoundEffectPlayer _audioPickup;

        /// <summary>
        /// The sound effect player that plays the "powerup" sound effect.
        /// </summary>
        private SoundEffectPlayer _audioPowerup;

        /// <summary>
        /// The type of pickable item for the current instance.
        /// </summary>
        private Type IKind = Type.Package;

        /// <summary>
        /// The animate sprite of the pickable item.
        /// </summary>
        private AnimatedSprite _sprite;

        /// <summary>
        /// A tween animation used to fade the item in and out of the scene.
        /// </summary>
        private Tween _tween;

        /// <summary>
        /// Instantiate the scene after entering the scene tree.
        /// </summary>
        public override void _Ready()
        {
            InstantiateOnreadyInstances();
            Connect("body_entered", this, nameof(OnBodyEntered));

            _tween.Connect("tween_all_completed", this, "queue_free");
            AnimatePickable();
            AnimatePowerupText();

            RedrawSprite();
        }

        /// <summary>
        /// Animate the pickable's fade out and the lighting.
        /// </summary>
        private void AnimatePickable()
        {
            var trans = Tween.TransitionType.Cubic;
            var ease = Tween.EaseType.InOut;
            _tween.Fadeout(_sprite, 0.25f);
            _tween.InterpolateProperty(GetNode("Sprite/Light"), "energy", 0.7, 0, 0.25f, trans, ease);
        }

        /// <summary>
        /// Animates the powerup modifier text so that it appears.
        /// </summary>
        private void AnimatePowerupText()
        {
            var hint = GetNode<Control>("Hint");
            var origin = hint.RectPosition;
            var hintPositionOffset = hint.RectPosition + (Vector2.Up * 16);
            var trans = Tween.TransitionType.Cubic;
            var ease = Tween.EaseType.InOut;

            _tween.InterpolateProperty(hint, "rect_position", origin, hintPositionOffset, 1.25f, trans, ease);
            _tween.Fadeout(hint, 1.25f);
        }

        /// <summary>
        /// Instantiate fields that reference nodes in the scene tree.
        /// </summary>
        /// <remarks>
        /// In GDScript, these fields would be marked with <c>onready</c>.
        /// </remarks>
        private void InstantiateOnreadyInstances()
        {
            _sprite = GetNode<AnimatedSprite>("Sprite");
            _tween = GetNode<Tween>("Tween");
            _audioPickup = GetNode<SoundEffectPlayer>("Pickup");
            _audioPowerup = GetNode<SoundEffectPlayer>("Powerup");
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

            var hint = GetNode<Control>("Hint");
            switch (IKind)
            {
                case Type.Package:
                    _audioPickup.Play();
                    hint.Visible = true;
                    EmitSignal("PickedPackage", 1);
                    break;
                case Type.PackagePlus:
                    _audioPickup.Play();
                    hint.Visible = true;
                    EmitSignal("PickedPackage", 2);
                    break;
                case Type.TimeModifier:
                    _audioPowerup.Play();
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
            if (_sprite == null)
                return;
            var hintLbl = GetNode<Label>("Hint/Label");
            switch (IKind)
            {
                case Type.Package:
                    _sprite.Animation = "Package";
                    hintLbl.Text = "+1";
                    break;
                case Type.PackagePlus:
                    _sprite.Animation = "PackagePlus";
                    hintLbl.Text = "+2";
                    break;
                case Type.TimeModifier:
                    _sprite.Animation = "TimeModifier";
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
            _tween.Start();
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