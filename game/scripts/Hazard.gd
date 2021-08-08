# Hazard
# Package Resolved - Trijam 132
# (C) 2021 Marquis Kurt.

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

class_name Hazard
extends Area2D

enum Type {
	PALLETE,
	WET_FLOOR
}

export(Type) var Kind = Type.PALLETE

signal started_contact()
signal stopped_contact()

onready var shape_pallete := $"Palette"
onready var shape_wetfloor := $"WetFloor"

func _ready() -> void:
	setup_hazard()
	var _connect_err = connect("body_entered", self, "_on_body_entered")
	_connect_err = connect("body_exited", self, "_on_body_exited")
	if _connect_err != OK:
		push_error("Failed to connect - code %s" % _connect_err)
	
func setup_hazard() -> void:
	if shape_wetfloor != null:
		shape_wetfloor.disabled = Kind == Type.PALLETE
		shape_wetfloor.visible = Kind != Type.PALLETE
	if shape_pallete != null:
		shape_pallete.disabled = Kind == Type.WET_FLOOR
		shape_pallete.visible = Kind != Type.WET_FLOOR
	
func teardown() -> void:
	queue_free()

func _on_body_entered(body: Node2D) -> void:
	if not body is Player:
		return
	emit_signal("started_contact")

func _on_body_exited(body: Node2D) -> void:
	if not body is Player:
		return
	emit_signal("stopped_contact")
