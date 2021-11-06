using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using HaloWarsTools;
using HaloWarsTools.Helpers;
using System.Linq;
using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HaloWarsInspector
{
    public class HWDataContext {
        public string RelativePath;
        public HWContext Context;
    }

    public class HWTreeEntry
    {
        public TreeViewItem TreeViewItem;

        public virtual void CreateTreeViewItem(MainWindow window) {
        }
    }

    public class HWTreeFolder : HWTreeEntry
    {
        public List<HWTreeEntry> FilesAndFolders;
        public string DisplayName;
        public bool IsExpanded;

        public HWTreeFolder(string displayName, bool isExpanded) {
            FilesAndFolders = new List<HWTreeEntry>();
            DisplayName = displayName;
            IsExpanded = isExpanded;
        }

        public override void CreateTreeViewItem(MainWindow window) {
            TreeViewItem = new TreeViewItem() {
                Header = MainWindow.CreateIconAndLabel("FolderClosed", DisplayName),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            TreeViewItem.IsExpanded = IsExpanded;
        }
    }

    public class HWTreeFile : HWTreeEntry
    {
        public HWDataContext Context;
        public string DisplayName;

        public HWTreeFile(string displayName, HWDataContext context) {
            Context = context;
            DisplayName = displayName;
        }

        public override void CreateTreeViewItem(MainWindow window) {
            TreeViewItem = new TreeViewItem() {
                Header = MainWindow.CreateIconAndLabel("ThreeDScene", DisplayName),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            TreeViewItem.DataContext = Context;
            TreeViewItem.Selected += window.FileItem_Selected;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HWContext context;

        public MainWindow() {
            InitializeComponent();
            Initialize();
        }

        private async void Initialize() {
            lblStatus.Text = "Searching for game install directory...";
            statusProgressBar.Visibility = Visibility.Visible;
            string gameInstallDirectory = await Task.Run(() => SteamInterop.GetGameInstallDirectory("HaloWarsDE"));
            if (string.IsNullOrEmpty(gameInstallDirectory)) {
                MessageBox.Show("Halo Wars Definitive Edition install directory could not be located.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                lblStatus.Text = "Error";
                statusProgressBar.Visibility = Visibility.Hidden;
                return;
            }

            string scratchDirectory = "C:\\Users\\rid3r\\Documents\\HaloWarsTools\\scratch";
            context = new HWContext(gameInstallDirectory, scratchDirectory);

            lblStatus.Text = "Expanding ERA files...";
            await Task.Run(() => context.ExpandAllEraFiles());

            lblStatus.Text = "Discovering game files...";
            string scenarioPath = Path.Combine(context.ScratchDirectory, "scenario");
            string artPath = Path.Combine(context.ScratchDirectory, "art");
            var root = new HWTreeFolder("root", true);
            await Task.Run(() => {
                AddAllFilesOfTypeToTree(root, "Maps", scenarioPath, "*.scn");
                AddAllFilesOfTypeToTree(root, "Visuals", artPath, "*.vis");
                AddAllFilesOfTypeToTree(root, "Models", artPath, "*.ugx");
            });

            BuildTreeView(root, myTreeView.Items);

            lblStatus.Text = "Ready";
            statusProgressBar.Visibility = Visibility.Hidden;
        }

        public void BuildTreeView(HWTreeFolder root, ItemCollection treeViewItems) {
            foreach (var entry in root.FilesAndFolders) {
                entry.CreateTreeViewItem(this);
                treeViewItems.Add(entry.TreeViewItem);
                if (entry is HWTreeFolder folder) {
                    BuildTreeView(folder, entry.TreeViewItem.Items);
                }
            }
        }

        private void AddAllFilesOfTypeToTree(HWTreeFolder folder, string topLevelFolderName, string startIn, string searchPattern) {
            var topLevelItem = new HWTreeFolder(topLevelFolderName, true);
            folder.FilesAndFolders.Add(topLevelItem);

            AddFilesToTreeHelper(topLevelItem, startIn, searchPattern);
        }

        private void AddFilesToTreeHelper(HWTreeFolder folder, string currentDirectory, string searchPattern) {
            foreach (var subdir in Directory.EnumerateDirectories(currentDirectory)) {
                if (!Directory.EnumerateFiles(subdir, searchPattern, SearchOption.AllDirectories).Any()) {
                    continue;
                }

                var newItem = new HWTreeFolder(Path.GetRelativePath(currentDirectory, subdir), false);
                folder.FilesAndFolders.Add(newItem);

                AddFilesToTreeHelper(newItem, subdir, searchPattern);
            }

            foreach (var file in Directory.EnumerateFiles(currentDirectory, searchPattern)) {
                var dataContext = new HWDataContext() {
                    RelativePath = context.GetRelativeScratchPath(file),
                    Context = context
                };
                var fileItem = new HWTreeFile(Path.GetFileNameWithoutExtension(file), dataContext);
                folder.FilesAndFolders.Add(fileItem);
            }
        }

        public void FileItem_Selected(object sender, RoutedEventArgs e) {
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

        public static Uri GetIconPath(string iconName) {
            return new Uri(Path.Combine("C:\\Users\\rid3r\\Documents\\GitHub\\VS2019 Image Library\\vswin2019", iconName, iconName + "_16x.png"));
        }

        public static StackPanel CreateIconAndLabel(string iconName, string label) {
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
