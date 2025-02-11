// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    // Define a static helper class for parallel processing
    public static class ParallelHelper
    {
        // Execute an action for each element in a 2D range
        public static void For2D(int startY, int endY, int startX, int endX, IAction2D action)
        {
            int batchHeight = (endY - startY) / Environment.ProcessorCount;

            Parallel.For(0, Environment.ProcessorCount, i =>
            {
                int currentStartY = startY + i * batchHeight;
                int currentEndY = (i == Environment.ProcessorCount - 1) ? endY : currentStartY + batchHeight;

                for (int y = currentStartY; y <= currentEndY; y++)
                {
                    for (int x = startX; x <= endX; x++)
                    {
                        action.Invoke(x, y);
                    }
                }
            });
        }
    }
}
