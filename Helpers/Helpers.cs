using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using OpenTK.Graphics.OpenGL4;

namespace HaloWarsInspector
{
    class Helpers
    {
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
    }
}
