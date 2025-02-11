// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    // Define a static helper class for input parsing
    public static class Scanf
    {
        // Read a line from a text reader
        public static string GetLine(TextReader reader)
        {
            return reader.ReadLine();
        }

        // Read a vector from a text reader
        public static Vector3f Read(TextReader reader)
        {
            string[] parts = reader.ReadLine().Split();
            return new Vector3f(double.Parse(parts[0]), double.Parse(parts[1]), double.Parse(parts[2]));
        }
    }
}
