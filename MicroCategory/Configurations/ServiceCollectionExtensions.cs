using MicroCategory.Domain.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

namespace MicroCategory.Configurations
{
    /// <summary>
    /// ServiceCollection Extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add ConnectionString
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddConnectionString(this IServiceCollection services, IConfiguration configuration)
        {
            // get default connection
            var strDDConnect = configuration.GetConnectionString("DefaultConnection");

            // add dbContext Postgresql database
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseNpgsql(strDDConnect, o => o.UseNetTopologySuite());
            });

            // AddUnitOfWork 
            //services.AddUnitOfWork<DatabaseContext>();

            // connection retry settings
            //Action<NpgsqlDbContextOptionsBuilder> npgsqlOptionsAction = (o) =>
            //{
            //    o.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
            //    o.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
            //};

            return services;
        }

        /// <summary>
        /// AddKestrelOption
        /// </summary>
        /// <param name="services"></param>
        public static void AddKestrelOption(this IServiceCollection services)
        {
            // thêm option cho kestrel
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }
    }
}
