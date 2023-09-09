using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DaemonsRunner.ViewModels;

internal class NotificationPanelViewModel : ObservableObject
{
	#region --Fields--

	private readonly IDataBus _dataBus;
	private readonly ObservableCollection<string> _notifications = new();
	private readonly ICollection<IDisposable> _subscriptions = new List<IDisposable>();

	#endregion

	#region --Properties--

	public ObservableCollection<string> Notifications => _notifications;

	#endregion

	#region --Constructors--

	public NotificationPanelViewModel() { }

	public NotificationPanelViewModel(IDataBus dataBus)
	{
		_dataBus = dataBus;
		_subscriptions.Add(_dataBus.RegisterHandler<string>(OnMessageReceived));
		_notifications.CollectionChanged += (_, _) => ClearNotificationsCommand.NotifyCanExecuteChanged();
	}

	#endregion

	#region --Commands--

	public IRelayCommand ClearNotificationsCommand => new RelayCommand(
		Notifications.Clear,
		() => Notifications.Count > 0);

	#endregion

	#region --Methods--

	private async void OnMessageReceived(string message) =>
		await App.Current.Dispatcher.InvokeAsync(() => Notifications.Add($"[{DateTime.Now.ToShortTimeString()}]: {message}"));

	#endregion
}
