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
            OpenTkControl.Visibility = System.Windows.Visibility.Hidden;
            DataContext = dataContext;
            Load();
        }

        private async void Load() {
            var hwDataContext = DataContext as HWDataContext;
            myVisualViewerTab.Header = "Visual Viewer - " + Path.GetFileNameWithoutExtension(hwDataContext.RelativePath);
            MainWindow.Instance.SetStatus("Loading " + hwDataContext.RelativePath);
            await MainWindow.Instance.GiveApplicationAChanceToRender();

            scene = new SceneBehavior(OpenTkControl);
            OpenTkControl.Visibility = System.Windows.Visibility.Visible;
            var resource = HWVisResource.FromFile(hwDataContext.Context, hwDataContext.RelativePath);
            foreach (var model in resource.Models) {
                scene.Root.Children.Add(SceneNode.FromGenericMesh(model.Resource.Mesh));
            }

            MainWindow.Instance.ResetStatus();
        }

        private void OpenTkControl_OnRender(TimeSpan delta) => scene.TickAndRender(delta);
    }
}
