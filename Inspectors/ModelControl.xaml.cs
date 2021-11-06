using System.Windows.Controls;
using OpenTK.Wpf;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using HaloWarsInspector.Rendering;
using System.Linq;

namespace HaloWarsInspector
{
    /// <summary>
    /// Interaction logic for ModelControl.xaml
    /// </summary>
    public partial class ModelControl : UserControl
    {
        public ModelControl(object dataContext) {
            InitializeComponent();

            DataContext = dataContext;

            //var hwDataContext = DataContext as HWDataContext;
            //if (hwDataContext != null) {
            //    myModelViewerTab.Header = "Model Viewer - " + Path.GetFileNameWithoutExtension(hwDataContext.RelativePath);
            //    var resource = HWUgxResource.FromFile(hwDataContext.Context, hwDataContext.RelativePath);

            //    myScene.Children.Clear();
            //    myScene.Children.Add(resource.Mesh.ToModelVisual3d());
            //    Helpers.SetupCamera(this, viewport3D1, camMain);
            //} else {
            //    var trackball = new TrackballController();
            //    trackball.Attach(this);
            //    trackball.Viewports.Add(viewport3D1);
            //    trackball.Enabled = true;
            //}

            var settings = new GLWpfControlSettings {
                MajorVersion = 4,
                MinorVersion = 6
            };
            OpenTkControl.Start(settings);
        }

        // We create a double to hold how long has passed since the program was opened.
        private double _time;
        private Model _model;

        private void OpenTkControl_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // We enable depth testing here. If you try to draw something more complex than one plane without this,
            // you'll notice that polygons further in the background will occasionally be drawn over the top of the ones in the foreground.
            // Obviously, we don't want this, so we enable depth testing. We also clear the depth buffer in GL.Clear over in OnRenderFrame.
            GL.Enable(EnableCap.DepthTest);

            float[] data = new float[] {
                 // Position          Normal
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0f, 0f, // Front face
                 0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1f, 0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1f, 1f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1f, 1f,
                -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0f, 1f,
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0f, 0f,

                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0f, 0f, // Back face
                 0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1f, 0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1f, 1f,
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1f, 1f,
                -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0f, 1f,
                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0f, 0f,

                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 1f, 1f, // Left face
                -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 1f, 0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 0f, 0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 0f, 0f,
                -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 0f, 1f,
                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 1f, 1f,

                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 1f, 1f, // Right face
                 0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 1f, 0f,
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 0f, 0f,
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 0f, 0f,
                 0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 0f, 1f,
                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 1f, 1f,

                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 0f, 0f, // Bottom face
                 0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 1f, 0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 1f, 1f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 1f, 1f,
                -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 0f, 1f,
                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 0f, 0f,

                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 0f, 0f, // Top face
                 0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 1f, 0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 1f, 1f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 1f, 1f,
                -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 0f, 1f,
                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 0f, 0f,
            };

            uint[] indices = Enumerable.Range(0, 36).Select(number => (uint)number).ToArray();

            // Now, head over to OnRenderFrame to see how we setup the model matrix.
            _model = new Model(data, indices);
        }

        private void OpenTkControl_OnRender(TimeSpan delta) {
            if (!OpenTkControl.IsLoaded) {
                return;
            }

            // We add the time elapsed since last frame, times 4.0 to speed up animation, to the total amount of time passed.
            _time += 10.0 * delta.TotalSeconds;

            // Finally, we have the model matrix. This determines the position of the model.
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            // For the view, we don't do too much here. Next tutorial will be all about a Camera class that will make it much easier to manipulate the view.
            // For now, we move it backwards three units on the Z axis.
            var view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);

            // For the matrix, we use a few parameters.
            //   Field of view. This determines how much the viewport can see at once. 45 is considered the most "realistic" setting, but most video games nowadays use 90
            //   Aspect ratio. This should be set to Width / Height.
            //   Near-clipping. Any vertices closer to the camera than this value will be clipped.
            //   Far-clipping. Any vertices farther away from the camera than this value will be clipped.
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), (float)OpenTkControl.RenderSize.Width / (float)OpenTkControl.RenderSize.Height, 0.1f, 100.0f);

            var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time));
            _model.Draw(model, view, projection);
        }
    }
}
