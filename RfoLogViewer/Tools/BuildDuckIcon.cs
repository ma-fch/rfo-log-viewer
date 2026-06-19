using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

public static class BuildDuckIcon
{
    public static void Run()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var projectDir = Path.Combine(baseDir, "RfoLogViewer");
        if (!Directory.Exists(projectDir))
        {
            projectDir = baseDir;
        }

        Run(projectDir);
    }

    public static void Run(string projectDir)
    {
        var sourcePath = Path.Combine(projectDir, "duck-icon-source.png");
        var squarePath = Path.Combine(projectDir, "duck-icon-square.png");
        var iconPath = Path.Combine(projectDir, "duck.ico");

        if (!File.Exists(sourcePath))
        {
            Console.Error.WriteLine("Source image not found: " + sourcePath);
            Environment.Exit(1);
        }

        using (var source = Image.FromFile(sourcePath))
        using (var square = CropToSquareContent(source))
        {
            square.Save(squarePath, ImageFormat.Png);
            SaveWin32Icon(square, iconPath, new[] { 16, 32, 48, 256 });
        }

        Console.WriteLine("Created " + squarePath);
        Console.WriteLine("Created " + iconPath);
    }

    private static Bitmap CropToSquareContent(Image source)
    {
        using (var working = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb))
        {
            using (var g = Graphics.FromImage(working))
            {
                g.DrawImage(source, 0, 0, source.Width, source.Height);
            }

            var bounds = GetContentBounds(working);
            var contentWidth = bounds.Right - bounds.Left + 1;
            var contentHeight = bounds.Bottom - bounds.Top + 1;
            var side = Math.Max(contentWidth, contentHeight);
            var centerX = (bounds.Left + bounds.Right) / 2.0;
            var centerY = (bounds.Top + bounds.Bottom) / 2.0;
            var cropX = (int)Math.Round(centerX - side / 2.0);
            var cropY = (int)Math.Round(centerY - side / 2.0);

            if (cropX < 0)
            {
                cropX = 0;
            }

            if (cropY < 0)
            {
                cropY = 0;
            }

            if (cropX + side > working.Width)
            {
                cropX = working.Width - side;
            }

            if (cropY + side > working.Height)
            {
                cropY = working.Height - side;
            }

            if (cropX < 0)
            {
                cropX = 0;
            }

            if (cropY < 0)
            {
                cropY = 0;
            }

            if (cropX + side > working.Width)
            {
                side = working.Width - cropX;
            }

            if (cropY + side > working.Height)
            {
                side = working.Height - cropY;
            }

            var square = new Bitmap(side, side, PixelFormat.Format32bppArgb);
            using (var g2 = Graphics.FromImage(square))
            {
                g2.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g2.SmoothingMode = SmoothingMode.AntiAlias;
                g2.DrawImage(
                    working,
                    new Rectangle(0, 0, side, side),
                    new Rectangle(cropX, cropY, side, side),
                    GraphicsUnit.Pixel);
            }

            return square;
        }
    }

    private static Rectangle GetContentBounds(Bitmap bitmap)
    {
        var minX = bitmap.Width;
        var minY = bitmap.Height;
        var maxX = 0;
        var maxY = 0;
        var found = false;

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var color = bitmap.GetPixel(x, y);
                if (IsEmptyPixel(color))
                {
                    continue;
                }

                found = true;
                if (x < minX)
                {
                    minX = x;
                }

                if (y < minY)
                {
                    minY = y;
                }

                if (x > maxX)
                {
                    maxX = x;
                }

                if (y > maxY)
                {
                    maxY = y;
                }
            }
        }

        if (!found)
        {
            return new Rectangle(0, 0, bitmap.Width - 1, bitmap.Height - 1);
        }

        return Rectangle.FromLTRB(minX, minY, maxX, maxY);
    }

    private static bool IsEmptyPixel(Color color)
    {
        return color.A < 16 || (color.R > 245 && color.G > 245 && color.B > 245);
    }

    private static void SaveWin32Icon(Bitmap square, string iconPath, int[] sizes)
    {
        using (var stream = new MemoryStream())
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)sizes.Length);

            var offset = 6 + (16 * sizes.Length);
            var dibImages = new byte[sizes.Length][];

            for (var i = 0; i < sizes.Length; i++)
            {
                var size = sizes[i];
                using (var resized = new Bitmap(size, size, PixelFormat.Format32bppArgb))
                {
                    using (var g = Graphics.FromImage(resized))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.Clear(Color.Transparent);
                        g.DrawImage(square, 0, 0, size, size);
                    }

                    dibImages[i] = CreateDibImage(resized);
                }

                var dim = size >= 256 ? (byte)0 : (byte)size;
                writer.Write(dim);
                writer.Write(dim);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)1);
                writer.Write((byte)0);
                writer.Write((ushort)32);
                writer.Write((uint)dibImages[i].Length);
                writer.Write((uint)offset);
                offset += dibImages[i].Length;
            }

            foreach (var dib in dibImages)
            {
                writer.Write(dib);
            }

            writer.Flush();
            File.WriteAllBytes(iconPath, stream.ToArray());
        }
    }

    private static byte[] CreateDibImage(Bitmap bitmap)
    {
        var size = bitmap.Width;
        var rect = new Rectangle(0, 0, size, size);
        var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        try
        {
            var stride = data.Stride;
            var rawLength = Math.Abs(stride) * size;
            var raw = new byte[rawLength];
            System.Runtime.InteropServices.Marshal.Copy(data.Scan0, raw, 0, rawLength);

            var andRowBytes = (int)Math.Ceiling(size / 8.0);
            var andMaskLength = andRowBytes * size;
            var xorLength = size * size * 4;
            var dib = new byte[40 + xorLength + andMaskLength];

            using (var ms = new MemoryStream(dib))
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(40);
                writer.Write(size);
                writer.Write(size * 2);
                writer.Write((short)1);
                writer.Write((short)32);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);

                for (var y = size - 1; y >= 0; y--)
                {
                    var rowStart = y * stride;
                    for (var x = 0; x < size; x++)
                    {
                        var index = rowStart + (x * 4);
                        writer.Write(raw[index]);
                        writer.Write(raw[index + 1]);
                        writer.Write(raw[index + 2]);
                        writer.Write(raw[index + 3]);
                    }
                }

                for (var i = 0; i < andMaskLength; i++)
                {
                    writer.Write((byte)0);
                }
            }

            return dib;
        }
        finally
        {
            bitmap.UnlockBits(data);
        }
    }
}
