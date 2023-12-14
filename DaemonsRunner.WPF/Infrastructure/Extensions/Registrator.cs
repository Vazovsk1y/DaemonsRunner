using DaemonsRunner.Application.Services.Interfaces;
using DaemonsRunner.DAL;
using DaemonsRunner.Services;
using DaemonsRunner.WPF;
using DaemonsRunner.WPF.ViewModels;
using DaemonsRunner.WPF.ViewModels.Interfaces;
using DaemonsRunner.WPF.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace DaemonsRunner.Infrastructure.Extensions;

internal static class Registrator
{
    public static IServiceCollection AddWPF(this IServiceCollection services) => services
        .AddSingleton<IScriptExecutorViewModelFactory, ScriptExecutorViewModelFactory>()
        .AddSingleton<MainWindowViewModel>()
        .AddSingleton<ScriptsPanelViewModel>()
        .AddSingleton<NotificationPanelViewModel>()
        .AddTransient<IFileManager, WPFFileDialogService>()
        .AddSingleton<IStorage, Storage>(e => new Storage(App.AssociatedFolderInAppDataPath))
        .AddScoped<ScriptAddViewModel>()
        .AddSingleton(typeof(IUserDialog<>), typeof(BaseUserDialogService<>))
        .AddSingleton(s =>
        {
            var viewModel = s.GetRequiredService<MainWindowViewModel>();
            var window = new MainWindow { DataContext = viewModel };

            return window;
        })
        .AddTransient(s =>
        {
			var viewModel = s.GetRequiredService<ScriptAddViewModel>();
			var window = new ScriptAddWindow { DataContext = viewModel };

			return window;
		})
        ;
}
