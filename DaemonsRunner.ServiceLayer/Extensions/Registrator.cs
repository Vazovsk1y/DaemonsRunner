using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.BuisnessLayer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DaemonsRunner.BuisnessLayer.Extensions;

public static class Registrator
{
    public static IServiceCollection AddBuisnessLayer(this IServiceCollection services) => services
        .AddSingleton<IDataBus, DataBusService>()
        .AddScoped<IScriptService, ScriptService>()
        ;

}
