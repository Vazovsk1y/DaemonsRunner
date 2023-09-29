using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace DaemonsRunner.WPF.Infrastructure.Extensions;

internal static class IHostBuilderExtensions
{
	public static IHostBuilder CreateAssociatedFolder(this IHostBuilder hostBuilder)
	{
		var companyDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.CompanyName);
		if (!Directory.Exists(companyDirectory))
		{
			Directory.CreateDirectory(companyDirectory);
		}

		if (!Directory.Exists(App.AssociatedFolderInAppDataPath))
		{
			Directory.CreateDirectory(App.AssociatedFolderInAppDataPath);
		}

		return hostBuilder;
	}
}
