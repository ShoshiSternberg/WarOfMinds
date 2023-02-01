using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
            services.AddScoped<IPlayerRatingBySubjectRepository, PlayerRatingBySubjectRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            return services;
        }
    }
}
