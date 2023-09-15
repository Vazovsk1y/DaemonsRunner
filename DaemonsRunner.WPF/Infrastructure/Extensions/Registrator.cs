﻿using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.DAL;
using DaemonsRunner.Services;
using DaemonsRunner.ViewModels;
using DaemonsRunner.ViewModels.Interfaces;
using DaemonsRunner.WPF;
using DaemonsRunner.WPF.ViewModels;
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
        .AddTransient<IFileDialog, WPFFileDialogService>()
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
