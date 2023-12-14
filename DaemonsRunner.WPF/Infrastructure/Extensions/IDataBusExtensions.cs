using DaemonsRunner.Application.Services.Interfaces;
using System.Collections.Generic;

namespace DaemonsRunner.Infrastructure.Extensions;

internal static class IDataBusExtensions
{
    public static void SendAll<T>(this IDataBus dataBus, IEnumerable<T> messages)
    {
        foreach(var message in messages) 
        {
            dataBus.Send(message);
        }
    }
}
