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
            OpenTkControl.Visibility = System.Windows.Visibility.Hidden;
            DataContext = dataContext;
            Load();
        }

        private async void Load() {
            var hwDataContext = DataContext as HWDataContext;
            myModelViewerTab.Header = "Model Viewer - " + Path.GetFileNameWithoutExtension(hwDataContext.RelativePath);
            MainWindow.Instance.SetStatus("Loading " + hwDataContext.RelativePath);
            await MainWindow.Instance.GiveApplicationAChanceToRender();

            scene = new SceneBehavior(OpenTkControl);
            OpenTkControl.Visibility = System.Windows.Visibility.Visible;
            var resource = HWUgxResource.FromFile(hwDataContext.Context, hwDataContext.RelativePath);
            scene.Root.Children.Add(SceneNode.FromGenericMesh(resource.Mesh));

            MainWindow.Instance.ResetStatus();
        }

        private void OpenTkControl_OnRender(TimeSpan delta) => scene.TickAndRender(delta);
    }
}