# MIT License

# Copyright (c) 2020 GDQuest

# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:

# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.

# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.

tool
class_name CameraRig
extends Spatial
# Accessor class that gives the nodes in the scene access the player or some
# frequently used nodes in the scene itself.

signal aim_fired(target_position)

onready var camera: InterpolatedCamera = $InterpolatedCamera
onready var spring_arm: SpringArm = $SpringArm
onready var aim_ray: RayCast = $InterpolatedCamera/AimRay
onready var aim_target: Sprite3D = $AimTarget

var player: KinematicBody

var zoom := 0.5 setget set_zoom

onready var _position_start: Vector3 = translation


func _ready() -> void:
	set_as_toplevel(true)
	yield(owner, "ready")
	player = owner


func _get_configuration_warning() -> String:
	return "Missing player node" if not player else ""


func set_zoom(value: float) -> void:
	zoom = clamp(value, 0.0, 1.0)
	if not spring_arm:
		yield(spring_arm, "ready")
	spring_arm.zoom = zoom
