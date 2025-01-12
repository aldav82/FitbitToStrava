using DataBase.Repository;

namespace FitBitToStravaApp.Infrastructure
{
    public static class DataBase
    {
        public static void AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IConfigRepository, ConfigRepository>();
            services.AddScoped<IExcerciseRepository, ExcerciseRepository>();



        }
    }
}
