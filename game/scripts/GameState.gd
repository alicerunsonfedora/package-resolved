# GameState
# Package Resolved - Trijam 132
# (C) 2021 Marquis Kurt.

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

extends Node

enum GameMode {
	ARCADE,
	ENDLESS
}

var current_level = 0
var current_mode = GameMode.ARCADE

var _max_levels = 0

var _level_data = []

func _ready() -> void:
	var json_file = File.new()
	var _err = json_file.open("res://levels.json", File.READ)
	if _err != OK:
		push_error("Error occurred: %s" % (_err))
		return
	var _json_data = JSON.parse(json_file.get_as_text())
	if _json_data.error != OK:
		return
	_level_data = _json_data.result as Array
	_max_levels = len(_level_data)
	
func get_required_packages() -> int:
	if current_level > len(_level_data):
		return -99
	return _level_data[current_level]["requiredPackages"]
	
func get_time_limit() -> int:
	if current_level > len(_level_data):
		return -99
	return _level_data[current_level]["timeLimit"]
	
func is_complete() -> bool:
	if current_mode == GameMode.ENDLESS:
		return false
	return current_level > _max_levels

func progress() -> void:
	current_level += 1
