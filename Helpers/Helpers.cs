using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using HaloWarsTools;

namespace HaloWarsInspector
{
    class Helpers
    {
        public static void SetupCamera(FrameworkElement element, Viewport3D viewport, PerspectiveCamera camera) {
            var trackball = new TrackballController();
            trackball.Attach(element);
            trackball.Viewports.Add(viewport);
            trackball.Enabled = true;
            var scene = viewport.Children[1] as ModelVisual3D;

            Rect3D bounds;
            bool assigned = false;
            ExpandBounds(scene, ref bounds, ref assigned);

            float distance = ((Vector3D)bounds.Size).ToVector3().Length() * 1.5f;
            camera.Position = (Vector3.Normalize(Vector3.One) * distance).ToPoint3D();
        }

        private static void ExpandBounds(Visual3D node, ref Rect3D bounds, ref bool assigned) {
            if (node is ModelVisual3D modelVisual3d) {
                var model = modelVisual3d.Content;
                if (model != null) {
                    if (assigned) {
                        bounds.Union(model.Bounds);
                    } else {
                        bounds = model.Bounds;
                        assigned = true;
                    }
                }

                foreach (var child in modelVisual3d.Children) {
                    ExpandBounds(child, ref bounds, ref assigned);
                }
            }
        }

        public void GetOrCreateOpenGLScene(GenericMesh mesh) {

        }
    }
}
