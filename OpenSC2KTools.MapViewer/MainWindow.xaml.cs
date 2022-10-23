using Microsoft.Win32;
using OpenSC2KTools.MapViewer.Pages;
using OpenSC2Kv2.API;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpenSC2KTools.MapViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window()
            {
                Content = new BuildingViewer()
            };
            window.Show();
        }

        private async void SC2Item_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(@"E:\Games\SC2K\GAME\Cities\BAYVIEW.SC2");
            if (Keyboard.GetKeyStates(Key.LeftShift) != KeyStates.Down)
            {
                OpenFileDialog dialog = new OpenFileDialog()
                {
                    InitialDirectory = @"E:\Games\SC2K\GAME\CITIES\",
                    Multiselect = false,
                    Title = "Open *.DAT File"
                };
                if (dialog.ShowDialog() ?? false)
                {
                    uri = new Uri(dialog.FileName);
                }
            }
            IFFParser parser = new IFFParser(uri);
            await parser.ParseAsync();

            ViewerPage.Attach(parser);
        }
    }
}
