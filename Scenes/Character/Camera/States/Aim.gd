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

extends CameraState
# Activates the aiming mode for the camera.
# Moves the camera to the character's shoulder, and narrows the field of view.
# Projects a target on the environment.

onready var tween := $Tween

export var fov := 40.0
export var offset_camera := Vector3(0.75, -0.7, 0)


func unhandled_input(event: InputEvent) -> void:
	if event.is_action_pressed("toggle_aim"):
		_state_machine.transition_to("Camera/Default")

	elif event.is_action_pressed("fire"):
		_state_machine.transition_to("Camera/Default")
		var target_position: Vector3 = (
			camera_rig.aim_ray.get_collision_point()
			if camera_rig.aim_ray.is_colliding()
			else camera_rig.get_global_transform().origin
		)
		camera_rig.emit_signal("aim_fired", target_position)

	else:
		_parent.unhandled_input(event)


func process(delta: float) -> void:
	_parent.process(delta)
	camera_rig.aim_target.update(camera_rig.aim_ray)


func enter(msg: Dictionary = {}) -> void:
	_parent._is_aiming = true
	camera_rig.aim_target.visible = true

	camera_rig.spring_arm.translation = camera_rig._position_start + offset_camera

	tween.interpolate_property(
		camera_rig.camera, 'fov', camera_rig.camera.fov, fov, 0.5, Tween.TRANS_QUAD, Tween.EASE_OUT
	)
	tween.start()


func exit() -> void:
	_parent._is_aiming = false
	camera_rig.aim_target.visible = false

	camera_rig.spring_arm.translation = camera_rig.spring_arm._position_start

	tween.interpolate_property(
		camera_rig.camera,
		'fov',
		camera_rig.camera.fov,
		_parent.fov_default,
		0.5,
		Tween.TRANS_QUAD,
		Tween.EASE_OUT
	)
	tween.start()
