using Microsoft.Win32;
using OpenSC2Kv2.API.Graphics;
using OpenSC2Kv2.API.Graphics.Win95;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpenSC2KTools.MapViewer.Pages
{
    /// <summary>
    /// Interaction logic for BuildingViewer.xaml
    /// </summary>
    public partial class BuildingViewer : Page
    {
        private SPRRender renderer;
        SC2SpriteArchive archive;
        SC2GraphicResource currentResource;
        private int currentIndex = 0, lastCurrentFrame = 0;
        private double currentFrame = 0, animationSpeed = 7.25;
        ConcurrentDictionary<string, ImageSource> imgDictionary = new();
        Uri DATFilePath = new Uri(@"E:\Games\SC2K\GAME\Data\LARGE.DAT");

        Task anonymousLoadTask;
        Timer animationClock;

        public BuildingViewer()
        {
            InitializeComponent();

            Loaded += BuildingViewer_Loaded;
        }

        private async Task Load()
        {
            ContentGrid.IsEnabled = false;
            LoadingGrid.Visibility = Visibility.Visible;

            LoadingDescBlock.Text = "Reticulating Splines...";
            Catelog.Children.Clear();

            var extractor = new SPRExtractor(DATFilePath);
            renderer = new SPRRender();

            await Task.Run(async delegate
            {
                try
                {
                    var actionResult = extractor.ExtractAll();
                    if (actionResult == null)
                        throw new Exception("Results inconclusive.");
                    if (actionResult.Value == null)
                        throw new Exception($"{string.Join(", ", actionResult.Errors)}");
                    archive = actionResult.Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                anonymousLoadTask = LoadAll();

                currentIndex = 0;
                try
                {
                    await LoadAndDisplay(currentIndex);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });

            animationClock = new Timer(new TimerCallback(OnReanimate), null, 25, 25);

            ContentGrid.IsEnabled = true;
            LoadingGrid.Visibility = Visibility.Collapsed;
        }

        private async void BuildingViewer_Loaded(object sender, RoutedEventArgs e)
        {
            await Load();
        }

        private void OnReanimate(object? state)
        {
            currentFrame += animationSpeed * TimeSpan.FromMilliseconds(25).TotalSeconds;
            Dispatcher.InvokeAsync(() => LoadAndDisplay(currentIndex));
        }        

        private async Task LoadAndDisplay(int index)
        {
            var gfx = archive.Graphics.Values.ElementAt(index);
            if (gfx == null) return;
            currentResource = gfx;
            var loadFrame = (int)currentFrame % (gfx.Frames ?? 0);
            if (loadFrame != 0 && loadFrame == lastCurrentFrame) return;
            var src = await LoadOne(gfx, loadFrame);
            if (src == null) return;
            await Dispatcher.InvokeAsync(delegate
            {
                NameBlock.Text = gfx.Header.ImageName;
                ImageDetailsBlock.Text = gfx.ScrapeInformation();
                PreviewImage.Source = src;
                FrameBlock.Text = loadFrame.ToString();
                AnimationSpeedBlock.Text = animationSpeed.ToString();
            });
            lastCurrentFrame = loadFrame;
        }

        private async Task LoadAll()
        {
            imgDictionary.Clear();
            int index = -1;
            foreach(var graphic in archive.Graphics.Values)
            {
                index++;
                var source = await LoadOne(graphic,0);
                if (source == null) continue;                
                await Dispatcher.InvokeAsync(delegate
                {
                    var imgControl = new Image()
                    {
                        Source = source
                    };
                    var button = new Button()
                    {
                        Tag = index,
                        Margin = new Thickness(10, 10, 0, 0),
                        Width = 100,
                        Height = 100,
                        Padding = new Thickness(5),
                        Content = imgControl
                    };
                    button.Click += async delegate
                    {
                        currentIndex = (int)button.Tag;
                        await LoadAndDisplay(currentIndex);
                    };
                    Catelog.Children.Add(button);
                    RenderOptions.SetBitmapScalingMode(imgControl, BitmapScalingMode.NearestNeighbor);
                });
            }
        }

        private async Task<ImageSource?> LoadOne(SC2GraphicResource resource, int frame)
        {
            if (archive == null || renderer == null)
                return null;
            var name = resource.Header.ImageName + $"_{frame}";
            if (InteropBitmapCache._bitmaps.TryGetValue(name, out var imageS))
                return imageS;
            System.Drawing.Bitmap image = null;
            await Task.Run(delegate
            {
                image = renderer.Render(resource, frame);
            });
            if (image is null) return null;
            ImageSource interopBitmap = null;
            await Dispatcher.InvokeAsync(() => interopBitmap = InteropBitmapCache.ToInteropBitmap(name, image));            
            return interopBitmap;
        }

        private async void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            currentIndex++;
            try
            {
                await LoadAndDisplay(currentIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                currentIndex--;
            }
        }

        private async void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            currentIndex--;
            try
            {
                await LoadAndDisplay(currentIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                currentIndex++;
            }
        }

        private async void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                InitialDirectory = @"E:\Games\SC2K\GAME\Data\",
                Multiselect = false,
                Title = "Open *.DAT File"
            };
            if (dialog.ShowDialog() ?? false)
            {
                DATFilePath = new Uri(dialog.FileName);
                await Load();
            }            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedGraphic = currentResource;
            if (selectedGraphic == default) return;

            StackPanel windStack = new StackPanel()
            {
                Margin = new Thickness(10)
            };
            Window window = new Window()
            {
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStyle = WindowStyle.None,
                Content = new Button()
                {
                    Content = windStack,
                },
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Parent as Window ?? Application.Current.MainWindow
            };
            int scaleFactor = 10;
            Image previewImage = new Image()
            {
                Width = selectedGraphic.Width.Value * scaleFactor,
                Height = selectedGraphic.Height.Value * scaleFactor,
                Stretch = Stretch.Fill
            };
            previewImage.SetBinding(Image.SourceProperty, new Binding()
            {
                Source = PreviewImage,
                Path = new PropertyPath(Image.SourceProperty)
            });
            RenderOptions.SetBitmapScalingMode(previewImage, BitmapScalingMode.NearestNeighbor);

            Border imgBorder = new Border()
            {
                Background = Brushes.Magenta,
                Child = previewImage
            };
            windStack.Children.Add(imgBorder);
            var button = new Button()
            {
                IsDefault = true,
                Content = "OK",
                Width = 150,
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(0, 10, 0, 0),
            };
            button.Click += delegate { window.Close(); };
            windStack.Children.Add(button);

            window.Show();
        }

        private void ButtonSlower_Click(object sender, RoutedEventArgs e)
        {
            animationSpeed -= .25;
            AnimationSpeedBlock.Text = animationSpeed.ToString();
        }

        private void ButtonFaster_Click(object sender, RoutedEventArgs e)
        {
            animationSpeed += .25;
            AnimationSpeedBlock.Text = animationSpeed.ToString();
        }
    }
}
