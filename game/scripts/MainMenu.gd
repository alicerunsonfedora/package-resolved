# MainMenu
# Package Resolved - Trijam 132
# (C) 2021 Marquis Kurt.

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

extends Control

onready var btn_start_arcade := $"VBoxContainer/Start"
onready var btn_start_endless := $"VBoxContainer/Endless"

onready var chk_music := $"VBoxContainer/Music"
onready var chk_sfx := $"VBoxContainer/SFX"

onready var helmet := $"Base/Helmet"

func _ready() -> void:
	var _connect_err = btn_start_arcade.connect("button_up", self, "_btn_start_arcade_press")
	_connect_err = btn_start_endless.connect("button_up", self, "_btn_start_endless_press")
	_connect_err = chk_music.connect("toggled", self, "_chk_music_toggle")
	_connect_err = chk_sfx.connect("toggled", self, "_chk_sfx_toggle")
	
	chk_music.pressed = GameState.music_enabled
	chk_sfx.pressed = GameState.sfx_enabled
	
	if GameState.is_complete():
		helmet.visible = false
	
func _btn_start_arcade_press() -> void:
	GameState.current_mode = GameState.GameMode.ARCADE
	var _err = get_tree().change_scene("res://scenes/game_loop.tscn")
	
func _btn_start_endless_press() -> void:
	GameState.current_mode = GameState.GameMode.ENDLESS
	var _err = get_tree().change_scene("res://scenes/game_loop.tscn")
	
func _chk_music_toggle(value: bool) -> void:
	GameState.music_enabled = value
	
func _chk_sfx_toggle(value: bool) -> void:
	GameState.sfx_enabled = value
