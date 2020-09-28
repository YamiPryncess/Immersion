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
extends SpringArm
# Control the zoom of the camera with `zoom`, a value between 0 and 1

export var length_range := Vector2(3.0, 6.0) setget set_length_range
export var zoom := 0.5 setget set_zoom

onready var _position_start: Vector3 = translation


# Ensures that each value is greater than 0, and that length_range.x <= length_range.y
# Then updates the zoom
func set_length_range(value: Vector2) -> void:
	value.x = max(value.x, 0.0)
	value.y = max(value.y, 0.0)
	length_range.x = min(value.x, value.y)
	length_range.y = max(value.x, value.y)
	self.zoom = zoom


func set_zoom(value: float) -> void:
	assert(value >= 0.0 and value <= 1.0)
	zoom = value
	spring_length = lerp(length_range.y, length_range.x, zoom)
