using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Catalog.Application
{
    public static class DependencyRegistrator
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            return services;
        }
    }
}
