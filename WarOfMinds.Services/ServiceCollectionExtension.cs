using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Repositories;
using WarOfMinds.Services.Interfaces;
using WarOfMinds.Services.Services;

namespace WarOfMinds.Services
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddRepositories();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IPlayerService, PlayerService>();

            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IEloCalculator, EloCalculator>();
            services.AddSingleton<IDictionary<int, PlayerForCalcRating>>(opts => new Dictionary<int, PlayerForCalcRating>());

            services.AddAutoMapper(typeof(MappingProfile));
            //מה זה
            //services.AddMemoryCache();

            return services;
        }
    }
}
