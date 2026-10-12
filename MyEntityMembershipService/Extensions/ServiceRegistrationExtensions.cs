namespace MyEntityMembershipService.Extensions
{

    public static class ServiceRegistrationExtensions
    {
        public static void AddServicesByConvention(
            this IServiceCollection services,
            HashSet<string>? classSuffixes = null,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            classSuffixes ??= new HashSet<string> { "Service", "Repository", "service", "repository" };

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic &&
                            !a.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase) &&
                            !a.FullName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) &&
                            !a.FullName.StartsWith("MassTransit",StringComparison.OrdinalIgnoreCase))
                .ToList();

            var serviceTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    classSuffixes.Any(suffix =>
                        t.Name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
                .Select(t => new
                {
                    Implementation = t,
                    Interface = t.GetInterface($"I{t.Name}")
                })
                .Where(x => x.Interface != null &&
                            !services.Any(s => s.ServiceType == x.Interface))
                .ToList();




            foreach (var type in serviceTypes)
            {

                services.Add(new ServiceDescriptor(type.Interface, type.Implementation, lifetime));

            }

        }
    }


}


