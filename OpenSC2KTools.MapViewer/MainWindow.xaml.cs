using Microsoft.Win32;
using OpenSC2KTools.MapViewer.Pages;
using OpenSC2Kv2.API;
using System;
using System.Collections.Generic;
using System.IO;
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
        const string SPECIAL_TOKEN = "recent.txt";

        public MainWindow()
        {
            InitializeComponent();

            DoWelcomePrompt();

            Loaded += MainWindow_Loaded;
        }

        private void DoWelcomePrompt()
        {
            string folderPath = default;
            bool success = false;
            if (File.Exists(SPECIAL_TOKEN))
                folderPath = File.ReadAllText(SPECIAL_TOKEN);
            try
            {
                SC2Path.Setup(folderPath);
                success = true;
            }
            catch
            {

            }

            while (!success)
            {
                if (File.Exists(SPECIAL_TOKEN))
                    File.Delete(SPECIAL_TOKEN);
                WelcomeWindow wnd = new WelcomeWindow();
                if (!wnd.ShowDialog() ?? true)
                {
                    Application.Current.Shutdown();
                    return;
                }
                folderPath = wnd.BaseGameDirectory;
                try
                {
                    SC2Path.Setup(folderPath);
                    success = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    continue;
                }
                try
                {
                    File.WriteAllText(SPECIAL_TOKEN, folderPath);
                }
                catch
                {
                    MessageBox.Show("There was a problem updating your selection. Try running the program as administrator next time.");
                }
            }
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
            var uri = new Uri(SC2Path.GetSpecialPath(SC2Path.SpecialPath.CitiesDirectory, "BAYVIEW.SC2"));
            if (Keyboard.GetKeyStates(Key.LeftShift) != KeyStates.Down)
            {
                OpenFileDialog dialog = new OpenFileDialog()
                {
                    InitialDirectory = SC2Path.GetSpecialPath(SC2Path.SpecialPath.CitiesDirectory),
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

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClearSC2KItem_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(SPECIAL_TOKEN);
            DoWelcomePrompt();
        }
    }
}
