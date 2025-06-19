using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenSC2KTools.MapViewer
{
    /// <summary>
    /// Interaction logic for WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        public string BaseGameDirectory => FilePath.Text;

        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog flDlg = new OpenFolderDialog()
            {
                InitialDirectory = @"C:\",
                Multiselect = false,
                Title = "Select your SimCity 2000 Installation Folder",
                ValidateNames = true,
                DereferenceLinks = true,
                
            };
            if (!flDlg.ShowDialog() ?? true) return;
            FilePath.Text = flDlg.FolderName;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
