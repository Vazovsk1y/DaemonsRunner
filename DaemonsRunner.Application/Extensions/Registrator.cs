using DaemonsRunner.Application.Services.Interfaces;
using DaemonsRunner.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DaemonsRunner.Application.Extensions;

public static class Registrator
{
    public static IServiceCollection AddBuisnessLayer(this IServiceCollection services) => services
        .AddSingleton<IDataBus, DataBusService>()
        .AddScoped<IScriptService, ScriptService>()
        ;

}
