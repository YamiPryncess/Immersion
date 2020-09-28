// # MIT License

// # Copyright (c) 2020 GDQuest

// # Permission is hereby granted, free of charge, to any person obtaining a copy
// # of this software and associated documentation files (the "Software"), to deal
// # in the Software without restriction, including without limitation the rights
// # to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// # copies of the Software, and to permit persons to whom the Software is
// # furnished to do so, subject to the following conditions:

// # The above copyright notice and this permission notice shall be included in all
// # copies or substantial portions of the Software.

// # THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// # IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// # FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// # AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// # LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// # OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// # SOFTWARE.

using Godot;
using System;

public class Camera_Follow : Camera
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export] float distance = 4.0f;
    [Export] float height = 2.0f;
    
    public Spatial parent;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        parent = (Spatial)GetParent();
        SetPhysicsProcess(true);
        SetAsToplevel(true);
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _PhysicsProcess(float delta){
    Vector3 target = parent.GlobalTransform.origin;
    Vector3 pos = GlobalTransform.origin;
    Vector3 up = new Vector3(0,1,0);

    Vector3 offset = pos - target;
     
    offset = offset.Normalized()*distance;
    offset.y = height;

    pos = target + offset;

    LookAtFromPosition(pos, target, up);
 }
}
