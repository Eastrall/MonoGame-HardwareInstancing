# MonoGame-HardwareInstancing


This is a sample to explain how works Hardware Instancing in XNA 4.0 and MonoGame 3.5.

First of all let's see together what is the Hardware Instancing:

The `Hardware Instancing` allows you to draw the same model several times using GPU instancing techniques to reduce the cost of repeated draw calls.

For example if you have very large cube terrain, let's say `4098 * 4098` for peformances reasons you don't realy want to draw the cubes one by one. It will be very costly to you computer.

How to use it and how it works
------------------------------

In XNA 4.0 and in MonoGame 3.5 now, there is a new kind of vertex buffer named `VertexBufferBinding`

> **Definition**
> Binding structure that specifies a vertex buffer and other per-vertex parameters (such as offset and instancing) for a graphics device.

To use Hardware Instancing, you first need:

- `VertexBuffer` for the Common Geometry. (a cube or a plane for example)
- `IndexBuffer` for the indices of the Common Geometry.
- Another `VertexBuffer` for all the instances.

Once you have all this 3 properties, you will need a `VertexBufferBinding` to "link" your "Common Geometry" `VertexBuffer` and all your instances.

In the sample the binding is definied like that:

    // Creates the binding between the geometry and the instances.
    this.bindings = new VertexBufferBinding[2];
    this.bindings[0] = new VertexBufferBinding(geometryBuffer);
    this.bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);

By doing that you will have your `VertexBufferBinding` object that contains your geometry and all your instances. You can draw the content like any other `VertexBuffer`, and it will draw all your instances in just one call.


Hope it will help a lot people. If you have any question, or think that this tutorial/sample can be improved, don't hesitate to send me a message, or open an issue! 

Based on: http://stackoverflow.com/questions/9929103/need-help-using-instancing-in-xna-4-0