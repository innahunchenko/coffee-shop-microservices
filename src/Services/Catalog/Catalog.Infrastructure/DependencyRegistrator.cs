using Catalog.Application;
using Catalog.Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Catalog.Infrastructure.Repositories.Products;
using Catalog.Domain.Repositories.Products;
using Catalog.Infrastructure.Data;
using Catalog.Domain.Services.Products;
using Catalog.Infrastructure.Services.Products;
using RedisCachingService;
using Catalog.Infrastructure.Repositories.Categories;
using Catalog.Domain.Repositories.Categories;
using Catalog.Domain.Services.Categories;
using Catalog.Infrastructure.Services.Categories;

namespace Catalog.Infrastructure
{
    public static class DependencyRegistrator
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ProductsConnection");
            services.AddHttpContextAccessor();
            services.AddScoped<IRedisCacheRepository, RedisCacheRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();
            services.Decorate<IProductService, ProductServiceCacheDecorator>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.Decorate<ICategoryService, CategoryServiceCacheDecorator>();

            services.AddScoped<ISaveChangesInterceptor, SaveEntityInterceptor>();
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IAppDbContext, AppDbContext>();

            return services;
        }
    }
}
