using System;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.IO;
using HaloWarsTools;

namespace HaloWarsInspector
{
    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl : UserControl
    {
        public MapControl(object dataContext) {
            InitializeComponent();

            DataContext = dataContext;

            var hwDataContext = DataContext as HWDataContext;
            myMapViewerTab.Header = "Map Viewer - " + Path.GetFileNameWithoutExtension(hwDataContext.RelativePath);
            var resource = HWXtdResource.FromFile(hwDataContext.Context, Path.ChangeExtension(hwDataContext.RelativePath, null));

            myScene.Children.Clear();
            myScene.Children.Add(resource.Mesh.ToModelVisual3d());
        }
    }
}
