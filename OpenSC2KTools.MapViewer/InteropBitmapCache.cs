using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OpenSC2KTools.MapViewer
{
    internal static class InteropBitmapCache
    {
        public static ConcurrentDictionary<string, ImageSource> _bitmaps = new();

        //If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hObject);

        private static ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {            
            var handle = bmp.GetHbitmap();
            try
            {                
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        public static ImageSource ToInteropBitmap(string Name, Bitmap Bitmap)
        {
            if (_bitmaps.TryGetValue(Name, out var bitmap))
                return bitmap;
            var source = ImageSourceFromBitmap(Bitmap);  
            _bitmaps.TryAdd(Name, source); 
            return source;
        }
    }
}
