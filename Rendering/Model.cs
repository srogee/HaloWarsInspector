using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloWarsTools;
using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace HaloWarsInspector.Rendering
{
    public class Model
    {
        private int _elementBufferObject;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int indicesLength;
        private Shader _shader;
        private Texture _texture;

        public Model(List<Vector3> vertices, List<Vector3> normals, List<Vector2> texCoords, IEnumerable<int> indices) {
            var data = new List<float>();
            
            for (int i = 0; i < vertices.Count; i++) {
                data.Add(vertices[i].X);
                data.Add(vertices[i].Y);
                data.Add(vertices[i].Z);

                data.Add(normals[i].X);
                data.Add(normals[i].Y);
                data.Add(normals[i].Z);

                data.Add(texCoords[i].X);
                data.Add(texCoords[i].Y);
            }

            var uintIndices = indices.Select(index => (uint)index);

            Initialize(data.ToArray(), uintIndices.ToArray());
        }

        public Model(float[] data, uint[] indices) {
            Initialize(data.ToArray(), indices.ToArray()); // Makes a copy
        }

        private void Initialize(float[] data, uint[] indices) {
            indicesLength = indices.Length;

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // shader.vert has been modified. Take a look at it after the explanation in OnRenderFrame.
            _shader = new Shader("Rendering/Shaders/shader.vert", "Rendering/Shaders/shader.frag");
            _shader.Use();

            var positionLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(positionLocation);
            // Remember to change the stride as we now have 6 floats per vertex
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            // We now need to define the layout of the normal so the shader can use it
            var normalLocation = _shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

            _texture = Texture.LoadFromFile("Rendering/Resources/container.png");
            _texture.Use(TextureUnit.Texture0);

            _shader.SetInt("texture0", 0);
        }

        public void Draw(Matrix4 model, Matrix4 view, Matrix4 projection) {
            GL.BindVertexArray(_vertexArrayObject);

            _texture.Use(TextureUnit.Texture0);
            _shader.Use();

            // Then, we pass all of these matrices to the vertex shader.
            // You could also multiply them here and then pass, which is faster, but having the separate matrices available is used for some advanced effects.

            // IMPORTANT: OpenTK's matrix types are transposed from what OpenGL would expect - rows and columns are reversed.
            // They are then transposed properly when passed to the shader. 
            // This means that we retain the same multiplication order in both OpenTK c# code and GLSL shader code.
            // If you pass the individual matrices to the shader and multiply there, you have to do in the order "model * view * projection".
            // You can think like this: first apply the modelToWorld (aka model) matrix, then apply the worldToView (aka view) matrix, 
            // and finally apply the viewToProjectedSpace (aka projection) matrix.
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

            GL.DrawElements(PrimitiveType.Triangles, indicesLength, DrawElementsType.UnsignedInt, 0);
        }
    }
}
