using AntivirusProgram.Repositories.Abstracts;
using AntivirusProgram.Repositories.EFCore;
using AntivirusProgram.Services;
using AntivirusProgram.Services.Abstracts;
using AntivirusProgram.Services.Clients;
using Microsoft.EntityFrameworkCore;

namespace AntivirusProgram.API.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
                services.AddDbContext<RepositoryContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("sqlConnectionServer")));
        /// <summary>
        /// IRepository Manager'in DI a eklenmesi Burası normalde daha sonra yapılacaktı fakat test için önden eklemem gerekti
        /// </summary>
        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
                services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services) =>
           services.AddScoped<IServiceManager, ServiceManager>();

        public static void ConfigureVirusTotalClient(this IServiceCollection services)=>
            services.AddHttpClient<IVirusTotalClient, VirusTotalClient>();

    }
}
