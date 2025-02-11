// Define the namespace for the Ray Tracer GUI application
namespace RayTracerGUI
{
    // Define a class for the ray tracer
    public class RayTracer
    {
        private readonly Scene scene;
        private readonly int maxDepth;
        private readonly int samplesPerPixel;

        // Constructor to initialize the ray tracer with a scene, max depth, and samples per pixel
        public RayTracer(Scene scene, int maxDepth = 5, int samplesPerPixel = 100)
        {
            this.scene = scene;
            this.maxDepth = maxDepth;
            this.samplesPerPixel = samplesPerPixel;
        }

        // Calculate the radiance at a given position and direction
        public Vector3f Radiance(Vector3f position, Vector3f direction, Random random, int depth = 0)
        {
            if (depth > maxDepth)
                return Vector3f.Zero;

            Ray ray = new Ray(position, direction);
            if (scene.Intersect(ray, out Triangle hitTriangle, out double hitDistance))
            {
                Vector3f hitPoint = ray.Origin + ray.Direction * hitDistance;
                Material material = hitTriangle.GetMaterial();
                Vector3f normal = hitTriangle.Normal();
                double u, v;
                hitTriangle.Intersect(ray, out _, out u, out v);
                Vector3f color = material.Emissivity * material.GetColor(u, v);

                Vector3f directLighting = Vector3f.Zero;
                foreach (var light in scene.GetLights())
                {
                    Vector3f lightDirection = Vector3f.Unitize(light.Position - hitPoint);
                    Ray shadowRay = new Ray(hitPoint + normal * 0.001, lightDirection);

                    if (!scene.Intersect(shadowRay, out _, out _))
                    {
                        double lambertian = Math.Max(0, Vector3f.Dot(normal, lightDirection));
                        Vector3f reflectionDirection = Vector3f.Reflect(-lightDirection, normal);
                        double specular = Math.Pow(Math.Max(0, Vector3f.Dot(reflectionDirection, direction)), 32);

                        // Get the material color at the intersection point (using UV coordinates)
                        Vector3f surfaceColor = material.GetColor(u, v);
                        // Compute the diffuse contribution (Lambertian shading)
                        Vector3f diffuseComponent = surfaceColor * lambertian;
                        // Compute the specular contribution (Blinn-Phong reflection)
                        Vector3f specularComponent = new Vector3f( specular, specular,specular) * material.Reflectivity;
                        // Compute total lighting contribution from this light source

                        // Multiply light color with diffuse component (Lambertian reflection)
                        Vector3f diffuseLighting = new Vector3f(
                            light.Color.X * diffuseComponent.X,
                            light.Color.Y * diffuseComponent.Y,
                            light.Color.Z * diffuseComponent.Z
                        );

                        // Multiply light color with specular component (Specular reflection)
                        Vector3f specularLighting = new Vector3f(
                            light.Color.X * specularComponent.X,
                            light.Color.Y * specularComponent.Y,
                            light.Color.Z * specularComponent.Z
                        );

                        // Sum both components to get the final lighting contribution
                        Vector3f lightingContribution = new Vector3f(
                            diffuseLighting.X + specularLighting.X,
                            diffuseLighting.Y + specularLighting.Y,
                            diffuseLighting.Z + specularLighting.Z
                        );



                        // Accumulate the direct lighting
                        directLighting += lightingContribution;

                    }
                }

                Vector3f indirectLighting = Vector3f.Zero;
                for (int i = 0; i < samplesPerPixel; i++)
                {
                    Vector3f randomDirection = RandomHemisphereDirection(normal, random);
                    indirectLighting += Radiance(hitPoint + normal * 0.001, randomDirection, random, depth + 1);
                }
                indirectLighting /= samplesPerPixel;

                color += directLighting + indirectLighting;
                return color;
            }

            return Vector3f.Zero;
        }

        // Generate a random direction within a hemisphere
        private Vector3f RandomHemisphereDirection(Vector3f normal, Random random)
        {
            double u = random.NextDouble();
            double v = random.NextDouble();
            double theta = 2 * Math.PI * u;
            double phi = Math.Acos(2 * v - 1);
            double x = Math.Sin(phi) * Math.Cos(theta);
            double y = Math.Sin(phi) * Math.Sin(theta);
            double z = Math.Cos(phi);
            Vector3f randomDirection = new Vector3f(x, y, z);
            return Vector3f.Dot(randomDirection, normal) > 0 ? randomDirection : -randomDirection;
        }
    }
}
