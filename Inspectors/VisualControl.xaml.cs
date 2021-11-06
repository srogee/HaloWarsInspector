using System.Windows.Controls;
using HaloWarsTools;
using System.IO;
using System;
using HaloWarsInspector.Rendering;

namespace HaloWarsInspector
{
    /// <summary>
    /// Interaction logic for ModelControl.xaml
    /// </summary>
    public partial class VisualControl : UserControl
    {
        private SceneBehavior scene;

        public VisualControl(object dataContext) {
            InitializeComponent();

            scene = new SceneBehavior(OpenTkControl);

            DataContext = dataContext;
            var hwDataContext = DataContext as HWDataContext;
            if (hwDataContext != null) {
                myVisualViewerTab.Header = "Visual Viewer - " + Path.GetFileNameWithoutExtension(hwDataContext.RelativePath);
                var resource = HWVisResource.FromFile(hwDataContext.Context, hwDataContext.RelativePath);
                foreach (var model in resource.Models) {
                    scene.Root.Children.Add(SceneNode.FromGenericMesh(model.Resource.Mesh));
                }
            }
        }

        private void OpenTkControl_OnRender(TimeSpan delta) => scene.TickAndRender(delta);
    }
}
