using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.Commands;
using DaemonsRunner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DaemonsRunner.ViewModels
{
    internal class NotificationPanelViewModel : BaseViewModel, IDisposable
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

        public NotificationPanelViewModel() 
        {
            _notifications = new ObservableCollection<string>(Enumerable.Range(0, 10).Select(x => new string('x', new Random().Next(0, 15))));
        }

        public NotificationPanelViewModel(IDataBus dataBus)
        {
            _dataBus = dataBus;
            _subscriptions.Add(_dataBus.RegisterHandler<string>(OnMessageReceived));
        }

        #endregion

        #region --Commands--

        public ICommand ClearNotificationsCommand => new RelayCommand(
            (arg) => Notifications.Clear(),
            (arg) => Notifications.Count > 0);

        #endregion

        #region --Methods--

        public void Dispose() 
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
        }

        private async void OnMessageReceived(string message) =>
            await App.Current.Dispatcher.InvokeAsync(() => Notifications.Add($"[{DateTime.Now.ToShortTimeString()}]: {message}"));

        #endregion
    }
}
