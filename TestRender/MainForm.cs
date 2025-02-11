using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace RayTracerGUI
{
    class Program
    {
        static void Main()
        {
            int width = 800;
            int height = 600;

            // Initialize the rendered image buffer
            RenderedImage renderedImage = new RenderedImage(width, height);

            // Initialize the scene with a house
            Scene scene = InitializeScene();

            // Set up camera parameters
            Vector3f cameraPos = new Vector3f(0, 0, -5);
            Vector3f cameraDir = new Vector3f(0, 0, 1);
            Vector3f up = new Vector3f(0, 1, 0);
            Vector3f right = new Vector3f(1, 0, 0);
            double fov = 90;
            int samplesPerPixel = 4;
            Random random = new Random();

            // Initialize RayTracer
            RayTracer rayTracer = new RayTracer(scene);

            // Initialize Render2D with required parameters
            Render2D renderer = new Render2D(rayTracer, renderedImage, random, up, right, cameraDir, fov, cameraPos, samplesPerPixel);

            // **Invoke Render2D per pixel**
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    renderer.Invoke(x, y);  // Ray tracing at (x, y)
                }
            }

            // Save the rendered image
            SaveRenderedImage(renderedImage, "house_rendered.png");
        }

        static Scene InitializeScene()
        {
            Scene scene = new Scene();

            // Define house structure using triangles
            Vector3f houseColor = new Vector3f(0.6f, 0.3f, 0.1f);
            Vector3f roofColor = new Vector3f(0.8f, 0.2f, 0.2f);
            Vector3f doorColor = new Vector3f(0.3f, 0.2f, 0.1f);

            // Base of the house (square made of two triangles)
            scene.AddTriangle(new Triangle(new Vector3f(-1, -1, 3), new Vector3f(1, -1, 3), new Vector3f(1, 1, 3), new Material(houseColor, 0.5, 0.1)));
            scene.AddTriangle(new Triangle(new Vector3f(-1, -1, 3), new Vector3f(1, 1, 3), new Vector3f(-1, 1, 3), new Material(houseColor, 0.5, 0.1)));

            // Roof (triangle)
            scene.AddTriangle(new Triangle(new Vector3f(-1.2f, -1, 3), new Vector3f(1.2f, -1, 3), new Vector3f(0, -2, 3), new Material(roofColor, 0.5, 0.1)));

            // Door (square made of two triangles)
            scene.AddTriangle(new Triangle(new Vector3f(-0.3f, 1, 3), new Vector3f(0.3f, 1, 3), new Vector3f(0.3f, 0.3f, 3), new Material(doorColor, 0.5, 0.1)));
            scene.AddTriangle(new Triangle(new Vector3f(-0.3f, 1, 3), new Vector3f(0.3f, 0.3f, 3), new Vector3f(-0.3f, 0.3f, 3), new Material(doorColor, 0.5, 0.1)));

            // Light source
            scene.AddLight(new Light(new Vector3f(0, 5, 5), new Vector3f(1, 1, 1)));

            return scene;
        }

        static void SaveRenderedImage(RenderedImage renderedImage, string filePath)
        {
            int width = renderedImage.Width;
            int height = renderedImage.Height;

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // PPM Header (P3 format: Plain text, ASCII)
                writer.WriteLine("P3");
                writer.WriteLine($"{width} {height}");
                writer.WriteLine("255"); // Maximum color value

                // Write pixel data
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Vector3f color = renderedImage.GetPixel(x, y);

                        // Clamp and convert to 0-255 range
                        int r = (int)(Math.Min(1.0, Math.Max(0.0, color.X)) * 255);
                        int g = (int)(Math.Min(1.0, Math.Max(0.0, color.Y)) * 255);
                        int b = (int)(Math.Min(1.0, Math.Max(0.0, color.Z)) * 255);

                        // Write RGB values to file
                        writer.WriteLine($"{r} {g} {b}");
                    }
                }
            }

            Console.WriteLine($"PPM image saved to {filePath}");
        }
    }
}
