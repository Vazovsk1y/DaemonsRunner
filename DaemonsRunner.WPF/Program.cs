using DaemonsRunner.WPF.Infrastructure.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Threading;

namespace DaemonsRunner;

internal class Program
{
	public static bool IsInDebug { get; private set; }

	private static Mutex? _mutex;

	[STAThread]
	public static void Main(string[] args)
	{
		_mutex = new Mutex(true, App.Name, out bool createdNew);
		if (!createdNew)
		{
			return;
		}


#if DEBUG
		IsInDebug = true;
#endif

		App app = new();
		app.ConfigureGlobalExceptionsHandler();
		app.InitializeComponent();
		app.Run();
	}

	public static IHostBuilder CreateHostBuilder(string[] args)
	{
		Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", IsInDebug ? "Development" : "Production");

		return Host
		.CreateDefaultBuilder(args)
		.CreateApplicationAssociatedFolder()
		.ConfigureAppConfiguration((a, e) =>
		{
			a.HostingEnvironment.ApplicationName = App.Name;
		})
		.UseSerilog((host, loggingConfiguration) =>
		{
			string logFileName = "log.txt";
			string logDirectory = Path.Combine(App.AssociatedFolderPath, "logs");
			if (!Directory.Exists(logDirectory))
			{
				Directory.CreateDirectory(logDirectory);
			}

			string logFileFullPath = Path.Combine(logDirectory, logFileName);
			loggingConfiguration.MinimumLevel.Information();

			if (host.HostingEnvironment.IsDevelopment())
			{
				loggingConfiguration.WriteTo.Debug();
			}
			else
			{
				loggingConfiguration.WriteTo.File(logFileFullPath, rollingInterval: RollingInterval.Day);
			}
		})
		.ConfigureServices(App.ConfigureServices)
		;
	}
}
