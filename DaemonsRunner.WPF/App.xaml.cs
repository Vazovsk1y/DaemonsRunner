using DaemonsRunner.Application.Extensions;
using DaemonsRunner.DAL;
using DaemonsRunner.DAL.Extensions;
using DaemonsRunner.Infrastructure.Extensions;
using DaemonsRunner.WPF;
using DaemonsRunner.WPF.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace DaemonsRunner;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
	#region --Fields--

	private static IHost? _host;

	public const string Name = "DaemonsRunner";

	public const string CompanyName = "Vazovskiy";

	#endregion

	#region --Properties--

	public static string WorkingDirectory => IsDesignMode ? Path.GetDirectoryName(GetSourceCodePath())! : Environment.CurrentDirectory;

	public static string AssociatedFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CompanyName, Name);

	public static bool IsDesignMode { get; private set; } = true;

	public static IServiceProvider Services => Host.Services;

	public static IHost Host => _host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

	#endregion

	#region --Constructors--

	public App()
	{
	}

	#endregion

	#region --Methods--

	public static void ConfigureServices(HostBuilderContext host, IServiceCollection services) => services
		.AddApplicationLayer()
		.AddDataAccessLayer()
		.AddWPF()
		;

	private static string GetSourceCodePath([CallerFilePath] string? path = null) => string.IsNullOrWhiteSpace(path)
		? throw new ArgumentNullException(nameof(path)) : path;

	public void ConfigureGlobalExceptionsHandler()
	{
		DispatcherUnhandledException += (sender, e) =>
		{
			var logger = Services.GetRequiredService<ILogger<App>>();
			logger.LogError(e.Exception, "Something went wrong in [{nameofDispatcherUnhandledException}]", nameof(DispatcherUnhandledException));
			e.Handled = true;
			Current?.Shutdown();
		};

		AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
		{
			var logger = Services.GetRequiredService<ILogger<App>>();
			logger.LogError(e.ExceptionObject as Exception, "Something went wrong in [{nameofCurrentDomainUnhandledException}].", nameof(AppDomain.CurrentDomain.UnhandledException));
		};

		TaskScheduler.UnobservedTaskException += (sender, e) =>
		{
			var logger = Services.GetRequiredService<ILogger<App>>();
			logger.LogError(e.Exception, "Something went wrong in [{UnobservedTaskException}].", nameof(TaskScheduler.UnobservedTaskException));
		};
	}

	protected override async void OnStartup(StartupEventArgs e)
	{
		IsDesignMode = false;
		base.OnStartup(e);
		await Host.StartAsync();
		Services.GetRequiredService<MainWindow>().Show();
	}

	protected override async void OnExit(ExitEventArgs e)
	{
		base.OnExit(e);
		using var host = Host;
		await host.StopAsync();
	}

	#endregion
}

