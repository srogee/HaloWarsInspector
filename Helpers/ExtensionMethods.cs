using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ColorHelper;
using HaloWarsTools;

namespace HaloWarsInspector
{
    static class ExtensionMethods
    {
        public static Vector3D ToVector3D(this Point3D value) {
            return new Vector3D(value.X, value.Y, value.Z);
        }

        public static Vector3 ToVector3(this Point3D value) {
            return new Vector3((float)value.X, (float)value.Y, (float)value.Z);
        }

        public static Point3D ToPoint3D(this Vector3D value) {
            return new Point3D(value.X, value.Y, value.Z);
        }

        public static Vector3 ToVector3(this Vector3D value) {
            return new Vector3((float)value.X, (float)value.Y, (float)value.Z);
        }

        public static Point3D ToPoint3D(this Vector3 value) {
            return new Point3D(value.X, value.Y, value.Z);
        }

        public static Vector3D ToVector3D(this Vector3 value) {
            return new Vector3D(value.X, value.Y, value.Z);
        }

        public static ModelVisual3D ToModelVisual3d(this GenericMesh mesh) {
            var containerModel = new ModelVisual3D();

            GenericMeshSection[] sections = mesh.GetMeshSections();
            var materialMap = new Dictionary<GenericMaterial, Material>();

            foreach (var section in sections) {
                var faceGroups = mesh.Faces.Where(face => face.Section == section).GroupBy(face => face.Material);

                foreach (var group in faceGroups) {
                    var meshGeometry3d = new MeshGeometry3D();

                    var indicesToUse = new HashSet<int>();
                    var indexMap = new Dictionary<int, int>();

                    var genericMaterial = group.First().Material;
                    if (!materialMap.ContainsKey(genericMaterial)) {
                        materialMap.Add(genericMaterial, GenerateMaterial(genericMaterial));
                    }

                    foreach (var face in group) {
                        indicesToUse.Add(face.A);
                        indicesToUse.Add(face.B);
                        indicesToUse.Add(face.C);
                    }

                    foreach (var vertexIndex in indicesToUse) {
                        var vertex = mesh.Vertices[vertexIndex];
                        var normal = mesh.Normals[vertexIndex];
                        indexMap.Add(vertexIndex, meshGeometry3d.Positions.Count);
                        meshGeometry3d.Positions.Add(vertex.ToPoint3D());
                        meshGeometry3d.Normals.Add(normal.ToVector3D());
                    }

                    // Remap entire model indices to just this section indices
                    foreach (var face in group) {
                        meshGeometry3d.TriangleIndices.Add(indexMap[face.A]);
                        meshGeometry3d.TriangleIndices.Add(indexMap[face.B]);
                        meshGeometry3d.TriangleIndices.Add(indexMap[face.C]);
                    }

                    var modelVisual3d = new ModelVisual3D() {
                        Content = new GeometryModel3D() {
                            Geometry = meshGeometry3d,
                            Material = materialMap[genericMaterial],
                        }
                    };

                    containerModel.Children.Add(modelVisual3d);
                }
            }

            return containerModel;
        }

        private static Material GenerateMaterial(GenericMaterial material) {
            var color = ColorGenerator.GetRandomColor<RGB>();
            return new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(255, color.R, color.G, color.B)));
        }
    }
}
