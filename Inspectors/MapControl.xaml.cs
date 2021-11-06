using System;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.IO;
using HaloWarsTools;
using HaloWarsInspector.Rendering;

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

            scene = new SceneBehavior(OpenTkControl);

            DataContext = dataContext;
            var hwDataContext = DataContext as HWDataContext;
            if (hwDataContext != null) {
                myMapViewerTab.Header = "Map Viewer - " + Path.GetFileNameWithoutExtension(hwDataContext.RelativePath);
                var resource = HWXtdResource.FromFile(hwDataContext.Context, Path.ChangeExtension(hwDataContext.RelativePath, null));
                scene.Root.Children.Add(SceneNode.FromGenericMesh(resource.Mesh));
            }
        }

        private void OpenTkControl_OnRender(TimeSpan delta) => scene.TickAndRender(delta);
    }
}
