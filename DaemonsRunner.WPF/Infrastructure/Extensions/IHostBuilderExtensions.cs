using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace DaemonsRunner.WPF.Infrastructure.Extensions;

internal static class IHostBuilderExtensions
{
	public static IHostBuilder CreateApplicationAssociatedFolder(this IHostBuilder hostBuilder)
	{
		var companyFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.CompanyName);
		if (!Directory.Exists(companyFolder))
		{
			Directory.CreateDirectory(companyFolder);
		}

		if (!Directory.Exists(App.AssociatedFolderPath))
		{
			Directory.CreateDirectory(App.AssociatedFolderPath);
		}

		return hostBuilder;
	}
}
