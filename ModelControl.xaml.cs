using System.Windows.Controls;
using HaloWarsTools;
using System.IO;

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

            var hwDataContext = DataContext as HWDataContext;
            myModelViewerTab.Header = "Model Viewer - " + Path.GetFileNameWithoutExtension(hwDataContext.RelativePath);
            var resource = HWUgxResource.FromFile(hwDataContext.Context, hwDataContext.RelativePath);

            myScene.Children.Clear();
            myScene.Children.Add(resource.Mesh.ToModelVisual3d());
        }
    }
}
