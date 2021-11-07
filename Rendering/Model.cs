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
        private static LazyValueCache ModelCache = new LazyValueCache();

        public static Model CoordinateHelper => ModelCache.Get(CreateCoordinateHelper);

        private int _elementBufferObject;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int indicesLength;
        private Shader shader;

        public Model(List<Vector3> vertices, List<Vector3> normals, List<Vector2> texCoords, List<int> indices, Shader shader) {
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
            this.shader = shader ?? ShaderCompiler.DefaultShader;
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

            shader.Use();

            var positionLocation = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(positionLocation);
            // Remember to change the stride as we now have 6 floats per vertex
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            // We now need to define the layout of the normal so the shader can use it
            var normalLocation = shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            var texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        }

        public void Draw(Matrix4 model, Matrix4 view, Matrix4 projection) {
            GL.BindVertexArray(_vertexArrayObject);

            // Then, we pass all of these matrices to the vertex shader.
            // You could also multiply them here and then pass, which is faster, but having the separate matrices available is used for some advanced effects.

            // IMPORTANT: OpenTK's matrix types are transposed from what OpenGL would expect - rows and columns are reversed.
            // They are then transposed properly when passed to the shader. 
            // This means that we retain the same multiplication order in both OpenTK c# code and GLSL shader code.
            // If you pass the individual matrices to the shader and multiply there, you have to do in the order "model * view * projection".
            // You can think like this: first apply the modelToWorld (aka model) matrix, then apply the worldToView (aka view) matrix, 
            // and finally apply the viewToProjectedSpace (aka projection) matrix.
            shader.Use();
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);

            GL.DrawElements(PrimitiveType.Triangles, indicesLength, DrawElementsType.UnsignedInt, 0);
        }

        private static Model CreateCoordinateHelper() {
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var texCoords = new List<Vector2>();
            var indices = new List<int>();

            var size = new Vector2(0.25f, 1000);
            DefineLine(vertices, normals, texCoords, indices, Vector3.Zero, Vector3.UnitX * size.Y, size.X, true, new Vector2(0.166f, 0.5f));
            DefineLine(vertices, normals, texCoords, indices, Vector3.Zero, Vector3.UnitY * size.Y, size.X, true, new Vector2(0.5f, 0.5f));
            DefineLine(vertices, normals, texCoords, indices, Vector3.Zero, Vector3.UnitZ * size.Y, size.X, true, new Vector2(0.833f, 0.5f));

            var model = new Model(vertices, normals, texCoords, indices, ShaderCompiler.RgbShader);

            return model;
        }

        private static void DefinePlane(List<Vector3> vertices, List<Vector3> normals, List<Vector2> texCoords, List<int> indices, Vector3 center, Vector3 xAxis, Vector3 yAxis, Vector2 size, bool overrideTexCoords, Vector2 texCoordOverride) {
            var startIndex = vertices.Count;
            vertices.Add(center - (xAxis * size.X / 2) - (yAxis * size.Y / 2));
            vertices.Add(center + (xAxis * size.X / 2) - (yAxis * size.Y / 2));
            vertices.Add(center + (xAxis * size.X / 2) + (yAxis * size.Y / 2));
            vertices.Add(center - (xAxis * size.X / 2) + (yAxis * size.Y / 2));
            
            var normal = Vector3.Cross(xAxis, yAxis);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);

            if (overrideTexCoords) {
                texCoords.Add(texCoordOverride);
                texCoords.Add(texCoordOverride);
                texCoords.Add(texCoordOverride);
                texCoords.Add(texCoordOverride);
            } else {
                texCoords.Add(new Vector2(0, 0));
                texCoords.Add(new Vector2(1, 0));
                texCoords.Add(new Vector2(1, 1));
                texCoords.Add(new Vector2(0, 1));
            }

            indices.Add(startIndex);
            indices.Add(startIndex + 1);
            indices.Add(startIndex + 2);
            indices.Add(startIndex);
            indices.Add(startIndex + 2);
            indices.Add(startIndex + 3);
        }

        private static void DefineLine(List<Vector3> vertices, List<Vector3> normals, List<Vector2> texCoords, List<int> indices, Vector3 start, Vector3 end, float size, bool overrideTexCoords, Vector2 texCoordOverride) {
            var sizeVector = new Vector2(size, Vector3.Distance(start, end));
            var center = (start + end) / 2;
            var xAxis = Vector3.Normalize(end - start);
            var yAxis = GetRandomTangent(xAxis);
            DefinePlane(vertices, normals, texCoords, indices, center, yAxis, xAxis, sizeVector, overrideTexCoords, texCoordOverride);
            DefinePlane(vertices, normals, texCoords, indices, center, Vector3.Cross(xAxis, yAxis), xAxis, sizeVector, overrideTexCoords, texCoordOverride);

        }

        private static Vector3 GetRandomTangent(Vector3 normal) {
            Vector3 tangent;
            Vector3 t1 = Vector3.Cross(normal, Vector3.UnitX);
            Vector3 t2 = Vector3.Cross(normal, Vector3.UnitZ);
            if (t1.Length > t2.Length) {
                tangent = t1;
            } else {
                tangent = t2;
            }

            return Vector3.Normalize(tangent);
        }
    }
}
