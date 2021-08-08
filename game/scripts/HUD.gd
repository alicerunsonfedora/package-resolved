# HUD
# Package Resolved - Trijam 132
# (C) 2021 Marquis Kurt.

# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

class_name HUD
extends Control

onready var _packages_remaining := $"PackagesRemaining/Label"
onready var _time_limit := $"TimeLimit/Label"

onready var _timer := $"Timer"
onready var _tween := $"Tween"

func _ready() -> void:
	if GameState.current_level > 0 or GameState.current_mode == GameState.GameMode.ENDLESS:
		$"IntroLabel".visible = false
	else:
		_tween.interpolate_property(
			$"IntroLabel",
			"modulate",
			Color.white,
			Color.transparent,
			0.5,
			Tween.TRANS_LINEAR,
			Tween.EASE_IN_OUT
		)
		var _err = _timer.connect("timeout", _tween, "start")
		_timer.start()

func update_packages_remaining(text: String) -> void:
	_packages_remaining.text = text
	
func update_time_limit(text: String) -> void:
	_time_limit.text = text

