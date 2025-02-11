// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    // Define a class for the camera
    public partial class Camera
    {
        private readonly Vector3f viewPosition;
        private readonly double viewAngle;
        private readonly Vector3f viewDirection;
        private readonly Vector3f right;
        private readonly Vector3f up;

        // Constructor to initialize the camera from an input buffer
        public Camera(Vector3f pos, Vector3f dir, double angle)
        {
            viewPosition = pos;
            viewDirection = dir;
            viewDirection = viewDirection.IsZero() ? Vector3f.OneZ : viewDirection;
            viewAngle = angle * (Math.PI / 180.0);

            right = Vector3f.Unitize(Vector3f.Cross(Vector3f.OneY, viewDirection));
            if (right.IsZero())
            {
                right = Vector3f.Unitize(Vector3f.Cross(viewDirection, Vector3f.OneX));
            }

            up = Vector3f.Unitize(Vector3f.Cross(viewDirection, right));
        }

        // Property for the camera's eye point (position)
        public Vector3f EyePoint => viewPosition;

        // Render a frame of the scene
        public RenderedImage Frame(Scene scene, RenderedImage renderedImage, Random random)
        {
            var rayTracer = new RayTracer(scene);
            var action = new Render2D(rayTracer, renderedImage, new Random(), up, right, viewDirection, viewAngle, viewPosition);

            ParallelHelper.For2D(0, renderedImage.Height - 1, 0, renderedImage.Width - 1, action);

            return renderedImage;
        }
    }
}
