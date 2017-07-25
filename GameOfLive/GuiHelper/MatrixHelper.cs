using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GuiHelper
{
    public static class MatrixHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bitmap ConvertToBitmap(bool[,] board, Color background, Color foreground)
        {
            // Create 2D array of integers
            int width = board.GetLength(0);
            int height = board.GetLength(1);
            int stride = width * 4;
            int[,] integers = new int[width, height];

            // http://stackoverflow.com/questions/5113919/how-to-convert-2-d-array-into-image-in-c-sharp
            var black = background.ToArgb();
            var white = foreground.ToArgb();
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    integers[x, y] = board[x, y] ? white : black;
                }
            }

            // Copy into bitmap
            Bitmap bitmap;
            unsafe
            {
                fixed (int* intPtr = &integers[0, 0])
                {
                    bitmap = new Bitmap(width, height, stride, PixelFormat.Format32bppArgb, new IntPtr(intPtr));
                }
            }

            return bitmap;
        }
    }
}
