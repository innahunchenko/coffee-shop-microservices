using Catalog.Application.Mapping;
using MapsterMapper;
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

            var mapsterConfig = MapsterConfiguration.Configure();
            services.AddSingleton(mapsterConfig);
            services.AddScoped<IMapper>(sp => new Mapper(mapsterConfig));

            return services;
        }
    }
}
