// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    // Define a class for a bounding box
    public class BoundingBox
    {
        public Vector3f Min { get; private set; }
        public Vector3f Max { get; private set; }

        // Constructor to initialize the bounding box
        public BoundingBox(Vector3f min, Vector3f max)
        {
            Min = min;
            Max = max;
        }

        // Create a bounding box that surrounds two other bounding boxes
        public static BoundingBox SurroundingBox(BoundingBox box0, BoundingBox box1)
        {
            Vector3f small = new Vector3f(Math.Min(box0.Min.X, box1.Min.X),
                                          Math.Min(box0.Min.Y, box1.Min.Y),
                                          Math.Min(box0.Min.Z, box1.Min.Z));

            Vector3f big = new Vector3f(Math.Max(box0.Max.X, box1.Max.X),
                                        Math.Max(box0.Max.Y, box1.Max.Y),
                                        Math.Max(box0.Max.Z, box1.Max.Z));

            return new BoundingBox(small, big);
        }

        // Check if a ray intersects with the bounding box
        public bool Intersects(Ray ray, double tMin, double tMax)
        {
            for (int a = 0; a < 3; a++)
            {
                double invD = 1.0 / ray.Direction[a];
                double t0 = (Min[a] - ray.Origin[a]) * invD;
                double t1 = (Max[a] - ray.Origin[a]) * invD;

                if (invD < 0.0)
                {
                    double temp = t0;
                    t0 = t1;
                    t1 = temp;
                }

                tMin = t0 > tMin ? t0 : tMin;
                tMax = t1 < tMax ? t1 : tMax;

                if (tMax <= tMin)
                    return false;
            }
            return true;
        }

        // Calculate the surface area of the bounding box
        public double GetSurfaceArea()
        {
            Vector3f diff = Max - Min;
            return 2 * (diff.X * diff.Y + diff.X * diff.Z + diff.Y * diff.Z);
        }
    }
}
