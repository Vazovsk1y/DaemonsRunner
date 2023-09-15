using DaemonsRunner.DAL.Repositories.Interfaces;
using DaemonsRunner.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DaemonsRunner.DAL.Extensions;

public static class Registrator
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services) => services
        .AddScoped<IScriptRepository, ScriptRepository>()
        ;
}
