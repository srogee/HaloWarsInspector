using System.Windows.Controls;
using HaloWarsTools;
using System.IO;
using System.Windows.Media.Media3D;
using OpenTK.Wpf;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using LearnOpenTK.Common;

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

        private readonly float[] _vertices =
        {
            // Position         Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int _elementBufferObject;

        private int _vertexBufferObject;

        private int _vertexArrayObject;

        private Shader _shader;

        private Texture _texture;

        private Texture _texture2;

        // We create a double to hold how long has passed since the program was opened.
        private double _time;

        // Then, we create two matrices to hold our view and projection. They're initialized at the bottom of OnLoad.
        // The view matrix is what you might consider the "camera". It represents the current viewport in the window.
        private Matrix4 _view;

        // This represents how the vertices will be projected. It's hard to explain through comments,
        // so check out the web version for a good demonstration of what this does.
        private Matrix4 _projection;

        private void OpenTkControl_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // We enable depth testing here. If you try to draw something more complex than one plane without this,
            // you'll notice that polygons further in the background will occasionally be drawn over the top of the ones in the foreground.
            // Obviously, we don't want this, so we enable depth testing. We also clear the depth buffer in GL.Clear over in OnRenderFrame.
            GL.Enable(EnableCap.DepthTest);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            // shader.vert has been modified. Take a look at it after the explanation in OnRenderFrame.
            _shader = new Shader("Rendering/Shaders/shader.vert", "Rendering/Shaders/shader.frag");
            _shader.Use();

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _texture = Texture.LoadFromFile("Rendering/Resources/container.png");
            _texture.Use(TextureUnit.Texture0);

            _texture2 = Texture.LoadFromFile("Rendering/Resources/awesomeface.png");
            _texture2.Use(TextureUnit.Texture1);

            _shader.SetInt("texture0", 0);
            _shader.SetInt("texture1", 1);

            // For the view, we don't do too much here. Next tutorial will be all about a Camera class that will make it much easier to manipulate the view.
            // For now, we move it backwards three units on the Z axis.
            _view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);

            // For the matrix, we use a few parameters.
            //   Field of view. This determines how much the viewport can see at once. 45 is considered the most "realistic" setting, but most video games nowadays use 90
            //   Aspect ratio. This should be set to Width / Height.
            //   Near-clipping. Any vertices closer to the camera than this value will be clipped.
            //   Far-clipping. Any vertices farther away from the camera than this value will be clipped.
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), (float)OpenTkControl.RenderSize.Width / (float)OpenTkControl.RenderSize.Height, 0.1f, 100.0f);

            // Now, head over to OnRenderFrame to see how we setup the model matrix.
        }

        private void OpenTkControl_OnRender(TimeSpan delta) {
            if (!OpenTkControl.IsLoaded) {
                return;
            }

            // We add the time elapsed since last frame, times 4.0 to speed up animation, to the total amount of time passed.
            _time += 10.0 * delta.TotalSeconds;

            // We clear the depth buffer in addition to the color buffer.
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(_vertexArrayObject);

            _texture.Use(TextureUnit.Texture0);
            _texture2.Use(TextureUnit.Texture1);
            _shader.Use();

            // Finally, we have the model matrix. This determines the position of the model.
            var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time));

            // Then, we pass all of these matrices to the vertex shader.
            // You could also multiply them here and then pass, which is faster, but having the separate matrices available is used for some advanced effects.

            // IMPORTANT: OpenTK's matrix types are transposed from what OpenGL would expect - rows and columns are reversed.
            // They are then transposed properly when passed to the shader. 
            // This means that we retain the same multiplication order in both OpenTK c# code and GLSL shader code.
            // If you pass the individual matrices to the shader and multiply there, you have to do in the order "model * view * projection".
            // You can think like this: first apply the modelToWorld (aka model) matrix, then apply the worldToView (aka view) matrix, 
            // and finally apply the viewToProjectedSpace (aka projection) matrix.
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", _view);
            _shader.SetMatrix4("projection", _projection);

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
