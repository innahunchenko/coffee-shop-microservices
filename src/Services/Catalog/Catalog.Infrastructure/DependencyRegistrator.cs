using Catalog.Application;
using Catalog.Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Catalog.Domain.Repositories.Products;
using Catalog.Infrastructure.Data;
using Catalog.Domain.Services.Products;
using Catalog.Infrastructure.Services.Products;
using RedisCachingService;
using Catalog.Infrastructure.Repositories.Categories;
using Catalog.Domain.Repositories.Categories;
using Catalog.Domain.Services.Categories;
using Catalog.Infrastructure.Services.Categories;
using Microsoft.Data.SqlClient;
using System.Data;
using StackExchange.Redis;

namespace Catalog.Infrastructure
{
    public static class DependencyRegistrator
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //var redisConnectionString = configuration.GetValue<string>("CacheSettings:RedisConnectionString");
            //Console.WriteLine(redisConnectionString);
            //var expiryTime = TimeSpan.FromMinutes(configuration.GetValue<double>("CacheSettings:DefaultCacheDurationMinutes"));
            //var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString!);
            //services.AddSingleton(connectionMultiplexer);
            //services.AddScoped<IRedisCacheRepository, RedisCacheRepository>(provider =>
            //{
            //    var multiplexer = provider.GetRequiredService<ConnectionMultiplexer>();
            //    return new RedisCacheRepository(multiplexer, expiryTime);
            //});

            var connectionString = configuration.GetConnectionString("ProductsConnection");
            Console.WriteLine(connectionString);
            services.AddHttpContextAccessor();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();
            //services.AddScoped<IProductCacheService, ProductCacheService>();
            //services.Decorate<IProductService, ProductServiceCacheDecorator>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            //services.AddScoped<ICategoryCacheService, CategoryCacheService>();
            //services.Decorate<ICategoryService, CategoryServiceCacheDecorator>();

            services.AddScoped<ISaveChangesInterceptor, SaveEntityInterceptor>();
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IAppDbContext, AppDbContext>();
            services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

         //   services.AddHostedService<DbWarmupService>();

            return services;
        }
    }
}
