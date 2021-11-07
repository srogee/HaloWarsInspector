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

        public HWDataContext(HWContext context, string relativePath) {
            Context = context;
            RelativePath = relativePath;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string imageLibraryPath = "C:\\Users\\rid3r\\Documents\\GitHub\\VS2019 Image Library\\vswin2019";
        private HWContext context;
        public static MainWindow Instance;
        private HashSet<string> extensions = new HashSet<string>() { ".xtd", ".vis", ".ugx" };

        public MainWindow() {
            Instance = this;
            InitializeComponent();
            Initialize();
        }

        public void SetStatus(string label) {
            lblStatus.Text = label;
            statusProgressBar.Visibility = Visibility.Visible;
        }

        public void ResetStatus() {
            lblStatus.Text = "Ready";
            statusProgressBar.Visibility = Visibility.Hidden;
        }

        private async void Initialize() {
            SetStatus("Searching for game install directory...");
            string gameInstallDirectory = await Task.Run(() => SteamInterop.GetGameInstallDirectory("HaloWarsDE"));
            if (string.IsNullOrEmpty(gameInstallDirectory)) {
                MessageBox.Show("Halo Wars Definitive Edition install directory could not be located.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                lblStatus.Text = "Error";
                statusProgressBar.Visibility = Visibility.Hidden;
                return;
            }

            string scratchDirectory = "C:\\Users\\rid3r\\Documents\\HaloWarsTools\\scratch";
            context = new HWContext(gameInstallDirectory, scratchDirectory);

            SetStatus("Expanding ERA files...");
            await Task.Run(() => context.ExpandAllEraFiles());

            PopulateTreeViewFolder(myTreeView.Items, "");

            ResetStatus();
        }

        private bool DoesDirectoryHaveRelevantFiles(string absolutePath) {
            return Directory.EnumerateFiles(absolutePath, "*.*", SearchOption.AllDirectories).Where(file => extensions.Contains(Path.GetExtension(file).ToLowerInvariant())).Any();
        }

        private async void PopulateTreeViewFolder(ItemCollection collection, string relativePath) {
            collection.Clear();
            collection.Add(CreateLoadingTreeViewItem());

            SetStatus("Discovering game files...");
            string[] filesInDirectory = null;
            string[] foldersInDirectory = null;

            await Task.Run(() => {
                string absolutePath = context.GetAbsoluteScratchPath(relativePath);
                foldersInDirectory = Directory.EnumerateDirectories(absolutePath).Where(dir => DoesDirectoryHaveRelevantFiles(dir)).ToArray();
                filesInDirectory = Directory.EnumerateFiles(absolutePath, "*.*", SearchOption.TopDirectoryOnly).Where(file => extensions.Contains(Path.GetExtension(file).ToLowerInvariant())).ToArray();
            });

            collection.Clear();
            foreach (var folder in foldersInDirectory) {
                var treeViewItem = new TreeViewItem() {
                    Header = CreateIconAndLabel("FolderClosed", Path.GetFileName(folder)),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                treeViewItem.DataContext = context.GetRelativeScratchPath(folder);
                treeViewItem.Expanded += TreeViewItem_Expanded;

                collection.Add(treeViewItem);
                treeViewItem.Items.Add(CreateLoadingTreeViewItem());
            }

            foreach (var file in filesInDirectory) {
                var treeViewItem = new TreeViewItem() {
                    Header = CreateIconAndLabel("ThreeDScene", Path.GetFileName(file)),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                treeViewItem.DataContext = new HWDataContext(context, context.GetRelativeScratchPath(file));
                treeViewItem.Selected += FileItem_Selected;
                collection.Add(treeViewItem);
            }

            ResetStatus();
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e) {
            var treeViewItem = sender as TreeViewItem;
            string relativePath = treeViewItem.DataContext as string;
            PopulateTreeViewFolder(treeViewItem.Items, relativePath);
            e.Handled = true;
        }

        private TreeViewItem CreateLoadingTreeViewItem() {
            var loadingItem = new TreeViewItem() {
                Header = CreateIconAndLabel("Hourglass", "Loading..."),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center
            };

            return loadingItem;
        }

        public void FileItem_Selected(object sender, RoutedEventArgs e) {
            var treeViewItem = sender as TreeViewItem;
            var dataContext = treeViewItem?.DataContext as HWDataContext;
            if (dataContext != null) {
                var extension = Path.GetExtension(dataContext.RelativePath);
                myControlDockPanel.Content = extension switch {
                    ".xtd" => new MapControl(dataContext),
                    ".ugx" => new ModelControl(dataContext),
                    ".vis" => new VisualControl(dataContext),
                    _ => null
                };
            }
        }

        public static Uri GetIconPath(string iconName) {
            return new Uri(Path.Combine(imageLibraryPath, iconName, iconName + "_16x.png"));
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
