# MainMenu
# Package Resolved - Trijam 132
# (C) 2021 Marquis Kurt.

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

extends Control

onready var btn_start_arcade := $"VBoxContainer/Start"
onready var btn_start_endless := $"VBoxContainer/Endless"

func _ready() -> void:
	var _connect_err = btn_start_arcade.connect("button_up", self, "_btn_start_arcade_press")
	_connect_err = btn_start_endless.connect("button_up", self, "_btn_start_endless_press")
	
func _btn_start_arcade_press() -> void:
	GameState.current_mode = GameState.GameMode.ARCADE
	var _err = get_tree().change_scene("res://scenes/game_loop.tscn")
	
func _btn_start_endless_press() -> void:
	GameState.current_mode = GameState.GameMode.ENDLESS
	var _err = get_tree().change_scene("res://scenes/game_loop.tscn")
	
