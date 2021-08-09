# GameLoop.gd
# Package Resolved - Trijam 132
# (C) 2021 Marquis Kurt.

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

extends Node2D

onready var hud := $"CanvasLayer/HUD" as HUD
onready var player := $"Player" as Player
onready var teleport_trigger := $"TeleportTrigger" as Area2D
onready var teleport_destination := $"TeleportDestination" as Node2D
onready var timer_tick := $"Tick" as Timer
onready var timer_level := $"Timer" as Timer

onready var pickable_packed: PackedScene = load("res://objects/pickable.tscn")
onready var hazard_packed: PackedScene = load("res://objects/hazard.tscn")

var obstacle_positions = []

var remaining_packages = 0

func _ready() -> void:
	var _connect_err = teleport_trigger.connect("body_entered", self, "_on_body_entered")
	_connect_err = timer_tick.connect("timeout", self, "_tick")
	_connect_err = timer_level.connect("timeout", self, "_game_over")
	if _connect_err != OK:
		push_error("An error occured - Code %s" % _connect_err)
	
	randomize()
	_place_hazards()
	_place_pickables()
	
	if GameState.current_mode == GameState.GameMode.ARCADE:
		remaining_packages = GameState.get_required_packages()
		timer_level.wait_time = GameState.get_time_limit()
		hud.update_packages_remaining("%s" % remaining_packages)
		_tick()
		timer_level.start()
	
func _physics_process(_delta: float) -> void:
	teleport_destination.position = Vector2(player.position.x, teleport_destination.position.y)

func _on_body_entered(body: Node2D) -> void:
	if not body is Player:
		return
	body.position = teleport_destination.position
	_teardown()
	_place_hazards()
	_place_pickables()
	
func _make_pickable() -> Node2D:
	var pickable = pickable_packed.instance()
	var pickable_seed = rand_range(0, 50)
	if pickable_seed > 40:
		pickable.Kind = Pickable.Type.PACKAGE_PLUS
	elif pickable_seed > 30 and pickable_seed <= 39:
		pickable.Kind = Pickable.Type.TIME_MODIFIER
	pickable.redraw_sprite()
	var _connect_err = pickable.connect("picked_package", self, "_on_picked_package")
	_connect_err = pickable.connect("picked_modifier", self, "_on_picked_modifier")
	if _connect_err != OK:
		push_error("Failed to connect signals - code %s" % _connect_err)
	return pickable
	
func _make_hazard() -> Node2D:
	var hazard = hazard_packed.instance()
	var hazard_seed = rand_range(1, 20)
	if hazard_seed > 15:
		hazard.Kind = Hazard.Type.WET_FLOOR
		var _connect_err = hazard.connect("started_contact", player, "speed_up")
		_connect_err = hazard.connect("stopped_contact", player, "slow_down")
		if _connect_err != OK:
			push_error("Failed to connect signals - code %s" % _connect_err)
	else:
		hazard.connect("started_contact", self, "_game_over")
	hazard.setup_hazard()
	return hazard
	
func _game_over() -> void:
	var _err = get_tree().change_scene("res://scenes/game_over.tscn")
	
func _on_picked_package(amount: int) -> void:
	if GameState.current_mode == GameState.GameMode.ARCADE:
		remaining_packages -= amount
		if remaining_packages <= 0:
			var _err = get_tree().change_scene("res://scenes/level_success.tscn")
	else:
		remaining_packages += 1
	hud.update_packages_remaining("%s" % remaining_packages)

func _on_picked_modifier() -> void:
	var _elapsed_time = timer_level.time_left
	timer_level.stop()
	timer_level.wait_time = _elapsed_time + 7
	timer_level.start()

func _place_hazards() -> void:
	var last_vert_position = 300
	for _index in range(3):
		var random_x_position = rand_range(-4, 8)
		if random_x_position == 0:
			random_x_position += 3 if randf() > 0 else -3
		var hazard_object := _make_hazard()
		hazard_object.position = Vector2(random_x_position * 48 * 2, last_vert_position)
		obstacle_positions.append(hazard_object.position)
		call_deferred("add_child", hazard_object)
		last_vert_position += 16 + (hazard_object.get_rect().extents.y * 2)

func _place_pickables() -> void:
	var last_vert_position = -64
	for _index in range(6):
		var random_x_position = rand_range(-5, 7)
		var pickable_object := _make_pickable()
		pickable_object.position = Vector2(random_x_position * 48 * 2, last_vert_position)
		if pickable_object.position in obstacle_positions:
			pickable_object.position -= Vector2(0, 48)
		call_deferred("add_child", pickable_object)
		last_vert_position += 50 * 2
	
func _teardown() -> void:
	for child in get_children():
		if not child is Pickable and not child is Hazard:
			continue
		child.teardown()
	obstacle_positions.clear()
		
func _tick() -> void:
	hud.update_time_limit("%s" % timer_level.time_left as int)
