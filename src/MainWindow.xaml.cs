using ImagesToPdf.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace ImagesToPdf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string _previousSelection = null;

        public MainViewModel ViewModel { get; set; } = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = ViewModel;
        }

        private void ButtonAddFolder_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new FolderBrowserDialog();
            dialog.Description = "Select the directory that has images.";
            dialog.SelectedPath = _previousSelection ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            ViewModel.SourceFolders.Add(new SourceFolder(dialog.SelectedPath));
            _previousSelection = dialog.SelectedPath;
        }

        private void ButtonRemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewSourceFolders.SelectedItem == null)
                return;

            var sourceFolder = ListViewSourceFolders.SelectedItem as SourceFolder;
            if (sourceFolder == null)
                return;

            ViewModel.SourceFolders.Remove(sourceFolder);
        }

        private void ListViewSourceFolders_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridView gView = ListViewSourceFolders.View as GridView;
            var workingWidth = ListViewSourceFolders.ActualWidth - SystemParameters.VerticalScrollBarWidth;

            gView.Columns[0].Width = workingWidth;
        }

        private void ButtonOutputFolderBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "Select the directory that you save PDFs.";
            dialog.SelectedPath = ViewModel.OutputFolder;
            //dialog.RootFolder = Environment.SpecialFolder.Personal;
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            ViewModel.OutputFolder = dialog.SelectedPath;
        }

        private void ButtonConvert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string outputFolder = ViewModel.OutputFolder;

                foreach (var folder in ViewModel.SourceFolders)
                {
                    string folderName = Path.GetFileName(folder.Path.TrimEnd(Path.DirectorySeparatorChar));
                    string pdfFilename = $"{folderName}.pdf";
                    
                    var converter = new ImagesToPdfConverter(folder.Path, outputFolder, pdfFilename);

                    converter.Convert();
                }

                System.Windows.MessageBox.Show("Suucessfully converted!");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
