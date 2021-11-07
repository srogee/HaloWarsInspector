using System;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.IO;
using HaloWarsTools;
using HaloWarsInspector.Rendering;
using LearnOpenTK.Common;

namespace HaloWarsInspector
{
    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl : UserControl
    {
        private SceneBehavior scene;

        public MapControl(object dataContext) {
            InitializeComponent();

            scene = new SceneBehavior(OpenTkControl, 1500);

            DataContext = dataContext;
            var hwDataContext = DataContext as HWDataContext;
            if (hwDataContext != null) {
                myMapViewerTab.Header = "Map Viewer - " + Path.GetFileNameWithoutExtension(hwDataContext.RelativePath);
                var resource = HWXtdResource.FromFile(hwDataContext.Context, Path.ChangeExtension(hwDataContext.RelativePath, null));
                var resource2 = HWXttResource.FromFile(hwDataContext.Context, Path.ChangeExtension(hwDataContext.RelativePath, null));

                var shader = new Shader("Rendering/Shaders/shader.vert", "Rendering/Shaders/shader.frag");
                shader.Texture = Texture.FromBitmap(resource2.AlbedoTexture);

                scene.Root.Children.Add(SceneNode.FromGenericMesh(resource.Mesh, shader));
            }
        }

        private void OpenTkControl_OnRender(TimeSpan delta) => scene.TickAndRender(delta);
    }
}
