using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Repositories.Interfaces;
using WarOfMinds.Repositories.Repositories;

namespace WarOfMinds.Repositories
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IGameRepository, GameRepository>();
            
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            
            services.AddScoped<IGamePlayerRepository, GamePlayerRepository>();
            return services;
        }
    }
}
