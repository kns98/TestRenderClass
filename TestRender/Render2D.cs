// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{

        // Define a nested class to render a 2D image
        class Render2D : IAction2D
        {
            private readonly RayTracer rayTracer;
            private readonly RenderedImage renderedImage;
            private readonly Random rand;
            private readonly Vector3f up;
            private readonly Vector3f right;
            private readonly Vector3f dir;
            private readonly double ang;
            private readonly Vector3f pos;
            private int count;
            private readonly int total;
            private readonly int samplesPerPixel;

            // Constructor to initialize the render action
            public Render2D(RayTracer r, RenderedImage i, Random ra, Vector3f up, Vector3f right, Vector3f dir, double ang, Vector3f pos, int samplesPerPixel = 4)
            {
                rayTracer = r;
                renderedImage = i;
                rand = ra;
                this.up = up;
                this.right = right;
                this.dir = dir;
                this.ang = ang;
                this.pos = pos;
                this.samplesPerPixel = samplesPerPixel;
                total = (renderedImage.Height - 1) * (renderedImage.Width - 1);
            }

            // Execute the rendering action for each pixel
            public void Invoke(int x, int y)
            {
                Vector3f color = Vector3f.Zero;

                for (int i = 0; i < samplesPerPixel; i++)
                {
                    double u = (x + rand.NextDouble()) / renderedImage.Width;
                    double v = (y + rand.NextDouble()) / renderedImage.Height;

                    double f1 = (u * 2 - 1) * Math.Tan(ang * 0.5) * renderedImage.AspectRatio;
                    double num1 = (v * 2 - 1) * Math.Tan(ang * 0.5);

                    var V_2 = right * f1 + up * num1;
                    var V_3 = Vector3f.Unitize(dir + V_2);
                    color += rayTracer.Radiance(pos, V_3, rand);
                }

                color /= samplesPerPixel;
                renderedImage.AddToPixel(x, y, color);

                int num2 = Interlocked.Increment(ref count);
                if (num2 % 100000 == 0)
                {
                    int V_7 = num2 / (total / 100);
                    Console.WriteLine($"{V_7} % of pixels processed: {DateTime.Now}");
                }
            }
        }
    }

