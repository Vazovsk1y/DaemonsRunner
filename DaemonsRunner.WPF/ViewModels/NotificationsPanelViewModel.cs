using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaemonsRunner.Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DaemonsRunner.WPF.ViewModels;

internal partial class NotificationsPanelViewModel : ObservableObject
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

	public NotificationsPanelViewModel() { }

	public NotificationsPanelViewModel(IDataBus dataBus)
	{
		_dataBus = dataBus;
		_subscriptions.Add(_dataBus.RegisterHandler<string>(OnMessageReceived));
		Notifications.CollectionChanged += (_, _) => ClearNotificationsCommand.NotifyCanExecuteChanged();
	}

	#endregion

	#region --Commands--

	[RelayCommand(CanExecute = nameof(CanClearNotifications))]
	private void ClearNotifications() => Notifications.Clear();

	private bool CanClearNotifications() => Notifications.Count > 0;

	#endregion

	#region --Methods--

	private async void OnMessageReceived(string message) =>
		await App.Current.Dispatcher.InvokeAsync(() => Notifications.Add($"[{DateTime.Now.ToShortTimeString()}]: {message}"));

	#endregion
}
