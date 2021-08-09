# Pickable
# Package Resolved - Trijam 132
# (C) 2021 Marquis Kurt.

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

class_name Pickable
extends Area2D

enum Type {
	PACKAGE,
	PACKAGE_PLUS,
	TIME_MODIFIER
}

export(Type) var Kind := Type.PACKAGE setget _update_pickable_type

signal picked_package(amount)
signal picked_modifier()

onready var sprite := $Sprite as AnimatedSprite
onready var tween := $Tween as Tween

onready var audio_pickup := $"Pickup"
onready var audio_powerup := $"Powerup"

func _ready() -> void:
	var _connect_err = connect("body_entered", self, "_on_body_entered")
	_connect_err = tween.connect("tween_all_completed", self, "queue_free")
	if _connect_err != OK:
		push_error("Error occurred - code %s" % _connect_err)
	tween.interpolate_property(self, "modulate", Color.white, Color.transparent, 0.25, Tween.TRANS_CUBIC, Tween.EASE_IN_OUT)
	redraw_sprite()
		
func redraw_sprite() -> void:
	if sprite == null:
		return
	match Kind:
		Type.PACKAGE:
			sprite.animation = "Package"
		Type.PACKAGE_PLUS:
			sprite.animation = "PackagePlus"
		Type.TIME_MODIFIER:
			sprite.animation = "TimeModifier"
			
func teardown() -> void:
	tween.start()
	
func _on_body_entered(body: Node2D) -> void:
	if not body is Player:
		return
	
	match Kind:
		Type.PACKAGE:
			audio_pickup.play()
			emit_signal("picked_package", 1)
		Type.PACKAGE_PLUS:
			audio_pickup.play()
			emit_signal("picked_package", 2)
		Type.TIME_MODIFIER:
			audio_powerup.play()
			emit_signal("picked_modifier")
	teardown()

func _update_pickable_type(value) -> void:
	Kind = value
	redraw_sprite()
