using System.Collections.Generic;
using System.Linq;
using HaloWarsTools;
using LearnOpenTK.Common;
using OpenTK.Mathematics;

namespace HaloWarsInspector.Rendering
{
    public class SceneNode
    {
        public Matrix4 Matrix;
        public List<SceneNode> Children;

        public SceneNode() {
            Children = new List<SceneNode>();
            Matrix = Matrix4.Identity;
        }

        public void Draw(Matrix4 view, Matrix4 projection) {
            Draw(Matrix, view, projection);
        }

        protected virtual void Draw(Matrix4 model, Matrix4 view, Matrix4 projection) {
            foreach (var child in Children) {
                child.Draw(model * child.Matrix, view, projection); // TODO - is this right?
            }
        }

        public static SceneNode FromGenericMesh(GenericMesh mesh, Shader shader = null) {
            var node = new SceneNode();

            GenericMeshSection[] sections = mesh.GetMeshSections();
            //var materialMap = new Dictionary<GenericMaterial, Material>();

            foreach (var section in sections) {
                var faceGroups = mesh.Faces.Where(face => face.Section == section).GroupBy(face => face.Material);

                foreach (var group in faceGroups) {
                    var indicesToUse = new HashSet<int>();
                    var indexMap = new Dictionary<int, int>();
                    var vertices = new List<Vector3>();
                    var normals = new List<Vector3>();
                    var texcoords = new List<Vector2>();
                    var indices = new List<int>();

                    //var genericMaterial = group.First().Material;
                    //if (!materialMap.ContainsKey(genericMaterial)) {
                    //    materialMap.Add(genericMaterial, GenerateMaterial(genericMaterial));
                    //}

                    foreach (var face in group) {
                        indicesToUse.Add(face.A);
                        indicesToUse.Add(face.B);
                        indicesToUse.Add(face.C);
                    }

                    foreach (var vertexIndex in indicesToUse) {
                        indexMap.Add(vertexIndex, vertices.Count);
                        vertices.Add(mesh.Vertices[vertexIndex].ToGLVector3());
                        normals.Add(mesh.Normals[vertexIndex].ToGLVector3());
                        texcoords.Add(mesh.TexCoords[vertexIndex].ToGLVector2()); // TODO - might need to account for z coords? Are they used?
                    }

                    // Remap entire model indices to just this section indices
                    foreach (var face in group) {
                        indices.Add(indexMap[face.A]);
                        indices.Add(indexMap[face.B]);
                        indices.Add(indexMap[face.C]);
                    }

                    var model = new Model(vertices, normals, texcoords, indices, shader);
                    node.Children.Add(new SceneModelNode(model));
                }
            }

            return node;
        }
    }
}
