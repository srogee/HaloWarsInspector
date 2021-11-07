using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Wpf;

namespace HaloWarsInspector.Rendering
{
    public class SceneBehavior
    {
        private GLWpfControl control;
        public SceneNode Root;
        private float cameraDistance;

        public SceneBehavior(GLWpfControl control, float cameraDistance = 150) {
            this.control = control;
            Root = new SceneNode();

            var settings = new GLWpfControlSettings {
                MajorVersion = 4,
                MinorVersion = 6
            };

            this.cameraDistance = cameraDistance;
            control.Start(settings);
        }

        public void TickAndRender(TimeSpan delta) {
            Tick(delta);
            Render();
        }

        public void Tick(TimeSpan delta) {
            // We add the time elapsed since last frame, times 4.0 to speed up animation, to the total amount of time passed.
            _time += 10.0 * delta.TotalSeconds;
        }

        public void Render() {
            GL.ClearColor(1f, 1f, 1f, 1f);

            // We enable depth testing here. If you try to draw something more complex than one plane without this,
            // you'll notice that polygons further in the background will occasionally be drawn over the top of the ones in the foreground.
            // Obviously, we don't want this, so we enable depth testing. We also clear the depth buffer in GL.Clear over in OnRenderFrame.
            GL.Enable(EnableCap.DepthTest);

            // Finally, we have the model matrix. This determines the position of the model.
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            // For the view, we don't do too much here. Next tutorial will be all about a Camera class that will make it much easier to manipulate the view.
            // For now, we move it backwards three units on the Z axis.
            var view = Matrix4.CreateTranslation(0.0f, 0.0f, -cameraDistance); // this is actually opposite the camera's position

            // For the matrix, we use a few parameters.
            //   Field of view. This determines how much the viewport can see at once. 45 is considered the most "realistic" setting, but most video games nowadays use 90
            //   Aspect ratio. This should be set to Width / Height.
            //   Near-clipping. Any vertices closer to the camera than this value will be clipped.
            //   Far-clipping. Any vertices farther away from the camera than this value will be clipped.
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), (float)control.RenderSize.Width / (float)control.RenderSize.Height, 1f, 10000);

            var mat = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time)) * Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(_time * 0.5f));

            Model.CoordinateHelper.Draw(mat, view, projection);
            if (Root != null) {
                Root.Matrix = mat;
                Root.Draw(view, projection);
            }
        }

        // We create a double to hold how long has passed since the program was opened.
        private double _time;
    }
}
