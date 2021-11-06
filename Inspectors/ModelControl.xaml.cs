using System.Windows.Controls;
using System;
using HaloWarsInspector.Rendering;
using HaloWarsTools;
using System.IO;

namespace HaloWarsInspector
{
    /// <summary>
    /// Interaction logic for ModelControl.xaml
    /// </summary>
    public partial class ModelControl : UserControl
    {
        private SceneBehavior scene;

        public ModelControl(object dataContext) {
            InitializeComponent();

            scene = new SceneBehavior(OpenTkControl);

            DataContext = dataContext;
            var hwDataContext = DataContext as HWDataContext;
            if (hwDataContext != null) {
                myModelViewerTab.Header = "Model Viewer - " + Path.GetFileNameWithoutExtension(hwDataContext.RelativePath);
                var resource = HWUgxResource.FromFile(hwDataContext.Context, hwDataContext.RelativePath);

                scene.Root.Children.Add(SceneNode.FromGenericMesh(resource.Mesh));
            }
        }

        private void OpenTkControl_OnRender(TimeSpan delta) => scene.TickAndRender(delta);
    }
}