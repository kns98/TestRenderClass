// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    // Define a class for a bounding volume hierarchy (BVH) node
    public class BVHNode
    {
        public BVHNode Left { get; private set; }
        public BVHNode Right { get; private set; }
        public BoundingBox Box { get; private set; }
        public Triangle Triangle { get; private set; }

        // Constructor to create a BVH node from a list of triangles
        public BVHNode(List<Triangle> triangles, int start, int end)
        {
            var objects = new List<Triangle>(triangles);
            int axis = GetBestAxisToSplit(objects, start, end);
            objects.Sort((a, b) => a.GetBoundingBox().Min[axis].CompareTo(b.GetBoundingBox().Min[axis]));

            int objectSpan = end - start;

            if (objectSpan == 1)
            {
                Left = Right = new BVHNode(objects[start]);
            }
            else if (objectSpan == 2)
            {
                Left = new BVHNode(objects[start]);
                Right = new BVHNode(objects[start + 1]);
            }
            else
            {
                int mid = start + objectSpan / 2;
                Left = new BVHNode(objects, start, mid);
                Right = new BVHNode(objects, mid, end);
            }

            Box = BoundingBox.SurroundingBox(Left.Box, Right.Box);
        }

        // Determine the best axis to split the BVH node
        private int GetBestAxisToSplit(List<Triangle> triangles, int start, int end)
        {
            int bestAxis = 0;
            double bestCost = double.MaxValue;
            for (int axis = 0; axis < 3; axis++)
            {
                triangles.Sort((a, b) => a.GetBoundingBox().Min[axis].CompareTo(b.GetBoundingBox().Min[axis]));
                BoundingBox boxLeft = triangles[start].GetBoundingBox();
                BoundingBox boxRight = triangles[end - 1].GetBoundingBox();
                double cost = 0;
                for (int i = start + 1; i < end; i++)
                {
                    boxLeft = BoundingBox.SurroundingBox(boxLeft, triangles[i].GetBoundingBox());
                    boxRight = BoundingBox.SurroundingBox(boxRight, triangles[i].GetBoundingBox());
                    double surfaceAreaLeft = boxLeft.GetSurfaceArea();
                    double surfaceAreaRight = boxRight.GetSurfaceArea();
                    cost = surfaceAreaLeft * (i - start) + surfaceAreaRight * (end - i);
                }
                if (cost < bestCost)
                {
                    bestAxis = axis;
                    bestCost = cost;
                }
            }
            return bestAxis;
        }

        // Constructor to create a leaf BVH node for a single triangle
        private BVHNode(Triangle triangle)
        {
            Triangle = triangle;
            Box = triangle.GetBoundingBox();
        }

        // Check if a ray intersects with the BVH node
        public bool Intersect(Ray ray, double tMin, double tMax, out Triangle hitTriangle, out double hitDistance)
        {
            hitTriangle = null;
            hitDistance = double.MaxValue;
            if (!Box.Intersects(ray, tMin, tMax))
                return false;

            bool hitLeft = Left?.Intersect(ray, tMin, tMax, out Triangle leftTriangle, out double leftDistance) ?? false;
            bool hitRight = Right?.Intersect(ray, tMin, tMax, out Triangle rightTriangle, out double rightDistance) ?? false;

            if (hitLeft && hitRight)
            {
                if (leftDistance < rightDistance)
                {
                    hitTriangle = leftTriangle;
                    hitDistance = leftDistance;
                }
                else
                {
                    hitTriangle = rightTriangle;
                    hitDistance = rightDistance;
                }
                return true;
            }

            if (hitLeft)
            {
                hitTriangle = leftTriangle;
                hitDistance = leftDistance;
                return true;
            }

            if (hitRight)
            {
                hitTriangle = rightTriangle;
                hitDistance = rightDistance;
                return true;
            }

            return false;
        }
    }
}
