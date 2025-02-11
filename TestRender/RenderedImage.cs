using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{

    // Define a class for the rendered image
    public class RenderedImage
    {
        private readonly int width;
        private readonly int height;
        private readonly Vector3f[,] pixels;

        // Constructor to initialize the rendered image with dimensions
        public RenderedImage(int width, int height)
        {
            this.width = width;
            this.height = height;
            pixels = new Vector3f[width, height];
        }

        // Properties for width, height, and aspect ratio
        public int Width => width;
        public int Height => height;
        public double AspectRatio => (double)width / height;

        // Add a color to a specific pixel
        public void AddToPixel(int x, int y, Vector3f color)
        {
            pixels[x, y] = color;
        }

        // Get the color of a specific pixel
        public Vector3f GetPixel(int x, int y)
        {
            return pixels[x, y];
        }

        // Save the rendered image as a PPM file
        public void SaveAsPPM(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine($"P3\n{width} {height}\n255");
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Vector3f color = pixels[x, y];
                        int r = (int)(Math.Min(1.0, color.X) * 255);
                        int g = (int)(Math.Min(1.0, color.Y) * 255);
                        int b = (int)(Math.Min(1.0, color.Z) * 255);
                        writer.WriteLine($"{r} {g} {b}");
                    }
                }
            }
        }
    }
}
