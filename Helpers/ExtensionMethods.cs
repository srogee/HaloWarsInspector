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

        public static OpenTK.Mathematics.Vector3 ToGLVector3(this Vector3 value) {
            return new OpenTK.Mathematics.Vector3(value.X, value.Y, value.Z);
        }

        public static OpenTK.Mathematics.Vector2 ToGLVector2(this Vector3 value) {
            return new OpenTK.Mathematics.Vector2(value.X, value.Y);
        }

        public static OpenTK.Mathematics.Vector2 ToGLVector2(this Vector2 value) {
            return new OpenTK.Mathematics.Vector2(value.X, value.Y);
        }
    }
}
