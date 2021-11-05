using System.Windows.Controls;
using HaloWarsTools;
using System.IO;

namespace HaloWarsInspector
{
    /// <summary>
    /// Interaction logic for ModelControl.xaml
    /// </summary>
    public partial class VisualControl : UserControl
    {
        public VisualControl(object dataContext) {
            InitializeComponent();

            DataContext = dataContext;

            var hwDataContext = DataContext as HWDataContext;
            myVisualViewerTab.Header = "Visual Viewer - " + Path.GetFileNameWithoutExtension(hwDataContext.RelativePath);
            var resource = HWVisResource.FromFile(hwDataContext.Context, hwDataContext.RelativePath);

            myScene.Children.Clear();
            foreach (var model in resource.Models) {
                myScene.Children.Add(model.Resource.Mesh.ToModelVisual3d());
            }
        }
    }
}
