﻿using OpenSC2Kv2.API;
using OpenSC2Kv2.API.Graphics;
using OpenSC2Kv2.API.Graphics.Win95;
using OpenSC2Kv2.API.IFF;
using OpenSC2Kv2.API.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
    /// Interaction logic for MapViewer.xaml
    /// </summary>
    public partial class MapViewer : Page
    {
        bool ready => parser != null;
        IFFParser parser;
        SC2SpriteArchive archive;
        ImageSource tileAsset;
        Dictionary<SC2WorldTile, Image[]> tileMap = new();

        bool movingCamera = false;
        System.Windows.Point cameraStartPoint = new System.Windows.Point(0,0);

        public MapViewer()
        {
            InitializeComponent();

            Loaded += delegate
            {
                Load();                
            };
        }

        private void Load()
        {
            var extractor = new SPRExtractor(new Uri(SC2Path.GetSpecialPath(SC2Path.SpecialPath.LargeDatFilePath)));
            var result = extractor.ExtractAll();

            if (result.Value == null) return;
            archive = result.Value;

            //1756 -- MED VIEW TILE SIZE
            //var tileResource = archive.Graphics.Values.Where(x => x.Header.ID == 756).First();
            var tileResource = archive.Graphics.Values.First();
            if (tileResource == null) return;

            var image = SPRRender.Render(tileResource, 0);
            
            tileAsset = InteropBitmapCache.ToInteropBitmap(tileResource.Header.ImageName, image);
            LoadingDescBlock.Text = "Select a City...";
        }

        private async void Render(SC2World World)
        {
            if (!ready) return;

            RenderLoadBar.Value = 0;
            RenderLoadBar.IsIndeterminate = false;

            LoadingGrid.Visibility = Visibility.Visible;
            int totalHeight = World.Height * SC2WorldTile.TILE_HEIGHT;
            //set the default image
            var defaultImage = new System.Windows.Controls.Image()
            {
                Width = SC2WorldTile.TILE_WIDTH,
                Height = SC2WorldTile.TILE_HEIGHT,
                Source = tileAsset
            };
            BindingBase binding = new Binding()
            {
                Source = defaultImage,
                Path = new PropertyPath(System.Windows.Controls.Image.SourceProperty)
            };
            var palette = SC2Palette.Default;
            await Dispatcher.InvokeAsync(delegate
            {
                foreach (var tile in World.WorldTiles)
                {
                    Image[] images = null;
                    if (!tileMap.TryGetValue(tile, out images))
                    {
                        LoadingDescBlock.Text = "Decoding Graphics...";
                        RenderLoadBar.Maximum = World.WorldTiles.Count * 2;
                        var loadedImage = new System.Windows.Controls.Image()
                        {
                            Width = SC2WorldTile.TILE_WIDTH,
                            Height = SC2WorldTile.TILE_HEIGHT,
                            ToolTip = tile.ToString()

                        };
                        images = new Image[1];
                        images[0] = loadedImage;

                        bool success = false;
                        if (tile.TerrainDescription != null
                            && tile.TerrainDescription.TerrainID != default)
                        {
                            ushort textureID = (ushort)((tile.TerrainDescription.WaterID ?? tile.TerrainDescription.TerrainID) + 1000);
                            GetImageFromGraphicID(ref loadedImage, textureID, palette, out var graphicResource);
                            tile.TileWidth = graphicResource.Width.Value;
                            tile.TileHeight = graphicResource.Height.Value;
                            success = true;
                        }
                        if (!success)
                        {
                            images[0].SetBinding(System.Windows.Controls.Image.SourceProperty, binding);
                            images[0].Opacity = .5;
                        }
                    }
                    if (images == null || images[0] == null) return;
                    var imageCtrl = images[0];
                    var position = tile.GetWorldPos() + new Point2D(0, totalHeight / 2);

                    RenderOptions.SetBitmapScalingMode(imageCtrl, BitmapScalingMode.NearestNeighbor);
                    Canvas.SetLeft(imageCtrl, position.X);
                    Canvas.SetTop(imageCtrl, position.Y);

                    if (images.Length != 2 || images[1] == null)
                    {
                        //BUILDING
                        if (tile.BuildingDescription != null && tile.BuildingDescription.DescriptorID != 0x00)
                        {
                            bool multitile = tile.IsMultitile();
                            if (multitile && tile.ZoneDescription.CornerFlag is not SC2CornerFlag.TOP)
                                goto Skip;

                            ushort textureID = (ushort)(tile.BuildingDescription.TryGetGraphicID());
                            var accyBuildingImage = new System.Windows.Controls.Image()
                            {
                                Width = SC2WorldTile.TILE_WIDTH,
                                Height = SC2WorldTile.TILE_HEIGHT,
                                ToolTip = $"BUILDING INFO:\n {tile.BuildingDescription.ToString()}\n" +
                                $"ZONE INFO:\n{tile.ZoneDescription}"
                            };
                            GetImageFromGraphicID(ref accyBuildingImage, textureID, palette, out var graphicResource);
                            RenderOptions.SetBitmapScalingMode(accyBuildingImage, BitmapScalingMode.NearestNeighbor);
                            Array.Resize(ref images, 2);
                            images[1] = accyBuildingImage;
                        Skip:
                            ;
                        }
                    }

                    if (images.Length == 2 && images[1] != null)
                    {
                        var accyBuildingImage = images[1];
                        if (accyBuildingImage.Width > SC2WorldTile.TILE_WIDTH)
                        {
                            var diff = accyBuildingImage.Width - SC2WorldTile.TILE_WIDTH;
                            position.X -= (int)(diff / 2);
                        }
                        position.Y -= (int)accyBuildingImage.Height;
                        position.Y += tile.TileHeight;
                        Canvas.SetLeft(accyBuildingImage, (position.X));
                        Canvas.SetTop(accyBuildingImage, position.Y);
                    }

                    imageCtrl.MouseLeftButtonUp += delegate
                    {
                        DoUpdateOverridesWizard(tile);
                    };
                    if (!tileMap.ContainsKey(tile))
                        tileMap.Add(tile, images);
                    RenderLoadBar.Value++;
                }
            });
            await Dispatcher.InvokeAsync(delegate
            {
                int pass = 0;
                LoadingDescBlock.Text = "Rendering...";
                foreach (var images in tileMap.Values)
                {
                    foreach (var img in images)
                    {
                        if (img.Parent == null)
                            MapView.Children.Add(img);
                    }
                    RenderLoadBar.Value++;
                }
                return;
                pass++;
                foreach (var images in tileMap.Values)
                {
                    if (images.Length == 2 && images[1] != null)
                    {
                        var img = images[pass];
                        if (img.Parent == null)
                            MapView.Children.Add(img);
                        RenderLoadBar.Value++;
                    }
                }
            });
            MapView.Width = World.Width * SC2WorldTile.TILE_WIDTH;
            MapView.Height = totalHeight;
            LoadingGrid.Visibility = Visibility.Collapsed;
        }

        internal void GetImageFromGraphicID(ref System.Windows.Controls.Image imageCtrl, ushort textureID, SC2Palette palette, out SC2GraphicResource graphicResource)
        {
            if (archive.TryGetGraphicByID(textureID, out graphicResource))
            {
                if (!InteropBitmapCache._bitmaps.TryGetValue(graphicResource.Header.ImageName, out var imageSource))
                {
                    var image = SPRRender.Render(graphicResource, 0, palette);
                    imageSource = InteropBitmapCache.ToInteropBitmap(graphicResource.Header.ImageName, image);
                }
                imageCtrl.Width = graphicResource.Width.Value;
                imageCtrl.Height = graphicResource.Height.Value;
                
                imageCtrl.Source = imageSource;                
            }
        }

        internal void Attach(IFFParser parser)
        {
            if (!parser.ParseSuccessful) return;
            this.parser = parser;

            if (ready)
            {
                MapView.Children.Clear();
                tileMap.Clear();
                Render(parser.LoadedWorld);
            }
        }

        private void Page_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var thickness = MapViewBox.Margin;
            double left = thickness.Left - (e.Delta * 3);
            double top = thickness.Top - (e.Delta * 2);
            double right = thickness.Right - (e.Delta * 3);
            double bot = thickness.Bottom - (e.Delta * 2);
            thickness = new Thickness(left, top, right, bot);
            MapViewBox.Margin = thickness;
        }

        private void Page_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            cameraStartPoint = e.GetPosition(this);
            movingCamera = true;
        }

        private void Page_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!movingCamera)
                return;
            var delta = e.GetPosition(this) - cameraStartPoint;
            var thickness = MapViewBox.Margin;
            var left = thickness.Left - (delta.X / 2);
            var right = thickness.Right + (delta.X / 2);
            var top = thickness.Top - (delta.Y / 2);
            var bot = thickness.Bottom + (delta.Y / 2);
            MapViewBox.Margin = new Thickness(left, top, right, bot);
        }

        private void Page_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            movingCamera = false;
        }

        private void Page_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.R:
                    MapView.Margin = new Thickness(0);
                    break;
            }
        }

        private async void DoUpdateOverridesWizard(SC2WorldTile Target)
        {
            var window = new TileProperties(Target.OverrideDefinition);
            if (window.ShowDialog() ?? false)
            {
                SC2WorldOverrides.AddOverride(Target, window.EditingDef);
                SC2WorldOverrides.SaveChanges();
                await SC2WorldOverrides.EnsureUpdated();
                ReticulatePositions();
            }
        }

        private void ReticulatePositions()
        {
            Render(parser.LoadedWorld);
        }

        private void DbgTilePosItem_Click(object sender, RoutedEventArgs e)
        {
            ReticulatePositions();
        }
    }
}
