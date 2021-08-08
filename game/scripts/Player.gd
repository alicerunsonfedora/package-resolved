# Player.gd
# Package Resolved - Trijam 132
# (C) 2021 Marquis Kurt.

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

class_name Player
extends KinematicBody2D

var _movement_vector: Vector2

const mass = 100
var speed = 150
var max_speed = 200
var acceleration = 100
var friction = 100

func _ready() -> void:
	pass
	
func slow_down() -> void:
	acceleration = 100
	friction = 100
	
func speed_up() -> void:
	acceleration = 150
	friction = 50
	
func _physics_process(delta: float) -> void:
	var movement = _get_movement_vector()
	if movement != Vector2.ZERO:
		_movement_vector = movement * acceleration * delta * mass
		_movement_vector = _movement_vector.clamped(max_speed * mass * delta)
	else:
		_movement_vector = _movement_vector.move_toward(Vector2.ZERO, friction * delta)
	
	move_and_slide(_movement_vector * delta * speed)

func _get_movement_vector() -> Vector2:
	var new_vector = Vector2.ONE
	new_vector.x = Input.get_action_strength("move_right") - Input.get_action_strength("move_left")
	return new_vector.normalized()
