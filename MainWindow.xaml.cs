using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using HaloWarsTools;
using HaloWarsTools.Helpers;
using System.Linq;

namespace HaloWarsInspector
{
    public class HWDataContext
    {
        public string RelativePath;
        public HWContext Context;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HWContext context;
        public MainWindow() {
            InitializeComponent();

            lblStatus.Text = "Searching for game directory...";
            string gameInstallDirectory = SteamInterop.GetGameInstallDirectory("HaloWarsDE");
            if (string.IsNullOrEmpty(gameInstallDirectory)) {
                MessageBox.Show("Halo Wars Definitive Edition install directory could not be located.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string scratchDirectory = "C:\\Users\\rid3r\\Documents\\HaloWarsTools\\scratch";
            context = new HWContext(gameInstallDirectory, scratchDirectory);

            lblStatus.Text = "Expanding ERA files...";
            context.ExpandAllEraFiles();

            lblStatus.Text = "Finding files...";
            string scenarioPath = Path.Combine(context.ScratchDirectory, "scenario");
            string artPath = Path.Combine(context.ScratchDirectory, "art");
            AddAllFilesOfTypeToTree("Maps", scenarioPath, "*.scn");
            AddAllFilesOfTypeToTree("Visuals", artPath, "*.vis");
            AddAllFilesOfTypeToTree("Models", artPath, "*.ugx");

            lblStatus.Text = "Ready";

            myControlDockPanel.Content = new ModelControl(null);
        }

        private void AddAllFilesOfTypeToTree(string topLevelFolderName, string startIn, string searchPattern) {
            var topLevelItem = new TreeViewItem() {
                Header = CreateIconAndLabel("FolderClosed", topLevelFolderName),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            topLevelItem.IsExpanded = true;
            myTreeView.Items.Add(topLevelItem);

            DoStuff(startIn, searchPattern, topLevelItem.Items);
        }

        private void DoStuff(string currentDirectory, string searchPattern, ItemCollection items) {
            foreach (var subdir in Directory.EnumerateDirectories(currentDirectory)) {
                if (!Directory.EnumerateFiles(subdir, searchPattern, SearchOption.AllDirectories).Any()) {
                    continue;
                }

                var newItem = new TreeViewItem() {
                    Header = CreateIconAndLabel("FolderClosed", Path.GetRelativePath(currentDirectory, subdir)),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                items.Add(newItem);

                DoStuff(subdir, searchPattern, newItem.Items);
            }

            foreach (var file in Directory.EnumerateFiles(currentDirectory, searchPattern)) {
                var fileItem = new TreeViewItem() {
                    Header = CreateIconAndLabel("ThreeDScene", Path.GetFileNameWithoutExtension(file)),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                fileItem.DataContext = new HWDataContext() {
                    RelativePath = context.GetRelativeScratchPath(file),
                    Context = context
                };
                fileItem.Selected += FileItem_Selected; ;
                items.Add(fileItem);
            }
        }

        private void FileItem_Selected(object sender, RoutedEventArgs e) {
            var treeViewItem = sender as TreeViewItem;
            var dataContext = treeViewItem?.DataContext as HWDataContext;
            if (dataContext != null) {
                lblStatus.Text = "Viewing " + dataContext.RelativePath;
                var extension = Path.GetExtension(dataContext.RelativePath);
                myControlDockPanel.Content = extension switch {
                    ".scn" => new MapControl(dataContext),
                    ".ugx" => new ModelControl(dataContext),
                    ".vis" => new VisualControl(dataContext),
                    _ => null
                };
            }
        }

        private Uri GetIconPath(string iconName) {
            return new Uri(Path.Combine("C:\\Users\\rid3r\\Documents\\GitHub\\VS2019 Image Library\\vswin2019", iconName, iconName + "_16x.png"));
        }

        private StackPanel CreateIconAndLabel(string iconName, string label) {
            // Add Icon
            // Create Stack Panel
            StackPanel stkPanel = new StackPanel();
            stkPanel.Orientation = Orientation.Horizontal;

            // Create Image
            Image img = new Image();
            img.Source = new BitmapImage(GetIconPath(iconName));
            img.Width = 16;
            img.Height = 16;
            img.Margin = new Thickness(0, 0, 3, 0);

            // Create TextBlock
            TextBlock lbl = new TextBlock();
            lbl.Text = label;

            // Add to stack
            stkPanel.Children.Add(img);
            stkPanel.Children.Add(lbl);

            return stkPanel;
        }
    }
}
