# %CLASS%
# Package Resolved - Trijam 132
# (C) 2021 Marquis Kurt.

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

extends Control

onready var btn_restart := $"VBoxContainer/Restart"
onready var btn_quit_to_menu := $"VBoxContainer/MainMenu"

onready var helmet := $"Panel/Base/Helmet"

func _ready() -> void:
	var _connect_err = btn_restart.connect("button_up", self, "_btn_press_restart")
	_connect_err = btn_quit_to_menu.connect("button_up", self, "_btn_press_quit_to_menu")
	
	GameState.progress()
	if GameState.is_complete():
		btn_restart.visible = false
		helmet.visible = false

func _btn_press_restart() -> void:
	var _err = get_tree().change_scene("res://scenes/game_loop.tscn")

func _btn_press_quit_to_menu() -> void:
	var _err = get_tree().change_scene("res://scenes/main_menu.tscn")
